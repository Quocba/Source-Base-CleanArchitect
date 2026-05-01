using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Config
{
    public class GoogleDriveConfig
    {
        public string FolderId { get; set; }
        public string CredentialsFile { get; set; }
        public string TokenFolder { get; set; }
    }
}
