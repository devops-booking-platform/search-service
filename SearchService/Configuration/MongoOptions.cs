namespace SearchService.Configuration
{
    public sealed class MongoOptions
    {
        public string ConnectionString { get; set; } = default!;
        public string Database { get; set; } = default!;
        public string AccommodationsCollection { get; set; } = default!;
    }
}
