namespace SearchService.Documents
{
    public class AvailabilityDocument
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public decimal Price { get; set; }
    }
}
