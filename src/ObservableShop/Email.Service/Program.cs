// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using EmailService;
using EmailService.OpenTelemetry;
using EmailService.Services.Eventing;

var builder = Host.CreateApplicationBuilder(args);
builder.AddCustomOpenTelemetry("email.service");
builder.Services.AddEventing();
builder.Services.AddHostedService<NewOrderWorker>();

var host = builder.Build();
host.Run();
