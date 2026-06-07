using System;
using System.Text.Json.Serialization;

namespace Application.Payload.Response.ReturnSells
{
    public class ReturnSellDetailsResponse
    {
        public Guid Id { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid? RiceId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid? BulkRiceId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public Guid? PackingId { get; set; }
        public string? PackingName { get; set; }
        public int NumberPacking { get; set; }
        public decimal Weight { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmounts { get; set; }
        public Guid RiceBoxId { get; set; }
        public string? RiceBox { get; set; }
    }
}
