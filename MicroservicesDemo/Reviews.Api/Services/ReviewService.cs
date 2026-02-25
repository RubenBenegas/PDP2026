using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using Reviews.Api.CustomExceptions;
using Reviews.Api.Models;
using Reviews.Api.Repository;
using System.Text;
using System.Text.Json;

namespace Reviews.Api.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repository;
        private readonly IDistributedCache _cache;

        private const string ReviewsCacheKey = "reviews_all";
        private const string ReviewByIdCacheKey = "review_{0}";

        public ReviewService(IReviewRepository repository,
                             IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<List<Review>> GetAsync()
        {
            var cachedBytes = await _cache.GetAsync(ReviewsCacheKey);

            if (cachedBytes != null)
            {
                var json = Encoding.UTF8.GetString(cachedBytes);
                return JsonSerializer.Deserialize<List<Review>>(json)!;
            }

            await Task.Delay(5000);

            var reviews = await _repository.GetAllAsync();

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            var serialized = JsonSerializer.Serialize(reviews);
            var bytes = Encoding.UTF8.GetBytes(serialized);

            await _cache.SetAsync(ReviewsCacheKey, bytes, options);

            return reviews;
        }

        public async Task<Review> GetAsync(string id)
        {
            string cacheKey = string.Format(ReviewByIdCacheKey, id);

            var cachedBytes = await _cache.GetAsync(cacheKey);

            if (cachedBytes != null)
            {
                var json = Encoding.UTF8.GetString(cachedBytes);
                return JsonSerializer.Deserialize<Review>(json)!;
            }

            var review = await _repository.GetByIdAsync(id);

            if (review == null)
                throw new BusinessException($"No se encontro la review con id {id}");

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            var serialized = JsonSerializer.Serialize(review);
            var bytes = Encoding.UTF8.GetBytes(serialized);

            await _cache.SetAsync(cacheKey, bytes, options);

            return review;
        }

        public async Task CreateAsync(Review review)
        {
            await _repository.CreateAsync(review);

            await _cache.RemoveAsync(ReviewsCacheKey);
            await _cache.RemoveAsync(string.Format(ReviewByIdCacheKey, review.Id));
        }

        public async Task UpdateAsync(string id, Review review)
        {
            await _repository.UpdateAsync(id, review);

            await _cache.RemoveAsync(ReviewsCacheKey);
            await _cache.RemoveAsync(string.Format(ReviewByIdCacheKey, id));
        }

        public async Task RemoveAsync(string id)
        {
            await _repository.DeleteAsync(id);

            await _cache.RemoveAsync(ReviewsCacheKey);
            await _cache.RemoveAsync(string.Format(ReviewByIdCacheKey, id));
        }

        public async Task ClearCache()
        {
            await _cache.RemoveAsync(ReviewsCacheKey);
        }
    }
}