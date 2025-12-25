using SearchService.DTO;

namespace SearchService.Services.Interfaces
{
    public interface ISearchService
    {
        Task<IReadOnlyList<SearchResultItem>> SearchAsync(
            string? city,
            string? country,
            int guests,
            DateTimeOffset start,
            DateTimeOffset end,
            CancellationToken ct);
    }
}
