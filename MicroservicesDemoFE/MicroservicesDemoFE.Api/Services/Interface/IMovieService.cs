using MicroservicesDemoFE.Api.Models;

namespace MicroservicesDemoFE.Api.Services.Interface
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAllAsync();
        Task<Movie?> GetAsync(string id);
        Task<HttpResponseMessage> CreateAsync(Movie movie);
        Task<HttpResponseMessage> UpdateAsync(string id, Movie movie);
        Task<HttpResponseMessage> DeleteAsync(string id);
        void ClearCache();
    }
}
