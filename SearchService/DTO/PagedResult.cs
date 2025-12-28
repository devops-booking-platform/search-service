namespace SearchService.DTO
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = null!;
        public long TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
