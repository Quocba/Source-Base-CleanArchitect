namespace Application.Payload.Response.ReturnSells
{
    public class ReturnSellInfoResponse
    {
        public Guid Id { get; set; }
        public string? ReturnSellNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? SellRiceOrSellBulkRiceNo { get; set; }
        public Guid? BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }
        public string? Driver { get; set; }
        public string? CarNumber { get; set; }
        public string? Note { get; set; }
        public decimal? TotalAmounts { get; set; }
        public string? SpentType { get; set; }
        public List<ReturnSellDetailsResponse> Details { get; set; } = new();
    }
}
