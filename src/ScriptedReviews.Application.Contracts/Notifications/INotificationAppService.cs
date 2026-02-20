using ScriptedReviews.Notifications.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ScriptedReviews.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        Task<List<NotificationDto>> GetMyNotificationsAsync();
        Task MarkAsReadAsync(int id);
        Task GenerateNotificationsAsync();
    }
}
