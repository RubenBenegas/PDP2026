using Books.Api.CustomExceptions;
using Books.Api.Models;
using Books.Api.Repository;
using Books.Api.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text;
using System.Text.Json;

namespace Books.Api.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;
        private readonly IDistributedCache _cache;

        private const string BooksCacheKey = "books_all";
        private const string BookByIdCacheKey = "book_{0}";

        public BookService(IBookRepository repository,
                           IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<List<Book>> GetAsync()
        {
            var cachedBytes = await _cache.GetAsync(BooksCacheKey);

            if (cachedBytes != null)
            {
                var json = Encoding.UTF8.GetString(cachedBytes);
                return JsonSerializer.Deserialize<List<Book>>(json)!;
            }

            var books = await _repository.GetAllAsync();

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            var serialized = JsonSerializer.Serialize(books);
            var bytes = Encoding.UTF8.GetBytes(serialized);

            await _cache.SetAsync(BooksCacheKey, bytes, options);

            return books;
        }

        public async Task<Book> GetAsync(string id)
        {
            string cacheKey = string.Format(BookByIdCacheKey, id);

            var cachedBytes = await _cache.GetAsync(cacheKey);

            if (cachedBytes != null)
            {
                var json = Encoding.UTF8.GetString(cachedBytes);
                return JsonSerializer.Deserialize<Book>(json)!;
            }

            var book = await _repository.GetByIdAsync(id);

            if (book == null)
                throw new BusinessException($"No se encontro el book con id {id}");

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            var serialized = JsonSerializer.Serialize(book);
            var bytes = Encoding.UTF8.GetBytes(serialized);

            await _cache.SetAsync(cacheKey, bytes, options);

            return book;
        }

        public async Task CreateAsync(Book book)
        {
            await _repository.CreateAsync(book);

            await _cache.RemoveAsync(BooksCacheKey);
            await _cache.RemoveAsync(string.Format(BookByIdCacheKey, book.Id));
        }

        public async Task UpdateAsync(string id, Book book)
        {
            await _repository.UpdateAsync(id, book);

            await _cache.RemoveAsync(BooksCacheKey);
            await _cache.RemoveAsync(string.Format(BookByIdCacheKey, id));
        }

        public async Task RemoveAsync(string id)
        {
            await _repository.DeleteAsync(id);

            await _cache.RemoveAsync(BooksCacheKey);
            await _cache.RemoveAsync(string.Format(BookByIdCacheKey, id));
        }

        public async Task ClearCache()
        {
            await _cache.RemoveAsync(BooksCacheKey);
        }
    }
}