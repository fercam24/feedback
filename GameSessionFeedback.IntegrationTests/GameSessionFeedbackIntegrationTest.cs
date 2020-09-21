using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GameSessionFeedback.DbContexts;
using GameSessionFeedback.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mongo2Go;
using MongoDB.Bson.IO;
using Xunit;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace GameSessionFeedback.IntegrationTests
{
    public class GameSessionFeedbackIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private const string ROOTENDPOINT = "api/feedback/"; 
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private readonly MongoDbRunner _runner;

        public GameSessionFeedbackIntegrationTest(WebApplicationFactory<Startup> factory)
        {
            _runner = MongoDbRunner.Start();
            FeedbackDatabaseSettings _databaseSettings = new FeedbackDatabaseSettings()
            {
                ConnectionString = _runner.ConnectionString,
                SessionFeedbacksCollectionName = "SessionFeedbacks"
            };

            GameSessionFeedbackProperties _feedbackProperties = new GameSessionFeedbackProperties()
            {
                GameKey = "IntegrationTest",
                ServiceName = "GameSessionFeedbackIntegration"
            };
            
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(
                    services =>
                    {
                        services.Configure<FeedbackDatabaseSettings>(settings =>
                        {
                            settings.ConnectionString = _databaseSettings.ConnectionString;
                            settings.SessionFeedbacksCollectionName = _databaseSettings.SessionFeedbacksCollectionName;
                        });
                        services.Configure<GameSessionFeedbackProperties>(properties =>
                        {
                            properties.GameKey = _feedbackProperties.GameKey;
                            properties.ServiceName = _feedbackProperties.ServiceName;
                        });
                    });
            });
            _client = _factory.CreateClient();
        }
        
        [Fact]
        public async Task Success_Get_Without_Filter_Should_Return_OK()
        {
            var response = await _client.GetAsync(ROOTENDPOINT);

            Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        }
        
        [Fact]
        public async Task Success_Get_With_Rating_Filter_Should_Return_OK()
        {
            var response = await _client.GetAsync(ROOTENDPOINT+"2");

            Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        }
        
        [Fact]
        public async Task Fail_Get_With_Rating_Filter_OutOfRange_Should_Return_BadRequest()
        {
            var response = await _client.GetAsync(ROOTENDPOINT+"24");
            
            Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        }
        
        [Fact]
        public async Task Success_Post_Should_Return_OK()
        {
            _client.DefaultRequestHeaders.Add("Ubi-UserId", "test");
            var response = await _client.PostAsync(ROOTENDPOINT+"24", new StringContent( JsonConvert.SerializeObject(new SessionFeedback()
            {
                Rate = 2,
                Comment = "This is an integration test"
            }), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
        }
        
        [Fact]
        public async Task Fail_Post_Should_Return_Conflict_When_Already_Exists_Feedback_For_UserId_SessionId()
        {
            _client.DefaultRequestHeaders.Add("Ubi-UserId", "test");
            // Insert first
            var response = await _client.PostAsync(ROOTENDPOINT+"24", new StringContent( JsonConvert.SerializeObject(new SessionFeedback()
            {
                Rate = 2,
                Comment = "This is an integration test"
            }), Encoding.UTF8, "application/json"));
            
            // Insert same feedback again
            var responseConflict = await _client.PostAsync(ROOTENDPOINT+"24", new StringContent( JsonConvert.SerializeObject(new SessionFeedback()
            {
                Rate = 2,
                Comment = "This is an integration test"
            }), Encoding.UTF8, "application/json"));
            
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            Assert.Equal(HttpStatusCode.Conflict,responseConflict.StatusCode);
        }
        
        [Fact]
        public async Task Fail_Post_Should_Return_BadRequest_When_Header_Missing()
        {
            var response = await _client.PostAsync(ROOTENDPOINT+"24", new StringContent( JsonConvert.SerializeObject(new SessionFeedback()
            {
                Rate = 2,
                Comment = "This is an integration test"
            }), Encoding.UTF8, "application/json"));
            
            Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Fail_Post_Should_Return_BadRequest_When_Header_Wrong(string headerUserId)
        {
            _client.DefaultRequestHeaders.Add("Ubi-UserId", headerUserId);
            var response = await _client.PostAsync(ROOTENDPOINT+"24", new StringContent( JsonConvert.SerializeObject(new SessionFeedback()
            {
                Rate = 2,
                Comment = "This is an integration test"
            }), Encoding.UTF8, "application/json"));
            
            Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        }

        public void Dispose()
        {
            _runner?.Dispose();
            _client?.Dispose();
        }
    }
}
