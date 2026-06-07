namespace Application.Payload.Response.RiceReceipt
{
    public class RiceReceiptDetailResponse
    {
        public Guid? Id { get; set; }

        public Guid? RiceId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }

        public Guid? PackingId { get; set; }
        public string? PackingCode { get; set; }
        public string? PackingName { get; set; }
        public decimal? PackingWeight { get; set; }

        public int? PackingQuantity { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalAmounts { get; set; }

        public Guid? RiceBoxId { get; set; }
        public string? RiceBox { get; set; }
    }
}
