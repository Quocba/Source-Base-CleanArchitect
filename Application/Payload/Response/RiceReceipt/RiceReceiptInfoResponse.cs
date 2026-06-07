namespace Application.Payload.Response.RiceReceipt
{
    public class RiceReceiptInfoResponse
    {
        public Guid Id { get; set; }
        public string ReceiptNo { get; set; }
        public string? Name { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Weight { get; set; }
        public decimal? TotalAmounts { get; set; }
        public List<FinishedRiceResponse> finishedRice { get; set; }
        public List<IngredientRice> IngredientRices { get; set; }
        public List<Packing> Packings { get; set; }
    }

    public class FinishedRiceResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string? Name { get; set; }
        public string Unit { get; set; }
        public decimal Weight { get; set; }
        public decimal TotalAmounts { get; set; }
    }

    public class IngredientRice
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Weight { get; set; }
        public decimal Price { get; set; }
    }

    public class Packing
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public int PackingQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmounts { get; set; }
    }
}
