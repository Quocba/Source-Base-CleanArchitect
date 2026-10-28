namespace Application.Payload.Response.Users
{
    public class StorageCategoryResponse
    {
        public string CategoryKey { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int TotalFiles { get; set; }
        public long TotalBytes { get; set; }
        public double SizeGB { get; set; }
        public double PercentageOfUsed { get; set; }
        public string SubText { get; set; } = string.Empty;
    }
}
