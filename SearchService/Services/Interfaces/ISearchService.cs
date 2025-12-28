using SearchService.DTO;

namespace SearchService.Services.Interfaces
{
    public interface ISearchService
    {
        Task<PagedResult<SearchResultItem>> SearchAsync(SearchRequest request, CancellationToken ct);
    }
}
