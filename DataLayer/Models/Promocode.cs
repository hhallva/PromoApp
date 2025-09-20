namespace DataLayer.Models;

public partial class Promocode
{
    public string Code { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }
}
