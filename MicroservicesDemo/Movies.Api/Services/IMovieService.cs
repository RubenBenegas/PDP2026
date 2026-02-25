using Movies.Api.Models;

namespace Movies.Api.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAsync();
        Task<Movie> GetAsync(string id);
        Task CreateAsync(Movie movie);
        Task UpdateAsync(string id, Movie movie);
        Task RemoveAsync(string id);
        Task ClearCache();
    }
}
