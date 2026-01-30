using ScriptedReviews.Notifications.Dtos;
using ScriptedReviews.Watchlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace ScriptedReviews.Notifications
{
    public class NotificationAppService : ApplicationService, INotificationAppService
    {
        private readonly IRepository<Notification, int> _notificationRepository;

        public NotificationAppService(
            IRepository<Notification, int> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<NotificationDto>> GetMyNotificationsAsync()
        {
            var userId = CurrentUser.GetId();

            var notifications = await _notificationRepository.GetListAsync(
                n => n.UserId == userId
            );

            return ObjectMapper.Map<List<Notification>, List<NotificationDto>>(notifications);
        }

        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _notificationRepository.GetAsync(id);

            notification.WasRead = true;
            await _notificationRepository.UpdateAsync(notification);
        }
    }

}
