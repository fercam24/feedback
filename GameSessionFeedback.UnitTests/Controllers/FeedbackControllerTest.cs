using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using dotNetTips.Utility.Standard.Tester;
using GameSessionFeedback.Controllers;
using GameSessionFeedback.DbContexts;
using GameSessionFeedback.Models;
using GameSessionFeedback.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
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
        
        [Fact]
        public async Task Success_FeedbackController_CreateSessionFeedback_Created()
        {
            //Arrange
            var sessionId = "someSessionId";
            var userId = "someUserId";
            var feedback = new SessionFeedback()
            {
                Comment = "",
                Id = "",
                Rate = 5,
                SessionId = sessionId,
                UserId = userId
            };
            _mockFeedbackService.Setup(service => service.CreateFeedbackAsync(It.IsAny<SessionFeedback>()))
                .ReturnsAsync(feedback);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Ubi-UserId"] = "1234566";
            
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            //Act
            var feedbackController = new FeedbackController(_mockLogger.Object, _mockFeedbackService.Object)
            {
                ControllerContext = controllerContext
            };
            var action = await feedbackController.CreateSessionFeedback(sessionId, new SessionFeedback(){Comment = "", Id = "", Rate = 4});
            var result = action.Result as StatusCodeResult;

            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, result.StatusCode);
            _mockFeedbackService.Verify(service => service.CreateFeedbackAsync(It.IsAny<SessionFeedback>()), Times.Once);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Fail_FeedbackController_CreateSessionFeedback_BadRequest_When_Wrong_Header(string headerValue)
        {
            //Arrange
            var sessionId = "someSessionId";
            var userId = "someUserId";
            var feedback = new SessionFeedback()
            {
                Comment = "",
                Id = "",
                Rate = 5,
                SessionId = sessionId,
                UserId = userId
            };
            _mockFeedbackService.Setup(service => service.CreateFeedbackAsync(It.IsAny<SessionFeedback>()))
                .ReturnsAsync(feedback);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Ubi-UserId"] = headerValue;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            //Act
            var feedbackController = new FeedbackController(_mockLogger.Object, _mockFeedbackService.Object)
            {
                ControllerContext = controllerContext
            };
            var action = await feedbackController.CreateSessionFeedback(sessionId, new SessionFeedback(){Comment = "", Id = "", Rate = 4});
            var result = action.Result as BadRequestResult;

            Assert.IsType<BadRequestResult>(result);
            _mockFeedbackService.Verify(service => service.CreateFeedbackAsync(It.IsAny<SessionFeedback>()), Times.Never);
        }
        
        [Fact]
        public async Task Fail_FeedbackController_CreateSessionFeedback_BadRequest_When_Feedback_Allready_Exists()
        {
            //Arrange
            var sessionId = "someSessionId";
            var userId = "someUserId";
            var feedback = new SessionFeedback()
            {
                Comment = "",
                Id = "",
                Rate = 5,
                SessionId = sessionId,
                UserId = userId
            };
            _mockFeedbackService.Setup(service => service.CreateFeedbackAsync(It.IsAny<SessionFeedback>()))
                .ThrowsAsync(CreateDuplicateMongoWriteException());
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Ubi-UserId"] = "1234566";
            
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            //Act
            var feedbackController = new FeedbackController(_mockLogger.Object, _mockFeedbackService.Object)
            {
                ControllerContext = controllerContext
            };
            var action = await feedbackController.CreateSessionFeedback(sessionId, new SessionFeedback(){Comment = "", Id = "", Rate = 4});
            var result = action.Result as ConflictResult;

            Assert.IsType<ConflictResult>(result);
            _mockFeedbackService.Verify(service => service.CreateFeedbackAsync(It.IsAny<SessionFeedback>()), Times.Once);
        }

        private MongoWriteException CreateDuplicateMongoWriteException()
        {
            var connectionId = new ConnectionId(new ServerId(new ClusterId(1), new DnsEndPoint("localhost", 27017)), 2);
            var ctor = typeof (WriteConcernError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeConcern = (WriteConcernError)ctor.Invoke(new object[] { 1, "writeConcernError", "writeConcernErrorMessage", new BsonDocument("details", "writeConcernError") });
            ctor = typeof (WriteError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeError = (WriteError)ctor.Invoke(new object[] {ServerErrorCategory.DuplicateKey, 1, "writeError", new BsonDocument("details", "writeError")});
            return new MongoWriteException(connectionId, writeError, writeConcern, new Exception());
        }
    }
}