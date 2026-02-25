using MicroservicesDemoFE.Api.Models;

namespace MicroservicesDemoFE.Api.Services.Interface
{
    public interface IBookService
    {
        Task<List<Book>> GetAllAsync();
        Task<Book?> GetAsync(string id);
        Task<HttpResponseMessage> CreateAsync(Book book);
        Task<HttpResponseMessage> UpdateAsync(string id, Book book);
        Task<HttpResponseMessage> DeleteAsync(string id);
        void ClearCache();
    }
}
