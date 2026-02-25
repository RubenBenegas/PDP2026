using MicroservicesDemoFE.Api.Models;
using MicroservicesDemoFE.Api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesDemoFE.Api.Controllers
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var movies = await _movieService.GetAllAsync();

            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var movie = await _movieService.GetAsync(id);
            if (movie == null) return NotFound();
            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Movie movie)
        {
            var response = await _movieService.CreateAsync(movie);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Movie movie)
        {
            var response = await _movieService.UpdateAsync(id, movie);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _movieService.DeleteAsync(id);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpDelete("clear-cache")]
        public IActionResult ClearCache()
        {
            _movieService.ClearCache();
            return Ok("Cache limpiado");
        }
    }
}