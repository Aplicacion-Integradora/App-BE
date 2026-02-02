using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Claims;
using ScriptedReviews.Notifications;

namespace ScriptedReviews.Application.Tests.Notifications
{
    public class NotificationAppService_Tests
        : ScriptedReviewsApplicationTestBase<ScriptedReviewsApplicationTestModule>
    {
        private readonly INotificationAppService _notificationAppService;
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

        public NotificationAppService_Tests()
        {
            _notificationAppService = GetRequiredService<INotificationAppService>();
            _notificationRepository = GetRequiredService<IRepository<Notification, int>>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }

        /// <summary>
        /// Crea un ClaimsPrincipal falso para simular un usuario autenticado en tests.
        /// </summary>
        private ClaimsPrincipal CreatePrincipal(Guid userId)
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    },
                    authenticationType: "TestAuth"
                )
            );
        }

        [Fact]
        public async Task Should_Return_Only_Current_User_Notifications()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            await _notificationRepository.InsertAsync(new Notification
            {
                UserId = currentUserId,
                Description = "Notificación del usuario actual",
                WasRead = false
            });

            await _notificationRepository.InsertAsync(new Notification
            {
                UserId = otherUserId,
                Description = "Notificación de otro usuario",
                WasRead = false
            });

            var principal = CreatePrincipal(currentUserId);

            // Act
            using (_currentPrincipalAccessor.Change(principal))
            {
                var result = await _notificationAppService.GetMyNotificationsAsync();

                // Assert
                result.Count.ShouldBe(1);
                result[0].Description.ShouldBe("Notificación del usuario actual");
            }
        }
    }
}
