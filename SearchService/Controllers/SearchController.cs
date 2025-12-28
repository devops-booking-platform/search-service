using Microsoft.AspNetCore.Mvc;
using SearchService.DTO;
using SearchService.Services.Interfaces;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public sealed class SearchController(ISearchService searchService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Search([FromBody] SearchRequest request, CancellationToken ct)
        {
            var results = await searchService.SearchAsync(request, ct);
            return Ok(results);
        }
    }
}
