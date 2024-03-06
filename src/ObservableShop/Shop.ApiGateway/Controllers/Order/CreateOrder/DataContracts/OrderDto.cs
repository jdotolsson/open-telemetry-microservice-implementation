namespace Shop.ApiGateway.Controllers.Order.CreateOrder.DataContracts
{
    public class OrderDto
    {
        public List<ArticleDto> Articles { get; set; } = new();
    }

    public class ArticleDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
