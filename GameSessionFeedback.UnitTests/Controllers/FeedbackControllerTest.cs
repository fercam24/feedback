using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using dotNetTips.Utility.Standard.Tester;
using GameSessionFeedback.Controllers;
using GameSessionFeedback.DbContexts;
using GameSessionFeedback.Models;
using GameSessionFeedback.Services;
using GameSessionFeedback.UnitTests.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace GameSessionFeedback.UnitTests.Controllers
{
    public class FeedbackControllerTest
    {
        private Mock<IMongoCollection<SessionFeedback>> _mockCollection;
        private Mock<IMongoDbContext> _mockDbContext;
        private Mock<IFeedbackDatabaseSettings> _mockFeedbackDbSettings;
        private Mock<IFeedbackService> _mockFeedbackService;
        private Mock<ILogger<FeedbackController>> _mockLogger;
        private List<SessionFeedback> _sessionFeedbacks;

        public FeedbackControllerTest()
        {
            _mockCollection = new Mock<IMongoCollection<SessionFeedback>>();
            _mockDbContext = new Mock<IMongoDbContext>();
            _mockFeedbackDbSettings = new Mock<IFeedbackDatabaseSettings>();
            _mockFeedbackService = new Mock<IFeedbackService>();
            _mockLogger = new Mock<ILogger<FeedbackController>>();
            _sessionFeedbacks = new List<SessionFeedback>()
            {
                new SessionFeedback()
                {
                    Id = "SessionFeedback2",
                    Comment = RandomData.GenerateWord(100),
                    UserId = RandomData.GenerateWord(10),
                    SessionId = RandomData.GenerateWord(10),
                    Rate = 2
                },
                new SessionFeedback()
                {
                    Id = "SessionFeedback3",
                    Comment = RandomData.GenerateWord(100),
                    UserId = RandomData.GenerateWord(10),
                    SessionId = RandomData.GenerateWord(10),
                    Rate = 2
                }
            };
        }

        [Fact]
        public async Task Success_FeedbackController_GetFeedbacks_OK_When_Without_Rating_Filtering()
        {
            //Arrange
            _mockFeedbackService.Setup(service => service.GetSessionFeedbacksAsync(null))
                .ReturnsAsync(_sessionFeedbacks);
            
            //Act
            var feedbackController = new FeedbackController(_mockLogger.Object, _mockFeedbackService.Object);
            var action = await feedbackController.GetFeedbacks(null);

            var result = (OkObjectResult) action.Result;
            var feedbacks = result.Value as List<SessionFeedback>;

            Assert.IsType<OkObjectResult>(result);
            _mockFeedbackService.Verify(service => service.GetSessionFeedbacksAsync(null), Times.Once);
            Assert.Equal(_sessionFeedbacks, feedbacks);
        }
        
        [Fact]
        public async Task Success_FeedbackController_GetFeedbacks_OK_When_With_Rating_Filtering()
        {
            //Arrange
            _mockFeedbackService.Setup(service => service.GetSessionFeedbacksAsync(It.IsAny<short>()))
                .ReturnsAsync(_sessionFeedbacks);
            
            //Act
            var feedbackController = new FeedbackController(_mockLogger.Object, _mockFeedbackService.Object);
            var action = await feedbackController.GetFeedbacks((short) new Random().Next(1,5));

            var result = action.Result as ObjectResult;
            var feedbacks = result.Value as List<SessionFeedback>;

            Assert.IsType<OkObjectResult>(result);
            _mockFeedbackService.Verify(service => service.GetSessionFeedbacksAsync(It.IsAny<short>()), Times.Once);
            Assert.Equal(_sessionFeedbacks, feedbacks);
        }
        
        [Fact]
        public async Task Success_FeedbackController_GetFeedbacks_BadRequest_When_Rating_Filtering_OutOfRange()
        {
            //Arrange
            _mockFeedbackService.Setup(service => service.GetSessionFeedbacksAsync(It.IsAny<short>()))
                .ReturnsAsync(_sessionFeedbacks);
            
            //Act
            var feedbackController = new FeedbackController(_mockLogger.Object, _mockFeedbackService.Object);
            var action = await feedbackController.GetFeedbacks(6);

            Assert.IsType<BadRequestResult>(action.Result);
            _mockFeedbackService.Verify(service => service.GetSessionFeedbacksAsync(It.IsAny<short>()), Times.Never);
        }
    }
}