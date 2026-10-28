using System;

namespace Application.Payload.Response.Users
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public string? Role { get; set; }
        public bool IsLock { get; set; }
        public long StorageCapacity { get; set; } = 5368709120;
        public double TotalStorage { get; set; } = 0;
        public double UsedStorage { get; set; } = 0;
        public double FreeStorage { get; set; } = 0;
        public long TotalStorageBytes { get; set; } = 0;
        public long UsedStorageBytes { get; set; } = 0;
        public long FreeStorageBytes { get; set; } = 0;
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
