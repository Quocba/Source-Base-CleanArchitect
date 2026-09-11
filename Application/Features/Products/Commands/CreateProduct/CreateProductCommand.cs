using Domain.Enums;
using Domain.Payload.Base;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<ApiResponse<string>>
    {
        [Required(ErrorMessage = "Mã sản phẩm không được để trống.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Mã sản phẩm phải từ 3 đến 50 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Mã sản phẩm chỉ được chứa chữ cái, chữ số, gạch ngang và gạch dưới.")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên sản phẩm phải từ 2 đến 200 ký tự.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Giá sản phẩm phải từ 0 đến 1,000,000,000 VNĐ.")]
        public decimal Price { get; set; }

        [Range(0, 1000000, ErrorMessage = "Số lượng tồn kho phải từ 0 đến 1,000,000.")]
        public int StockQuantity { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Active;

        public Guid? CategoryId { get; set; }
    }
}
