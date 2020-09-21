using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameSessionFeedback.DbContexts;
using GameSessionFeedback.Models;
using MongoDB.Driver;

namespace GameSessionFeedback.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IMongoDbContext _dbContext;
        private readonly IMongoCollection<SessionFeedback> _sessionFeedbacks;

        public FeedbackService(IMongoDbContext dbContext, IFeedbackDatabaseSettings dbSettings)
        {
            _dbContext = dbContext;
            _sessionFeedbacks = _dbContext.GetCollection<SessionFeedback>(dbSettings.SessionFeedbacksCollectionName);
        }
        public async Task<IEnumerable<SessionFeedback>> GetSessionFeedbacksAsync(short? rating)
        {
            try
            {
                var builder = Builders<SessionFeedback>.Filter;
                var filter = builder.Empty;
                var sort = Builders<SessionFeedback>.Sort.Descending(feedback => feedback.CreatedOn);
                var options = new FindOptions<SessionFeedback>(){Limit = 15, Sort = sort};

                if (rating != null)
                {
                    filter = builder.Eq(sf => sf.Rate, rating);
                }
                
                var feedbacks = await _sessionFeedbacks.FindAsync(filter, options);
                
                return feedbacks.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<SessionFeedback> CreateFeedbackAsync(SessionFeedback feedback)
        {
            try
            {
                await _sessionFeedbacks.InsertOneAsync(feedback);
            }
            catch (MongoWriteException ex)
            {
                throw ex;
            }
            
            return feedback;
        }
    }
}