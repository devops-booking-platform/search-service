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
            var results = await searchService.SearchAsync(
                city: request.City,
                country: request.Country,
                guests: request.Guests,
                start: request.Start!.Value,
                end: request.End!.Value,
                ct: ct);

            return Ok(results);
        }
    }
}
