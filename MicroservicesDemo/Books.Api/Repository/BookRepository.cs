using Books.Api.Models;
using MongoDB.Driver;

namespace Books.Api.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly IMongoCollection<Book> _booksCollection;

        public BookRepository(IMongoCollection<Book> bookCollection)
        {
            _booksCollection = bookCollection;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            await Task.Delay(5000);

            return await _booksCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(string id)
        {
            await Task.Delay(2000);

            return await _booksCollection
                .Find(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Book book)
        {
            await _booksCollection.InsertOneAsync(book);
        }

        public async Task UpdateAsync(string id, Book book)
        {
            await _booksCollection.ReplaceOneAsync(m => m.Id == id, book);
        }

        public async Task DeleteAsync(string id)
        {
            await _booksCollection.DeleteOneAsync(m => m.Id == id);
        }
    }
}
