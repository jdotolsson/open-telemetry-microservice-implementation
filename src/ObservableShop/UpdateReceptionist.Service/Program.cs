// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using UpdateReceptionist.Service;
using UpdateReceptionist.Service.OpenTelemetry;
using UpdateReceptionist.Service.Services.Eventing;


var builder = Host.CreateApplicationBuilder(args);
builder.AddCustomOpenTelemetry("update_receptionist.service");
builder.Services.AddEventing();
builder.Services.AddHostedService<UpdateProductPricePeriodically>();

var host = builder.Build();
host.Run();
