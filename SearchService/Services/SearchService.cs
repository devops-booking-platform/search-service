using MongoDB.Bson;
using MongoDB.Driver;
using SearchService.Documents;
using SearchService.DTO;
using SearchService.Enums;
using SearchService.Services.Interfaces;
using System.Text.RegularExpressions;

namespace SearchService.Services
{
    public sealed class SearchService(IMongoCollection<AccommodationDocument> collection) : ISearchService
    {
        public async Task<PagedResult<SearchResultItem>> SearchAsync(
            SearchRequest request,
            CancellationToken ct)
        {
            if (request.Page <= 0) throw new ArgumentOutOfRangeException(nameof(request.Page));

            if (request.PageSize <= 0) throw new ArgumentOutOfRangeException(nameof(request.PageSize));

            if (request.Guests <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.Guests), "Number of guests can not be 0.");

            if (request.Start >= request.End)
                throw new ArgumentOutOfRangeException(nameof(request.End), "End date must be after start.");

            var filter = BuildFilter(request.City, request.Country, request.Guests, request.Start, request.End);

            var sortDocs = Builders<AccommodationDocument>.Sort.Ascending(x => x.Name).Ascending(x => x.Id);
            var docs = await collection.Find(filter)
                .Sort(sortDocs)
                .ToListAsync(ct);

            var results = new List<SearchResultItem>(docs.Count);
            foreach (var d in docs)
            {
                try
                {
                    results.Add(MapToResult(d, request.Guests, request.Start, request.End));
                }
                catch (InvalidOperationException)
                {
                }
            }

            var totalCount = results.Count;

            var skip = (request.Page - 1) * request.PageSize;

            var pageItems = results
                .Skip(skip)
                .Take(request.PageSize)
                .ToList();

            return new PagedResult<SearchResultItem>
            {
                Items = pageItems,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        private static FilterDefinition<AccommodationDocument> BuildFilter(
            string? city,
            string? country,
            int guests,
            DateOnly start,
            DateOnly end)
        {
            var fb = Builders<AccommodationDocument>.Filter;
            var filter = fb.Empty;

            if (!string.IsNullOrWhiteSpace(city))
            {
                var trimmedCity = city.Trim();
                filter &= fb.Regex("Location.City",
                    new BsonRegularExpression($"^{Regex.Escape(trimmedCity)}$", "i"));
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                var trimmedCountry = country.Trim();
                filter &= fb.Regex("Location.Country",
                    new BsonRegularExpression($"^{Regex.Escape(trimmedCountry)}$", "i"));
            }

            filter &= fb.Lte(x => x.MinGuests, guests) & fb.Gte(x => x.MaxGuests, guests);

            filter &= fb.ElemMatch(x => x.Availabilities, a => a.EndDate > start && a.StartDate < end);

            filter &= fb.Not(fb.ElemMatch(
                x => x.Reservations,
                r => start < r.EndDate && end > r.StartDate));

            return filter;
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
                    StartDay = a.StartDate,
                    EndDay = a.EndDate,
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
            AccommodationDocument accommodation,
            int guests,
            DateOnly start,
            DateOnly end)
        {
            var total = CalculateTotalPriceFromDoc(accommodation, guests, start, end);

            var nights = end.DayNumber - start.DayNumber;
            if (nights <= 0)
                throw new InvalidOperationException("Invalid date range.");

            var pricePerUnitPerNight = total / nights;
            var pricePerPersonPerNight = pricePerUnitPerNight / guests;
            return new SearchResultItem(
                AccommodationId: accommodation.Id,
                Name: accommodation.Name,
                MinGuests: accommodation.MinGuests,
                MaxGuests: accommodation.MaxGuests,
                PriceType: accommodation.PriceType,
                City: accommodation.Location?.City,
                Country: accommodation.Location?.Country,
                TotalPrice: total,
                Price: accommodation.PriceType == PriceType.PerUnit
                    ? pricePerUnitPerNight
                    : pricePerPersonPerNight
            );
        }
    }
}