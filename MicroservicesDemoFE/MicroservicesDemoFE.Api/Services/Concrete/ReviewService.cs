using MicroservicesDemoFE.Api.Models;
using MicroservicesDemoFE.Api.Services.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace MicroservicesDemoFE.Api.Services.Concrete
{
    public class ReviewService : IReviewService
    {
        private readonly IMemoryCache _cache;

        private const string ReviewsCacheKey = "reviews_all";
        private const string ReviewByIdCacheKey = "review_{0}";

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _gatewayUrl = "http://localhost:5000/reviews";

        public ReviewService(HttpClient httpClient, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
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

        public async Task<List<Review>> GetAllAsync()
        {
            if (!_cache.TryGetValue(ReviewsCacheKey, out List<Review> reviews))
            {
                reviews = await _httpClient.GetFromJsonAsync<List<Review>>(_gatewayUrl) ?? new List<Review>();

                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                _cache.Set(ReviewsCacheKey, reviews, options);
            }

            return reviews;
        }

        public async Task<Review?> GetAsync(string id)
        {
            string cacheKey = string.Format(ReviewByIdCacheKey, id);

            if (!_cache.TryGetValue(cacheKey, out Review review))
            {
                review = await _httpClient.GetFromJsonAsync<Review>($"{_gatewayUrl}/{id}");

                if (review != null)
                {
                    var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                    _cache.Set(cacheKey, review, options);
                }
            }

            return review;
        }

        public async Task<HttpResponseMessage> CreateAsync(Review review)
        {
            var result = await _httpClient.PostAsJsonAsync(_gatewayUrl, review);

            _cache.Remove(ReviewsCacheKey);
            _cache.Remove(string.Format(ReviewByIdCacheKey, review.Id));

            return result;
        }

        public async Task<HttpResponseMessage> UpdateAsync(string id, Review review)
        {
            var result = await _httpClient.PutAsJsonAsync($"{_gatewayUrl}/{id}", review);

            _cache.Remove(ReviewsCacheKey);
            _cache.Remove(string.Format(ReviewByIdCacheKey, id));

            return result;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string id)
        {
            var result = await _httpClient.DeleteAsync($"{_gatewayUrl}/{id}");

            _cache.Remove(ReviewsCacheKey);
            _cache.Remove(string.Format(ReviewByIdCacheKey, id));

            return result;
        }

        public void ClearCache()
        {
            _cache.Remove(ReviewsCacheKey);
        }
    }
}

