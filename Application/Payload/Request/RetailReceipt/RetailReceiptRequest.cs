using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Payload.Request.RetailReceipt
{
    public class RetailReceiptRequest
    {
        [Required(ErrorMessage = "Nhập tên khách hàng")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nhập địa chỉ")]
        public string SalesAddress { get; set; } = string.Empty;

        public string? ReasonDiscount { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ReceiptNo { get; set; }
        public decimal? TotalAmounts { get; set; }

        [Required(ErrorMessage = "Nhập chi tiết sản phẩm")]
        public List<RetailReceiptDetailRequest> Details { get; set; } = new();
    }

    public class RetailReceiptDetailRequest
    {
        [Required(ErrorMessage = "Chọn sản phẩm")]
        public Guid RiceId { get; set; }

        [Required(ErrorMessage = "Nhập số lượng")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Nhập giá bán")]
        public decimal Price { get; set; }

        public decimal? TotalAmounts { get; set; }
    }
}
