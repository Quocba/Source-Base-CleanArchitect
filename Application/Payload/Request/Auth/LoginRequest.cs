using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payload.Request.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Nhập tên đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        public string Password { get; set; }
    }
}
