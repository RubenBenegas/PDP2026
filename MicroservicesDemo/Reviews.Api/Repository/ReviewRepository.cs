using MongoDB.Driver;
using Reviews.Api.Models;

namespace Reviews.Api.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IMongoCollection<Review> _reviewCollection;

        public ReviewRepository(IMongoCollection<Review> reviewCollection)
        {
            _reviewCollection = reviewCollection;
        }

        public async Task<List<Review>> GetAllAsync()
        {
            await Task.Delay(5000);

            return await _reviewCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(string id)
        {
            await Task.Delay(2000);

            return await _reviewCollection
                .Find(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Review review)
        {
            await _reviewCollection.InsertOneAsync(review);
        }

        public async Task UpdateAsync(string id, Review review)
        {
            await _reviewCollection.ReplaceOneAsync(m => m.Id == id, review);
        }

        public async Task DeleteAsync(string id)
        {
            await _reviewCollection.DeleteOneAsync(m => m.Id == id);
        }
    }
}
