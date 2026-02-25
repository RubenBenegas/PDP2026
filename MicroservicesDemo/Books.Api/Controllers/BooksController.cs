using Books.Api.Models;
using Books.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<List<Book>> Get()
        {
            return await _bookService.GetAsync();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(string id)
        {
            var book = await _bookService.GetAsync(id);
            if (book == null) return NotFound();
            return book;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create(Book book)
        {
            await _bookService.CreateAsync(book);
            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Book book)
        {
            var existingBook = await _bookService.GetAsync(id);
            if (existingBook == null) return NotFound();
            await _bookService.UpdateAsync(id, book);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingBook = await _bookService.GetAsync(id);
            if (existingBook == null) return NotFound();
            await _bookService.RemoveAsync(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpDelete("clear-cache")]
        public IActionResult ClearCache()
        {
            _bookService.ClearCache();
            return Ok("Cache limpiado");
        }
    }
}