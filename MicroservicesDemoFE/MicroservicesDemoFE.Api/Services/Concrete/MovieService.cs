using MicroservicesDemoFE.Api.Models;
using MicroservicesDemoFE.Api.Services.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace MicroservicesDemoFE.Api.Services.Concrete
{
    public class MovieService : IMovieService
    {
        private readonly IMemoryCache _cache;

        private const string MoviesCacheKey = "movies_all";
        private const string MovieByIdCacheKey = "movie_{0}";

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string _gatewayUrl = "http://localhost:5000/movies";

        public MovieService(HttpClient httpClient, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
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

        public async Task<List<Movie>> GetAllAsync()
        {
            try
            {
                if (!_cache.TryGetValue(MoviesCacheKey, out List<Movie> movies))
                {
                    movies = await _httpClient.GetFromJsonAsync<List<Movie>>(_gatewayUrl) ?? new List<Movie>();

                    var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                    _cache.Set(MoviesCacheKey, movies, options);
                }

                return movies;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public async Task<Movie?> GetAsync(string id)
        {
            string cacheKey = string.Format(MovieByIdCacheKey, id);

            if (!_cache.TryGetValue(cacheKey, out Movie movie))
            {
                movie = await _httpClient.GetFromJsonAsync<Movie>($"{_gatewayUrl}/{id}");

                if (movie != null)
                {
                    var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                    _cache.Set(cacheKey, movie, options);
                }
            }

            return movie;
        }

        public async Task<HttpResponseMessage> CreateAsync(Movie movie)
        {
            var result =  await _httpClient.PostAsJsonAsync(_gatewayUrl, movie);

            _cache.Remove(MoviesCacheKey);
            _cache.Remove(string.Format(MovieByIdCacheKey, movie.Id));

            return result;
        }

        public async Task<HttpResponseMessage> UpdateAsync(string id, Movie movie)
        {
            var result = await _httpClient.PutAsJsonAsync($"{_gatewayUrl}/{id}", movie);

            _cache.Remove(MoviesCacheKey);
            _cache.Remove(string.Format(MovieByIdCacheKey, id));

            return result;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string id)
        {
            var result = await _httpClient.DeleteAsync($"{_gatewayUrl}/{id}");

            _cache.Remove(MoviesCacheKey);
            _cache.Remove(string.Format(MovieByIdCacheKey, id));

            return result;
        }

        public void ClearCache()
        {
            _cache.Remove(MoviesCacheKey);
        }
    }
}

