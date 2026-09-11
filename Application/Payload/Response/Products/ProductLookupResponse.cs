using System;

namespace Application.Payload.Response.Products
{
    public class ProductLookupResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
