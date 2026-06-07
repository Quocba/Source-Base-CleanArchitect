public class WareHouseResponse
{
    public Guid Id { get; set; }

    public string? Code { get; set; } = null!;
    public string? Name { get; set; } = null!;

    public string? Area { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? Manager { get; set; }
    public string? Lat { get; set; }
    public string? Lon { get; set; }
    public string? Phone { get; set; }
    public string? TaxCode { get; set; }
    public string? Logo { get; set; }
    public string? Status { get; set; }
    public string? Note { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = null!;

    public DateTime? LastModidifedDate { get; set; }
    public string? LastModidifedBy { get; set; }
}
