import logging
import os
from concurrent import futures

import grpc
from features.endpoints.products import products
from features.consumers.update_product import update_product_handler
from open_telemetry import otel

logging.basicConfig(level=os.environ.get("LOGLEVEL", "INFO"))

otel.configureOpenTelemetry()

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    products.addToServer(server)
    server.add_insecure_port("[::]:50051")
    server.start()
    update_product_handler.start_product_update_consumer()
    server.wait_for_termination()

if __name__ == "__main__":
    serve()