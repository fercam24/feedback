using System;
using Xunit;
using Moq;
using MongoDB.Driver;
using dotNetTips.Utility.Standard.Tester;
using GameSessionFeedback.Models;
using GameSessionFeedback.DbContexts;
using System.Reflection;

namespace GameSessionFeedback.UnitTests.DbContexts
{
    public class MongoDbContextTest
    {
        private Mock<IFeedbackDatabaseSettings> _mockDatabaseSettings;
        private Mock<IGameSessionFeedbackProperties> _mockAppProperties;
        private Mock<IMongoDatabase> _mockDatabase;
        private Mock<IMongoClient> _mockClient;

        public MongoDbContextTest() {
            _mockDatabaseSettings = new Mock<IFeedbackDatabaseSettings>();
            _mockAppProperties = new Mock<IGameSessionFeedbackProperties>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();
        }

        [Fact]
        public void Success_MongoDbContext_Construct()
        {
            //Arrange
            string connectionString = "mongodb://" + RandomData.GenerateWord(20);
            string sessionFeedbacksCollectionName = RandomData.GenerateWord(10);
            string gameKey = RandomData.GenerateWord(10);
            string serviceName = RandomData.GenerateWord(10);

            //Act
            var dbContext = getDbContext(connectionString, sessionFeedbacksCollectionName, gameKey, serviceName);
        
            //Assert
            Assert.NotNull(dbContext);
        }

        [Fact]
        public void Success_MongoDbContext_GetCollection()
        {
            //Arrange
            string connectionString = "mongodb://" + RandomData.GenerateWord(20);
            string sessionFeedbacksCollectionName = RandomData.GenerateWord(10);
            string gameKey = RandomData.GenerateWord(10);
            string serviceName = RandomData.GenerateWord(10);

            var dbContext = getDbContext(connectionString, sessionFeedbacksCollectionName, gameKey, serviceName);
        
            //Act
            var sessionFeedbacks = dbContext.GetCollection<SessionFeedback>(sessionFeedbacksCollectionName);
        
            //Assert
            Assert.NotNull(sessionFeedbacks);
        }

        [Fact]
        public void Fail_MongoDbContext_GetCollection_EmptyCollectionName()
        {
            //Arrange
            string connectionString = "mongodb://" + RandomData.GenerateWord(20);
            string sessionFeedbacksCollectionName = RandomData.GenerateWord(10);
            string gameKey = RandomData.GenerateWord(10);
            string serviceName = RandomData.GenerateWord(10);

            var dbContext = getDbContext(connectionString, sessionFeedbacksCollectionName, gameKey, serviceName);
        
            //Act
            Action getCollection = () => dbContext.GetCollection<SessionFeedback>("");
        
            //Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(getCollection);
        }


        private IMongoDbContext getDbContext(string connectionString, string sessionFeedbacksCollectionName, string gameKey, string serviceName) {
            
            _mockDatabaseSettings.Setup(s => s.ConnectionString).Returns(connectionString);
            _mockDatabaseSettings.Setup(s => s.SessionFeedbacksCollectionName).Returns(sessionFeedbacksCollectionName);

            _mockAppProperties.Setup(p => p.GameKey).Returns(gameKey);
            _mockAppProperties.Setup(p => p.ServiceName).Returns(serviceName);

            return new MongoDbContext(_mockDatabaseSettings.Object, _mockAppProperties.Object);
        }
    }
}