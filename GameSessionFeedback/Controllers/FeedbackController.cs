using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GameSessionFeedback.Models;
using GameSessionFeedback.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace GameSessionFeedback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private const string USERID_HEADER_KEY = "Ubi-UserId";

        private readonly IFeedbackService _feedbackService;

        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(ILogger<FeedbackController> logger, IFeedbackService feedbackService)
        {
            _logger = logger;
            _feedbackService = feedbackService;
        }

        [HttpGet]
        [HttpGet("{rating}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<SessionFeedback>>> GetFeedbacks(short? rating)
        {
            if (rating != null && (rating < 1 || rating > 5))
            {
                return BadRequest();
            }

            var sessionFeedback = await _feedbackService.GetSessionFeedbacksAsync(rating);
            return Ok(sessionFeedback);
        }

        [HttpPost("{sessionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SessionFeedback>> CreateSessionFeedback(string sessionId, [Required, FromBody] SessionFeedback sessionFeedback)
        {
            // Check that header userid is present.. Can be replaced by Middleware
            if (!Request.Headers.ContainsKey(USERID_HEADER_KEY))
            {
                _logger.LogWarning(USERID_HEADER_KEY + " must not be null");
                return BadRequest();
            }

            Request.Headers.TryGetValue(USERID_HEADER_KEY, out var userId);
            if (String.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning(USERID_HEADER_KEY + " must not be empty");
                return BadRequest();
            }
            sessionFeedback.UserId = userId;
            sessionFeedback.SessionId = sessionId;
            sessionFeedback.CreatedOn = DateTime.Now;

            try
            {
                await _feedbackService.CreateFeedbackAsync(sessionFeedback);
            }
            catch (MongoWriteException e)
            {
                if (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    return Conflict();
                }
            }
            
            return StatusCode(201);
        }
    }
}