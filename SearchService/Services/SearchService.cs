using MongoDB.Driver;
using SearchService.Documents;
using SearchService.DTO;
using SearchService.Enums;
using SearchService.Services.Interfaces;

namespace SearchService.Services
{
    public sealed class SearchService : ISearchService
    {
        private readonly IMongoCollection<AccommodationDocument> _collection;
        public SearchService(IMongoCollection<AccommodationDocument> collection)
        {
            _collection = collection;
        }
        public async Task<IReadOnlyList<SearchResultItem>> SearchAsync(
            string? city,
            string? country,
            int guests,
            DateTimeOffset start,
            DateTimeOffset end,
            CancellationToken ct)
        {
            if (guests <= 0)
                throw new ArgumentOutOfRangeException(nameof(guests), "Number of guests can not be 0.");

            if (start >= end)
                throw new ArgumentOutOfRangeException(nameof(end), "End date must be after start.");

            var (startDay, endDay) = NormalizeToNightsUtc(start, end);

            var startMidnightUtc = new DateTimeOffset(startDay.Year, startDay.Month, startDay.Day, 0, 0, 0, TimeSpan.Zero);
            var endMidnightUtc = new DateTimeOffset(endDay.Year, endDay.Month, endDay.Day, 0, 0, 0, TimeSpan.Zero);

            var filter = BuildFilter(city, country, guests, startMidnightUtc, endMidnightUtc);

            var docs = await _collection.Find(filter).ToListAsync(ct);
            var results = new List<SearchResultItem>();
            foreach (var d in docs)
            {
                try
                {
                    results.Add(MapToResult(d, guests, startMidnightUtc, endMidnightUtc));
                }
                catch (InvalidOperationException)
                {
                }
            }
            return results;
        }

        private static FilterDefinition<AccommodationDocument> BuildFilter(
           string? city,
           string? country,
           int guests,
           DateTimeOffset start,
           DateTimeOffset end)
        {
            var fb = Builders<AccommodationDocument>.Filter;
            var filter = fb.Empty;

            if (!string.IsNullOrWhiteSpace(city))
                filter &= fb.Eq("Location.City", city.Trim());

            if (!string.IsNullOrWhiteSpace(country))
                filter &= fb.Eq("Location.Country", country.Trim());

            filter &= fb.Lte(x => x.MinGuests, guests) & fb.Gte(x => x.MaxGuests, guests);

            filter &= fb.ElemMatch(x => x.Availabilities, a => a.EndDate > start && a.StartDate < end);

            filter &= fb.Not(fb.ElemMatch(
                x => x.Reservations,
                r => start < r.EndDate && end > r.StartDate));

            return filter;
        }

        private static (DateOnly StartDay, DateOnly EndDay) NormalizeToNightsUtc(
            DateTimeOffset start,
            DateTimeOffset end)
        {
            var startUtc = start.ToUniversalTime();
            var endUtc = end.ToUniversalTime();

            var startMidnightUtc = new DateTimeOffset(startUtc.Year, startUtc.Month, startUtc.Day, 0, 0, 0, TimeSpan.Zero);
            var endMidnightUtc = new DateTimeOffset(endUtc.Year, endUtc.Month, endUtc.Day, 0, 0, 0, TimeSpan.Zero);

            var startDay = DateOnly.FromDateTime(startMidnightUtc.UtcDateTime);
            var endDay = DateOnly.FromDateTime(endMidnightUtc.UtcDateTime);

            if (endDay <= startDay)
                throw new ArgumentOutOfRangeException(nameof(end), "End date must be after start date.");

            return (startDay, endDay);
        }

        private static decimal CalculateTotalPriceFromDoc(
            AccommodationDocument d,
            int guests,
            DateOnly startDay,
            DateOnly endDay)
        {
            var dayIntervals = d.Availabilities
                .Select(a => new
                {
                    StartDay = DateOnly.FromDateTime(a.StartDate.UtcDateTime),
                    EndDay = DateOnly.FromDateTime(a.EndDate.UtcDateTime),
                    a.Price
                })
                .Where(a => a.EndDay > startDay && a.StartDay < endDay)
                .OrderBy(a => a.StartDay.DayNumber)
                .ToList();

            if (dayIntervals.Count == 0)
                throw new InvalidOperationException("No availability intervals found for requested dates.");

            var points = new List<DateOnly> { startDay, endDay };
            foreach (var it in dayIntervals)
            {
                if (it.StartDay > startDay && it.StartDay < endDay) points.Add(it.StartDay);
                if (it.EndDay > startDay && it.EndDay < endDay) points.Add(it.EndDay);
            }

            points = points
                .Distinct()
                .OrderBy(x => x.DayNumber)
                .ToList();

            decimal total = 0m;

            for (int i = 0; i < points.Count - 1; i++)
            {
                var segStart = points[i];
                var segEnd = points[i + 1];

                if (segEnd <= segStart)
                    continue;

                var covering = dayIntervals.FirstOrDefault(a => a.StartDay <= segStart && a.EndDay >= segEnd);
                if (covering is null)
                    throw new InvalidOperationException("Requested dates are not fully covered by availabilities.");

                var nights = segEnd.DayNumber - segStart.DayNumber;
                if (nights <= 0)
                    continue;

                var perNight = d.PriceType == PriceType.PerGuest
                    ? covering.Price * guests
                    : covering.Price;

                total += perNight * nights;
            }

            return total;
        }

        private static SearchResultItem MapToResult(
            AccommodationDocument d,
            int guests,
            DateTimeOffset start,
            DateTimeOffset end)
        {
            var (startDay, endDay) = NormalizeToNightsUtc(start, end);

            var total = CalculateTotalPriceFromDoc(d, guests, startDay, endDay);

            return new SearchResultItem(
                AccommodationId: d.Id,
                Name: d.Name,
                MinGuests: d.MinGuests,
                MaxGuests: d.MaxGuests,
                PriceType: d.PriceType,
                City: d.Location?.City,
                Country: d.Location?.Country,
                TotalPrice: total
            );
        }
    }
}
