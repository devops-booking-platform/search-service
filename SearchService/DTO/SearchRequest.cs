using System.ComponentModel.DataAnnotations;

namespace SearchService.DTO
{
    public sealed class SearchRequest
    {
        public string? City { get; init; }
        public string? Country { get; init; }

        [Range(1, int.MaxValue, ErrorMessage = "Guests must be >= 1.")]
        public int Guests { get; init; }

        [Required]
        public DateTimeOffset? Start { get; init; }

        [Required]
        public DateTimeOffset? End { get; init; }
    }
}
