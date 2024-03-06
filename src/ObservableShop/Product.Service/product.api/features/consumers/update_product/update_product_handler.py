import logging
import os
import pika
import json
from features.consumers .update_product.datacontracts import message as datacontracts
from data import database
from opentelemetry import trace
import time

def establish_rabbitmq_connection():
    credentials = pika.PlainCredentials(username=os.getenv("RABBITMQ_USERNAME"), password=os.getenv("RABBITMQ_PASSWORD"))
    connection_params = pika.ConnectionParameters(host=os.getenv("RABBITMQ_HOST"), credentials=credentials)
    
    try:
        connection = pika.BlockingConnection(connection_params)
        return connection
    except pika.exceptions.AMQPConnectionError as e:
        logging.error("RabbitMQ connection error: %s", str(e))
        return None

def start_product_update_consumer():
    max_reconnection_attempts = 5
    reconnection_delay = 5  # seconds

    current_attempt = 1
    connection = establish_rabbitmq_connection()

    while connection is None and current_attempt <= max_reconnection_attempts:
        print(f"Reconnection attempt {current_attempt}/{max_reconnection_attempts}...")
        time.sleep(reconnection_delay)
        connection = establish_rabbitmq_connection()
        current_attempt += 1

    if connection is not None:
        channel = connection.channel()
        queue_name = 'product.price.update'
        channel.queue_declare(queue=queue_name)

        def case_insensitive_dict_hook(d):
            return {key.lower(): value for key, value in d.items()}

        def update_product_price(ch, method, properties, body):
            tracer = trace.get_tracer("article.tracer")
            with tracer.start_as_current_span("product.price.updated"):
                product_price_update_model = json.loads(body, object_hook=lambda d: datacontracts.Message(**case_insensitive_dict_hook(d)))
                success = database.update_price(product_price_update_model.id, product_price_update_model.newprice)
                current_span = trace.get_current_span()
                current_span.set_attribute("product.id", product_price_update_model.id)
                current_span.set_attribute("new.price", product_price_update_model.newprice)
                current_span.set_attribute("success", success)

                if not success:
                    logging.error("Product with id '%s' could not be updated", product_price_update_model.id)

        channel.basic_consume(queue=queue_name, on_message_callback=update_product_price, auto_ack=True)

        print(' [*] Waiting for messages. To exit press CTRL+C')
        channel.start_consuming()
    else:
        print("Max reconnection attempts reached. Unable to establish RabbitMQ connection.")

if __name__ == "__main__":
    start_product_update_consumer()
