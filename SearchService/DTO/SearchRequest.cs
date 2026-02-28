using System.ComponentModel.DataAnnotations;

namespace SearchService.DTO
{
    public sealed class SearchRequest : PagedRequest
    {
        public string? City { get; init; }
        public string? Country { get; init; }

        [Range(1, int.MaxValue, ErrorMessage = "Guests must be >= 1.")]
        public int Guests { get; init; }

        [Required]
        public DateOnly Start { get; init; }

        [Required]
        public DateOnly End { get; init; }
    }
}
