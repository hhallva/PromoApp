namespace DataLayer.DTOs
{
    public class PostPromocodeDto
    {
        public string Code { get; set; } = null!;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
