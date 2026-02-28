namespace SearchService.Documents
{
    public class LocationDocument
    {
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
    }
}
