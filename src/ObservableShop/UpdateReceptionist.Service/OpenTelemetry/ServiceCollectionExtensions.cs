// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using UpdateReceptionist.Service.Services.Eventing;

namespace UpdateReceptionist.Service.OpenTelemetry
{
    public static class ServiceCollectionExtensions
    {
        public static HostApplicationBuilder AddCustomOpenTelemetry(this HostApplicationBuilder builder, string serviceName)
        {
            var resourceAttributes = new KeyValuePair<string, object>[]
            {
                new("service.version", "0.0.1-alpha")
            };
            var resource = ResourceBuilder.CreateDefault()
                .AddService(serviceName)
                .AddAttributes(resourceAttributes);
            builder.Logging.AddOpenTelemetry(options =>
            {
                options
                    .SetResourceBuilder(resource)
                    .AddConsoleExporter()
                    .AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = builder.Configuration.GetValue<Uri>("OTEL_EXPORTER_OTLP_ENDPOINT");
                        exporter.Protocol = OtlpExportProtocol.Grpc;
                        exporter.BatchExportProcessorOptions.ScheduledDelayMilliseconds = 100;
                        exporter.BatchExportProcessorOptions.MaxExportBatchSize = 20;
                    });
            });
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService(serviceName)
                    .AddAttributes(resourceAttributes))
                .WithTracing(tracing => tracing
                    .AddConsoleExporter()
                    .AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = builder.Configuration.GetValue<Uri>("OTEL_EXPORTER_OTLP_ENDPOINT");
                        exporter.Protocol = OtlpExportProtocol.Grpc;
                        exporter.BatchExportProcessorOptions.ScheduledDelayMilliseconds = 100;
                        exporter.BatchExportProcessorOptions.MaxExportBatchSize = 20;
                    })
                    .AddSource(nameof(RabbitMQEventBus)))
                .WithMetrics(metrics => metrics
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = builder.Configuration.GetValue<Uri>("OTEL_EXPORTER_OTLP_ENDPOINT");
                        exporter.Protocol = OtlpExportProtocol.Grpc;
                        exporter.BatchExportProcessorOptions.ScheduledDelayMilliseconds = 100;
                        exporter.BatchExportProcessorOptions.MaxExportBatchSize = 20;
                    }));

            return builder;
        }
    }
}
