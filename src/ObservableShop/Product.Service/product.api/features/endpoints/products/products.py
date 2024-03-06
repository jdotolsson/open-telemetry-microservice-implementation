
import logging
from features.endpoints.products import product_pb2
from features.endpoints.products import product_pb2_grpc
from data import database
from opentelemetry import trace

class ProductService(product_pb2_grpc.ProductServiceServicer):
    def CheckAvailability(self, request, context):
        availability_list = []
        tracer = trace.get_tracer("article.tracer")
        logging.info("Checking product availability")
        for article in request.Articles:
            with tracer.start_as_current_span("availability.check"):
                is_available = database.isProductAvailable(article.Id)
                updated_price = database.getPrice(article.Id)
                if updated_price == -1:
                    logging.error("Article with id '%s' is not listed!", article.Id)
                isUpdated = updated_price != article.Price and updated_price != -1
                current_span = trace.get_current_span()
                current_span.set_attribute("product.price.isUpdated", isUpdated)
                current_span.set_attribute("product.price.newPrice", updated_price if updated_price != -1 else "Not_Listed")
                current_span.set_attribute("product.price.oldPrice", article.Price)

                article_response = product_pb2.ArticleResponse(
                    Id=article.Id,
                    Price=article.Price if updated_price == -1 else updated_price,
                    available=is_available
                )

                availability_list.append(article_response)
        return product_pb2.AvailabilityResponse(Articles=availability_list)
    
def addToServer(server):
    product_pb2_grpc.add_ProductServiceServicer_to_server(ProductService(), server)
