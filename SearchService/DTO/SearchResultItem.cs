using SearchService.Enums;

namespace SearchService.DTO
{
    public sealed record SearchResultItem(
    Guid AccommodationId,
    string Name,
    int MinGuests,
    int MaxGuests,
    PriceType PriceType,
    string? City,
    string? Country,
    decimal TotalPrice);
}
