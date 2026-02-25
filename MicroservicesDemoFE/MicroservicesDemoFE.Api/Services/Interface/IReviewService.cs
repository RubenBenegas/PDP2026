using MicroservicesDemoFE.Api.Models;

namespace MicroservicesDemoFE.Api.Services.Interface
{
    public interface IReviewService
    {
        Task<List<Review>> GetAllAsync();
        Task<Review?> GetAsync(string id);
        Task<HttpResponseMessage> CreateAsync(Review review);
        Task<HttpResponseMessage> UpdateAsync(string id, Review review);
        Task<HttpResponseMessage> DeleteAsync(string id);
        void ClearCache();
    }
}
