using System.Collections.Generic;
using System.Threading.Tasks;
using GameSessionFeedback.Models;

namespace GameSessionFeedback.Services
{
    public interface IFeedbackService
    {
        /// <summary>
        /// Get session feedback asynchronously.
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        Task<IEnumerable<SessionFeedback>> GetSessionFeedbacksAsync(short? rating);
        
        /// <summary>
        /// Creates feedback asynchrounously
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        /// <exception cref="MongoWriteException"></exception>
        Task<SessionFeedback> CreateFeedbackAsync(SessionFeedback feedback);
    }
}