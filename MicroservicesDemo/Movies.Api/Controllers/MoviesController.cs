using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Models;
using Movies.Api.Services;

namespace Movies.Api.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<List<Movie>> Get()
        {
            return await _movieService.GetAsync();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> Get(string id)
        {
            var movie = await _movieService.GetAsync(id);
            if (movie == null) return NotFound();
            return movie;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create(Movie movie)
        {
            await _movieService.CreateAsync(movie);
            return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Movie movie)
        {
            var existingMovie = await _movieService.GetAsync(id);
            if (existingMovie == null) return NotFound();
            await _movieService.UpdateAsync(id, movie);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingMovie = await _movieService.GetAsync(id);
            if (existingMovie == null) return NotFound();
            await _movieService.RemoveAsync(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpDelete("clear-cache")]
        public IActionResult ClearCache()
        {
            _movieService.ClearCache();
            return Ok("Cache limpiado");
        }
    }
}