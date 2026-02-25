using Books.Api.CustomExceptions;
using Books.Api.Models;
using Books.Api.Repository;
using Books.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;
using System.Text.Json;

//dotnet test MicroservicesDemo.sln --collect:"XPlat Code Coverage"
//reportgenerator -reports:coverage.cobertura.xml -targetdir:coveragereport 
namespace Books.Tests
{
    public class BookServiceTests
    {

        [Fact]
        public async Task GetAsync_ShouldReturnBooks_WhenCacheIsEmpty()
        {
            var books = new List<Book>
            {
                new Book { Id = "1", Title = "Clean Code" }
            };

            var repositoryMock = new Mock<IBookRepository>();
            var cacheMock = new Mock<IDistributedCache>();

            cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(books);

            var service = new BookService(repositoryMock.Object, cacheMock.Object);

            var result = await service.GetAsync();

            result.Should().HaveCount(1);
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnBooks_FromCache_WhenCacheExists()
        {
            var books = new List<Book>
            {
                new Book { Id = "1", Title = "Clean Code" }
            };

            var serialized = JsonSerializer.Serialize(books);
            var bytes = Encoding.UTF8.GetBytes(serialized);

            var repositoryMock = new Mock<IBookRepository>();
            var cacheMock = new Mock<IDistributedCache>();

            cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bytes);

            var service = new BookService(repositoryMock.Object, cacheMock.Object);

            var result = await service.GetAsync();

            result.Should().HaveCount(1);
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAsync_ShouldThrowBusinessException_WhenBookNotFound()
        {
            var repositoryMock = new Mock<IBookRepository>();
            var cacheMock = new Mock<IDistributedCache>();

            cacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Book?)null);

            var service = new BookService(repositoryMock.Object, cacheMock.Object);

            await Assert.ThrowsAsync<BusinessException>(() => service.GetAsync("99"));
        }
    }
}