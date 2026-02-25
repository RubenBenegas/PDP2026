namespace Reviews.Api.Settings
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ReviewsCollectionName { get; set; }
    }
}
