using System.Collections.Generic;
using System.Threading.Tasks;
using GameSessionFeedback.Models;

namespace GameSessionFeedback.Services
{
    public interface IFeedbackService
    {
        Task<IEnumerable<SessionFeedback>> GetSessionFeedbacksAsync(short? rating);
    }
}