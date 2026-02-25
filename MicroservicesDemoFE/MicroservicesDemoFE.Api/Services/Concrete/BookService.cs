using MicroservicesDemoFE.Api.Models;
using MicroservicesDemoFE.Api.Services.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace MicroservicesDemoFE.Api.Services.Concrete
{
    public class BookService : IBookService
    {
        private readonly IMemoryCache _cache;

        private const string BooksCacheKey = "books_all";
        private const string BookByIdCacheKey = "book_{0}";

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string _gatewayUrl = "http://localhost:5000/books";

        public BookService(HttpClient httpClient, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Substring("Bearer ".Length).Trim());
            }
        }

        public async Task<List<Book>> GetAllAsync()
        {
            if (!_cache.TryGetValue(BooksCacheKey, out List<Book> books))
            {
                books = await _httpClient.GetFromJsonAsync<List<Book>>(_gatewayUrl) ?? new List<Book>();

                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                _cache.Set(BooksCacheKey, books, options);
            }

            return books;
        }

        public async Task<Book?> GetAsync(string id)
        {
            string cacheKey = string.Format(BookByIdCacheKey, id);

            if (!_cache.TryGetValue(cacheKey, out Book book))
            {
                book = await _httpClient.GetFromJsonAsync<Book>($"{_gatewayUrl}/{id}");

                if (book != null)
                {
                    var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                    _cache.Set(cacheKey, book, options);
                }
            }

            return book;
        }

        public async Task<HttpResponseMessage> CreateAsync(Book book)
        {
            var result = await _httpClient.PostAsJsonAsync(_gatewayUrl, book);

            _cache.Remove(BooksCacheKey);
            _cache.Remove(string.Format(BookByIdCacheKey, book.Id));

            return result;
        }

        public async Task<HttpResponseMessage> UpdateAsync(string id, Book book)
        {
            var result = await _httpClient.PutAsJsonAsync($"{_gatewayUrl}/{id}", book);

            _cache.Remove(BooksCacheKey);
            _cache.Remove(string.Format(BookByIdCacheKey, id));

            return result;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string id)
        {
            var result = await _httpClient.DeleteAsync($"{_gatewayUrl}/{id}");

            _cache.Remove(BooksCacheKey);
            _cache.Remove(string.Format(BookByIdCacheKey, id));

            return result;
        }

        public void ClearCache()
        {
            _cache.Remove(BooksCacheKey);
        }
    }
}

