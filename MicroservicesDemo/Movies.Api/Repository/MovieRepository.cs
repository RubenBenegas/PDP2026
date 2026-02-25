using MongoDB.Driver;
using Movies.Api.Models;

namespace Movies.Api.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IMongoCollection<Movie> _moviesCollection;

        public MovieRepository(IMongoCollection<Movie> moviesCollection)
        {
            _moviesCollection = moviesCollection;
        }

        public async Task<List<Movie>> GetAllAsync()
        {
            await Task.Delay(5000);

            return await _moviesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Movie?> GetByIdAsync(string id)
        {
            await Task.Delay(2000);

            return await _moviesCollection
                .Find(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Movie movie)
        {
            await _moviesCollection.InsertOneAsync(movie);
        }

        public async Task UpdateAsync(string id, Movie movie)
        {
            await _moviesCollection.ReplaceOneAsync(m => m.Id == id, movie);
        }

        public async Task DeleteAsync(string id)
        {
            await _moviesCollection.DeleteOneAsync(m => m.Id == id);
        }
    }
}
