using Reviews.Api.Models;

namespace Reviews.Api.Services
{
    public interface IReviewService
    {
        Task<List<Review>> GetAsync();
        Task<Review> GetAsync(string id);
        Task CreateAsync(Review movie);
        Task UpdateAsync(string id, Review movie);
        Task RemoveAsync(string id);
        Task ClearCache();
    }
}
