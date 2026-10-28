namespace Application.Payload.Response.Users
{
    public class StorageCategoryDbResult
    {
        public string CategoryKey { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int TotalFiles { get; set; }
        public long TotalBytes { get; set; }
        public long StorageCapacity { get; set; }
    }
}
