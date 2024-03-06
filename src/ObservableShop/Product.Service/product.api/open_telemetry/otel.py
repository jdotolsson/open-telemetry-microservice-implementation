import os
import logging
from opentelemetry.sdk.resources import SERVICE_NAME, SERVICE_VERSION, Resource

from opentelemetry import trace
from opentelemetry.exporter.otlp.proto.grpc.trace_exporter import OTLPSpanExporter
from opentelemetry.instrumentation.grpc import GrpcInstrumentorServer
from opentelemetry.sdk.trace import  TracerProvider
from opentelemetry.sdk.trace.export import (
    ConsoleSpanExporter,
    SimpleSpanProcessor,
    BatchSpanProcessor
)

from opentelemetry import metrics
from opentelemetry.exporter.otlp.proto.grpc.metric_exporter import OTLPMetricExporter
from opentelemetry.sdk.metrics import MeterProvider
from opentelemetry.sdk.metrics.export import PeriodicExportingMetricReader

from opentelemetry._logs import set_logger_provider
from opentelemetry.exporter.otlp.proto.grpc._log_exporter import (
    OTLPLogExporter
)
from opentelemetry.sdk._logs import LoggerProvider, LoggingHandler
from opentelemetry.sdk._logs.export import (
    BatchLogRecordProcessor,
    SimpleLogRecordProcessor,
    ConsoleLogExporter
)

from opentelemetry.instrumentation.psycopg2 import Psycopg2Instrumentor
from opentelemetry.instrumentation.pika import PikaInstrumentor

def configureOpenTelemetry():
    def configureTraces(resource):
        endpoint = os.getenv("OTEL_EXPORTER_OTLP_ENDPOINT")
        traceProvider = TracerProvider(resource=resource)
        processor = BatchSpanProcessor(OTLPSpanExporter(endpoint=endpoint, insecure=True))
        traceProvider.add_span_processor(processor)
        trace.set_tracer_provider(traceProvider)
        trace.get_tracer_provider().add_span_processor(
            SimpleSpanProcessor(ConsoleSpanExporter())
        )

    def configureMetrics(resource):
        endpoint = os.getenv("OTEL_EXPORTER_OTLP_ENDPOINT")
        reader = PeriodicExportingMetricReader(
            OTLPMetricExporter(endpoint=endpoint, insecure=True)
        )
        meterProvider = MeterProvider(resource=resource, metric_readers=[reader])
        metrics.set_meter_provider(meterProvider)


    def configureExpirementalLogging(resource):
        endpoint = os.getenv("OTEL_EXPORTER_OTLP_ENDPOINT")
        logger_provider = LoggerProvider(
            resource=resource
        )
        set_logger_provider(logger_provider)
        exporter = OTLPLogExporter(endpoint=endpoint, insecure=True)
        logger_provider.add_log_record_processor(BatchLogRecordProcessor(exporter))
        logger_provider.add_log_record_processor(
            SimpleLogRecordProcessor(ConsoleLogExporter()))
        handler = LoggingHandler(level=logging.DEBUG, logger_provider=logger_provider)

        # Attach OTLP handler to root logger
        logging.getLogger().addHandler(handler)

    def configureDatabaseInstrumentor():
        Psycopg2Instrumentor().instrument(enable_commenter=True, commenter_options={})
        
    def configureRabbitMQInstrumentor():
        PikaInstrumentor().instrument()

    resource = Resource(attributes={
        SERVICE_NAME: "product.service",
        SERVICE_VERSION: "0.0.1-alpha"
    })

    configureTraces(resource)
    configureMetrics(resource)
    configureExpirementalLogging(resource)
    configureDatabaseInstrumentor()
    configureRabbitMQInstrumentor()
    
    grpc_server_instrumentor = GrpcInstrumentorServer()
    grpc_server_instrumentor.instrument()
    