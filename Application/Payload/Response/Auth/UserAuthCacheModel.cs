using System;

namespace Application.Payload.Response.Auth
{
    public class UserAuthCacheModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLock { get; set; }
        public string? RoleName { get; set; }
        public long StorageCapacity { get; set; }

        public UserAuthCacheModel() { }

        public UserAuthCacheModel(Guid id, string? userName, string? passwordHash, bool isDeleted, bool isLock, string? roleName, long storageCapacity = 0)
        {
            Id = id;
            UserName = userName;
            PasswordHash = passwordHash;
            IsDeleted = isDeleted;
            IsLock = isLock;
            RoleName = roleName;
            StorageCapacity = storageCapacity;
        }
    }
}
