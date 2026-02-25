using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using Movies.Api.CustomExceptions;
using Movies.Api.Models;
using Movies.Api.Repository;
using System.Text.Json;

namespace Movies.Api.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _repository;
        private readonly IDistributedCache _cache;

        private const string MoviesCacheKey = "movies_all";
        private const string MovieByIdCacheKey = "movie_{0}";

        public MovieService(IMovieRepository repository,
                            IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<List<Movie>> GetAsync()
        {
            var cached = await _cache.GetStringAsync(MoviesCacheKey);

            if (cached != null)
                return JsonSerializer.Deserialize<List<Movie>>(cached)!;

            var movies = await _repository.GetAllAsync();

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            await _cache.SetStringAsync(
                MoviesCacheKey,
                JsonSerializer.Serialize(movies),
                options);

            return movies;
        }

        public async Task<Movie> GetAsync(string id)
        {
            string cacheKey = string.Format(MovieByIdCacheKey, id);

            var cached = await _cache.GetStringAsync(cacheKey);

            if (cached != null)
                return JsonSerializer.Deserialize<Movie>(cached)!;

            var movie = await _repository.GetByIdAsync(id);

            if (movie == null)
                throw new BusinessException($"No se encontro la movie con id {id}");

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(movie),
                options);

            return movie;
        }

        public async Task CreateAsync(Movie movie)
        {
            await _repository.CreateAsync(movie);

            await _cache.RemoveAsync(MoviesCacheKey);
            await _cache.RemoveAsync(string.Format(MovieByIdCacheKey, movie.Id));
        }

        public async Task UpdateAsync(string id, Movie movie)
        {
            await _repository.UpdateAsync(id, movie);

            await _cache.RemoveAsync(MoviesCacheKey);
            await _cache.RemoveAsync(string.Format(MovieByIdCacheKey, id));
        }

        public async Task RemoveAsync(string id)
        {
            await _repository.DeleteAsync(id);

            await _cache.RemoveAsync(MoviesCacheKey);
            await _cache.RemoveAsync(string.Format(MovieByIdCacheKey, id));
        }

        public async Task ClearCache()
        {
            await _cache.RemoveAsync(MoviesCacheKey);
        }
    }
}