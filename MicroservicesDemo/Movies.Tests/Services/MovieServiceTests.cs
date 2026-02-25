using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using Moq;
using Movies.Api.CustomExceptions;
using Movies.Api.Models;
using Movies.Api.Repository;
using Movies.Api.Services;
using System.Text;
using System.Text.Json;

//dotnet test MicroservicesDemo.sln --collect:"XPlat Code Coverage"
//reportgenerator -reports:coverage.cobertura.xml -targetdir:coveragereport 
namespace Movies.Tests.Services
{
    public class MovieServiceTests
    {

        [Fact]
        public async Task GetAsync_ShouldReturnMovies_WhenCacheIsEmpty()
        {
            // Arrange
            var movies = new List<Movie>
            {
                new Movie { Id = "1", Title = "Matrix" }
            };

            var repositoryMock = new Mock<IMovieRepository>();
            var cacheMock = new Mock<IDistributedCache>();

            cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(movies);

            var service = new MovieService(repositoryMock.Object, cacheMock.Object);

            // Act
            var result = await service.GetAsync();

            // Assert
            Assert.Equal(1, result.Count);
            result.Should().HaveCount(1); //FluentAssertions
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnMovies_FromCache_WhenCacheExists()
        {
            // Arrange
            var movies = new List<Movie>
            {
                new Movie { Id = "1", Title = "Matrix" }
            };

            var serialized = JsonSerializer.Serialize(movies);
            var bytes = Encoding.UTF8.GetBytes(serialized);

            var repositoryMock = new Mock<IMovieRepository>();
            var cacheMock = new Mock<IDistributedCache>();

            cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bytes);

            var service = new MovieService(repositoryMock.Object, cacheMock.Object);

            // Act
            var result = await service.GetAsync();

            // Assert
            Assert.Equal(1, result.Count);
            result.Should().HaveCount(1); //FluentAssertions
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAsync_ShouldThrowBusinessException_WhenReviewNotFound()
        {
            var repositoryMock = new Mock<IMovieRepository>();
            var cacheMock = new Mock<IDistributedCache>();

            cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Movie?)null);

            var service = new MovieService(repositoryMock.Object, cacheMock.Object);

            await Assert.ThrowsAsync<BusinessException>(() => service.GetAsync("99"));
        }
    }
}