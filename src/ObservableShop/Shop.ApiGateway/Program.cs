// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Shop.ApiGateway.HttpClients;
using Shop.ApiGateway.OpenTelemetry;
using Shop.ApiGateway.Services.Eventing;

var builder = WebApplication.CreateBuilder(args)
        .AddCustomOpenTelemetry("shop.api-gateway");

// Add services to the container.

builder.Services.AddHttpClients();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(schema => schema.FullName?.Replace("+", ".")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
builder.Services.AddEventing();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
