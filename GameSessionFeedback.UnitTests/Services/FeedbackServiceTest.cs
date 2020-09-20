using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using dotNetTips.Utility.Standard.Tester;
using GameSessionFeedback.DbContexts;
using GameSessionFeedback.Models;
using GameSessionFeedback.Services;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace GameSessionFeedback.UnitTests.Services
{
    public class FeedbackServiceTest
    {
        private Mock<IMongoCollection<SessionFeedback>> _mockCollection;
        private Mock<IMongoDbContext> _mockDbContext;
        private Mock<IFeedbackDatabaseSettings> _mockFeedbackDbSettings;

        private List<SessionFeedback> _sessionFeedbacks;

        public FeedbackServiceTest()
        {
            _mockCollection = new Mock<IMongoCollection<SessionFeedback>>();
            _mockDbContext = new Mock<IMongoDbContext>();
            _mockFeedbackDbSettings = new Mock<IFeedbackDatabaseSettings>();
            _sessionFeedbacks = new List<SessionFeedback>()
            {
                new SessionFeedback()
                {
                    Id = "SessionFeedback1",
                    Comment = RandomData.GenerateWord(100),
                    UserId = RandomData.GenerateWord(10),
                    SessionId = RandomData.GenerateWord(10),
                    Rate = 5
                },
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
            
            _mockCollection.Object.InsertMany(_sessionFeedbacks);
        }

        [Fact]
        public async Task Success_FeedbackService_GetSessionFeedbacksAsync()
        {
            //Arrange
            _mockDbContext.Setup(context => context.GetCollection<SessionFeedback>(It.IsAny<String>())).Returns(_mockCollection.Object);
            _mockFeedbackDbSettings.Setup(settings => settings.SessionFeedbacksCollectionName)
                .Returns(RandomData.GenerateWord(10));
            
            Mock<IAsyncCursor<SessionFeedback>> sessionFeedbackCursor = new Mock<IAsyncCursor<SessionFeedback>>();
            sessionFeedbackCursor.Setup(cursor => cursor.Current).Returns(_sessionFeedbacks);
            sessionFeedbackCursor.SetupSequence(cursor => cursor.MoveNextAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(true)).Returns(Task.FromResult(false));

            _mockCollection.Setup(collection => collection.FindAsync(It.IsAny<FilterDefinition<SessionFeedback>>(), It.IsAny<FindOptions<SessionFeedback>>(), default)).ReturnsAsync(sessionFeedbackCursor.Object);

            IFeedbackService feedbackService = new FeedbackService(_mockDbContext.Object, _mockFeedbackDbSettings.Object);
            
            //Act
            var feedbacks = await feedbackService.GetSessionFeedbacksAsync(2);
            
            //Assert
            Assert.NotNull(feedbacks);
            Assert.Collection(feedbacks, 
                feedback => Assert.Contains("SessionFeedback2", feedback.Id), 
                feedback => Assert.Contains("SessionFeedback3", feedback.Id));
            
        }

    }
}