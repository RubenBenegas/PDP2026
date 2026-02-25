using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Reviews.Api.CustomExceptions;
using Reviews.Api.Models;
using Reviews.Api.Repository;
using Reviews.Api.Services;
using System.Text;
using System.Text.Json;

namespace Reviews.Tests
{
    //dotnet test MicroservicesDemo.sln --collect:"XPlat Code Coverage"
    //reportgenerator -reports:coverage.cobertura.xml -targetdir:coveragereport 
    public class ReviewServiceTests
    {

        [Fact]
        public async Task GetAsync_ShouldReturnReviews_WhenCacheIsEmpty()
        {
            var reviews = new List<Review>
            {
                new Review { Id = "1", Comment = "Excelente" }
            };

            var repositoryMock = new Mock<IReviewRepository>();
            var cacheMock = new Mock<IDistributedCache>();

            cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(reviews);

            var service = new ReviewService(repositoryMock.Object, cacheMock.Object);

            var result = await service.GetAsync();

            result.Should().HaveCount(1);
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnReviews_FromCache_WhenCacheExists()
        {
            var reviews = new List<Review>
            {
                new Review { Id = "1", Comment = "Excelente" }
            };

            var serialized = JsonSerializer.Serialize(reviews);
            var bytes = Encoding.UTF8.GetBytes(serialized);

            var repositoryMock = new Mock<IReviewRepository>();
            var cacheMock = new Mock<IDistributedCache>();

            cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bytes);

            var service = new ReviewService(repositoryMock.Object, cacheMock.Object);

            var result = await service.GetAsync();

            result.Should().HaveCount(1);
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAsync_ShouldThrowBusinessException_WhenReviewNotFound()
        {
            var repositoryMock = new Mock<IReviewRepository>();
            var cacheMock = new Mock<IDistributedCache>();

            cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Review?)null);

            var service = new ReviewService(repositoryMock.Object, cacheMock.Object);

            await Assert.ThrowsAsync<BusinessException>(() => service.GetAsync("99"));
        }
    }
}