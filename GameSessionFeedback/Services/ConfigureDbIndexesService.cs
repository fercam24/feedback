using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using GameSessionFeedback.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace GameSessionFeedback.DbContexts
{
    public class ConfigureDbIndexesService : IHostedService
    {
        private readonly IMongoDbContext _dbContext;
        private readonly ILogger<ConfigureDbIndexesService> _logger;
        private readonly IFeedbackDatabaseSettings _dbSettings;

        public ConfigureDbIndexesService(IMongoDbContext dbContext, ILogger<ConfigureDbIndexesService> logger, IFeedbackDatabaseSettings dbSettings)
        {
            _dbContext = dbContext;
            _logger = logger;
            _dbSettings = dbSettings;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var collection = _dbContext.GetCollection<SessionFeedback>(_dbSettings.SessionFeedbacksCollectionName);
            var indexKeysDef = Builders<SessionFeedback>.IndexKeys.Ascending(feedback => feedback.UserId).Ascending(feedback => feedback.SessionId);
            
            var options = new CreateIndexOptions() { Unique = true };

            await collection.Indexes.CreateOneAsync(new CreateIndexModel<SessionFeedback>(indexKeysDef, options), cancellationToken : cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}