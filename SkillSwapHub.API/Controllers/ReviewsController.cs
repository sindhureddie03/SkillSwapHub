using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwapHub.API.DTOs;
using SkillSwapHub.API.Models;
using SkillSwapHub.API.Repositories;
using System.Security.Claims;

namespace SkillSwapHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewRepository _reviewRepository;

        public ReviewsController(
            ReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(
            [FromBody] ReviewDto reviewDto)
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int reviewerId =
                int.Parse(userIdClaim.Value);

            if (reviewDto.Rating < 1 ||
                reviewDto.Rating > 5)
            {
                return BadRequest(new
                {
                    message = "Rating must be between 1 and 5."
                });
            }

            if (reviewDto.SessionId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid session ID."
                });
            }

            if (reviewDto.ReviewedUserId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid reviewed user ID."
                });
            }

            var review = new Review
            {
                SessionId = reviewDto.SessionId,
                ReviewerId = reviewerId,
                ReviewedUserId = reviewDto.ReviewedUserId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment
            };

            var reviewId =
                await _reviewRepository
                    .AddReviewAsync(review);

            return Ok(new
            {
                ReviewId = reviewId,
                Message = "Review submitted successfully."
            });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReviewsForUser(
    int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid user ID."
                });
            }

            var reviews =
                await _reviewRepository
                    .GetReviewsForUserAsync(userId);

            return Ok(reviews);
        }
    }


}