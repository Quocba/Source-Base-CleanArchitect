using System;

namespace Application.Payload.Response.RiceBox
{
    public class RiceBoxResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? WareHouseId { get; set; }
        public string? WareHouseName { get; set; }
    }
}
