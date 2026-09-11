using Domain.Payload.Base;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<ApiResponse<string>>
    {
        [Required(ErrorMessage = "Mã danh mục không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Mã danh mục phải từ 2 đến 50 ký tự.")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Tên danh mục không được để trống.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên danh mục phải từ 2 đến 100 ký tự.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
