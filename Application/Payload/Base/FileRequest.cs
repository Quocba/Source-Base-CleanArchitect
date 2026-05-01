using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base
{
    public class FileRequest
    {
        [Required]
        [MaxLength(50)]
        public string Folder { get; set; }
        [Required]
        public IFormFile File { get; set; }
        [MaxLength(50)]
        public string? FileName { get; set; }
        public bool IsOverwrite { get; set; } = false;
    }
}
