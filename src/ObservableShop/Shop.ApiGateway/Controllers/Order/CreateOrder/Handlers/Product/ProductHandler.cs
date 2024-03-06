// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MediatR;
using ProductClient;
using Shop.ApiGateway.Controllers.Order.CreateOrder.Events;

namespace Shop.ApiGateway.Controllers.Order.CreateOrder.Handlers.Product
{
    public class ProductHandler(IMediator mediator, ProductService.ProductServiceClient client) : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IMediator _mediator = mediator;
        private readonly ProductService.ProductServiceClient _client = client;

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            var articles = notification.Request.Order.Articles.Select(x => new ProductClient.Article
            {
                Id = x.Id.ToString(),
                Price = x.Price.ToString(),
            }).ToList();
            var request = new ArticlesRequest();
            request.Articles.AddRange(articles);
            var response = await _client.CheckAvailabilityAsync(request, cancellationToken: cancellationToken);        

            notification.Data.Product.OrderedProducts.AddRange(response.Articles.Where(x=> x.Available).Select(ToArticle));
            notification.Data.Product.MissingInventory.AddRange(response.Articles.Where(x => !x.Available).Select(ToArticle));

            await _mediator.Publish(new ProductReservedEvent(notification.OrderId, notification.Request, notification.Data), cancellationToken);
        }

        public Models.Product.Article ToArticle(ArticleResponse dto)
        {
            return new Models.Product.Article
            {
                Id = dto.Id,
                Price = decimal.Parse(dto.Price)
            };
        }
    }
}
