namespace MicroservicesDemoFE.Api.Models
{
    public class Review
    {
        public string Id { get; set; }
        public string MovieId { get; set; }
        public string Reviewer { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
