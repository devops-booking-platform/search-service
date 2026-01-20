using FluentAssertions;
using MongoDB.Driver;
using Moq;
using SearchService.Documents;
using SearchService.DTO;

namespace SearchService.Tests.Services
{
    public class SearchServiceTests
    {
        [Fact]
        public async Task SearchAsync_WithInvalidPage_ThrowsArgumentOutOfRangeException()
        {
            var mockCollection = new Mock<IMongoCollection<AccommodationDocument>>();
            var sut = new SearchService.Services.SearchService(mockCollection.Object);
            var request = new SearchRequest { Page = 0, PageSize = 10, Guests = 2, Start = new DateOnly(2024, 1, 1), End = new DateOnly(2024, 1, 5) };

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.SearchAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task SearchAsync_WithZeroGuests_ThrowsArgumentOutOfRangeException()
        {
            var mockCollection = new Mock<IMongoCollection<AccommodationDocument>>();
            var sut = new SearchService.Services.SearchService(mockCollection.Object);
            var request = new SearchRequest { Page = 1, PageSize = 10, Guests = 0, Start = new DateOnly(2024, 1, 1), End = new DateOnly(2024, 1, 5) };

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.SearchAsync(request, CancellationToken.None));
            exception.Message.Should().Contain("Number of guests can not be 0");
        }

        [Fact]
        public async Task SearchAsync_WithEndDateBeforeStartDate_ThrowsArgumentOutOfRangeException()
        {
            var mockCollection = new Mock<IMongoCollection<AccommodationDocument>>();
            var sut = new SearchService.Services.SearchService(mockCollection.Object);
            var request = new SearchRequest { Page = 1, PageSize = 10, Guests = 2, Start = new DateOnly(2024, 1, 10), End = new DateOnly(2024, 1, 5) };

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.SearchAsync(request, CancellationToken.None));
            exception.Message.Should().Contain("End date must be after start");
        }
    }
}
