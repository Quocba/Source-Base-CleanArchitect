using System;
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Active;

        public Guid? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }
}
