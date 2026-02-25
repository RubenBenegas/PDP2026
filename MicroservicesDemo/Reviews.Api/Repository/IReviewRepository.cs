using Reviews.Api.Models;

namespace Reviews.Api.Repository
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(string id);
        Task CreateAsync(Review review);
        Task UpdateAsync(string id, Review review);
        Task DeleteAsync(string id);
    }
}