using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Response.Auth
{
    public class LoginResposne
    {
        public string? UserName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLock { get; set; }
        public string? Role { get; set;}
        public string? Token { get; set; }
        public double TotalStorage { get; set; } = 0;
        public double UsedStorage { get; set; } = 0;
        public double FreeStorage { get; set; } = 0;
        public long TotalStorageBytes { get; set; } = 0;
        public long UsedStorageBytes { get; set; } = 0;
        public long FreeStorageBytes { get; set; } = 0;
    }
}
