namespace MicroservicesDemoFE.Api.Models
{
    public class Movie
    {
        public string? Id { get; set; }
        public string Title { get; set; } = null!;
        public string Director { get; set; } = null!;
        public int Year { get; set; }
    }
}
