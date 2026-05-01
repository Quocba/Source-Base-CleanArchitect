using System;

namespace Application.Payload.Base
{
    public class LogMessage
    {
        public string Action { get; set; }         // Ví dụ: "TẠO SẢN PHẨM MỚI"
        public string UserName { get; set; }       // Người dùng
        public string ID { get; set; }             // Mã sản phẩm
        public string Name { get; set; }           // Tên sản phẩm
        public DateTime Timestamp { get; set; }    // Thời gian

        public override string ToString()
        {
            return
                $"""
                🟢 [IIT.VN LOG]
                🧑‍💻 Người dùng: {UserName}
                🆔 Mã sản phẩm: {ID}
                🏷️ Tên sản phẩm: {Name}
                ⏰ Thời gian: {Timestamp:dd/MM/yyyy HH:mm:ss}
                🔧 Hành động: {Action}
                """;
        }
    }
}
