using System.Threading.Tasks;

namespace ScriptedReviews.Notifications
{
    public interface IEmailNotificationSender
    {
        Task SendSeriesUpdateAsync(string email, string seriesName);
    }
}
