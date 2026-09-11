using Domain.Enums;
using Domain.Payload.Base;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Features.Products.Commands.EditProduct
{
    public class EditProductCommand : IRequest<ApiResponse<string>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên sản phẩm phải từ 2 đến 200 ký tự.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Giá sản phẩm phải từ 0 đến 1,000,000,000 VNĐ.")]
        public decimal Price { get; set; }

        [Range(0, 1000000, ErrorMessage = "Số lượng tồn kho phải từ 0 đến 1,000,000.")]
        public int StockQuantity { get; set; }

        public ProductStatus Status { get; set; }

        public Guid? CategoryId { get; set; }
    }
}
