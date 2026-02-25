using Books.Api.Models;

namespace Books.Api.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetAsync();
        Task<Book> GetAsync(string id);
        Task CreateAsync(Book movie);
        Task UpdateAsync(string id, Book movie);
        Task RemoveAsync(string id);
        Task ClearCache();
    }
}
