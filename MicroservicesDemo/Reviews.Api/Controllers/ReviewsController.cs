using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reviews.Api.Models;
using Reviews.Api.Services;

namespace Reviews.Api.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<List<Review>> Get() 
        { 
           return await _reviewService.GetAsync();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> Get(string id)
        {
            var review = await _reviewService.GetAsync(id);
            if (review == null) return NotFound();
            return review;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Review>> Create(Review review)
        {
            await _reviewService.CreateAsync(review);
            return CreatedAtAction(nameof(Get), new { id = review.Id }, review);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Review review)
        {
            var existingReview = await _reviewService.GetAsync(id);
            if (existingReview == null) return NotFound();

            await _reviewService.UpdateAsync(id, review);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingReview = await _reviewService.GetAsync(id);
            if (existingReview == null) return NotFound();

            await _reviewService.RemoveAsync(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpDelete("clear-cache")]
        public IActionResult ClearCache()
        {
            _reviewService.ClearCache();
            return Ok("Cache limpiado");
        }
    }
}