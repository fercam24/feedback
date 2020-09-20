using System;
using dotNetTips.Utility.Standard.Tester;
using GameSessionFeedback.DbContexts;
using GameSessionFeedback.Models;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace GameSessionFeedback.UnitTests.DbContexts
{
    public class MongoDbContextTest
    {
        private readonly Mock<IGameSessionFeedbackProperties> _mockAppProperties;
        private readonly Mock<IFeedbackDatabaseSettings> _mockDatabaseSettings;

        public MongoDbContextTest()
        {
            _mockDatabaseSettings = new Mock<IFeedbackDatabaseSettings>();
            _mockAppProperties = new Mock<IGameSessionFeedbackProperties>();
        }

        [Fact]
        public void Success_MongoDbContext_Construct()
        {
            //Arrange
            var sessionFeedbacksCollectionName = RandomData.GenerateWord(10);

            //Act
            var dbContext = GetDbContext(sessionFeedbacksCollectionName);

            //Assert
            Assert.NotNull(dbContext);
        }

        [Fact]
        public void Success_MongoDbContext_GetCollection()
        {
            //Arrange
            var sessionFeedbacksCollectionName = RandomData.GenerateWord(10);
            var dbContext = GetDbContext(sessionFeedbacksCollectionName);

            //Act
            var sessionFeedbacks = dbContext.GetCollection<SessionFeedback>(sessionFeedbacksCollectionName);

            //Assert
            Assert.NotNull(sessionFeedbacks);
        }

        [Fact]
        public void Fail_MongoDbContext_GetCollection_WhitespacesCollectionName()
        {
            //Arrange
            var sessionFeedbacksCollectionName = RandomData.GenerateWord(10);
            var dbContext = GetDbContext(sessionFeedbacksCollectionName);

            //Act
            var getCollection = dbContext.GetCollection<SessionFeedback>("  ");

            //Assert
            Assert.Null(getCollection);
        }
        
        [Fact]
        public void Fail_MongoDbContext_GetCollection_NullCollectionName()
        {
            //Arrange
            var sessionFeedbacksCollectionName = RandomData.GenerateWord(10);
            var dbContext = GetDbContext(sessionFeedbacksCollectionName);

            //Act
            var getCollection = dbContext.GetCollection<SessionFeedback>(null);

            //Assert
            Assert.Null(getCollection);
        }
        
        [Fact]
        public void Fail_MongoDbContext_GetCollection_EmptyCollectionName()
        {
            //Arrange
            var sessionFeedbacksCollectionName = RandomData.GenerateWord(10);
            var dbContext = GetDbContext(sessionFeedbacksCollectionName);

            //Act
            var getCollection = dbContext.GetCollection<SessionFeedback>(string.Empty);

            //Assert
            Assert.Null(getCollection);
        }

        private IMongoDbContext GetDbContext(string sessionFeedbacksCollectionName)
        {
            var connectionString = "mongodb://" + RandomData.GenerateWord(20);
            var gameKey = RandomData.GenerateWord(10);
            var serviceName = RandomData.GenerateWord(10);

            _mockDatabaseSettings.Setup(settings => settings.ConnectionString).Returns(connectionString);
            _mockDatabaseSettings.Setup(settings => settings.SessionFeedbacksCollectionName)
                .Returns(sessionFeedbacksCollectionName);

            _mockAppProperties.Setup(properties => properties.GameKey).Returns(gameKey);
            _mockAppProperties.Setup(properties => properties.ServiceName).Returns(serviceName);

            return new MongoDbContext(_mockDatabaseSettings.Object, _mockAppProperties.Object);
        }
    }
}