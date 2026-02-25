using Movies.Api.Models;

namespace Movies.Api.Repository
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAllAsync();
        Task<Movie?> GetByIdAsync(string id);
        Task CreateAsync(Movie movie);
        Task UpdateAsync(string id, Movie movie);
        Task DeleteAsync(string id);
    }
}
