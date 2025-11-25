using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;
using Moq;
using Volo.Abp.Domain.Repositories;
using ScriptedReviews.Notifications;
using ScriptedReviews.Watchlists;
using System.Threading;
using System.Linq;
using ScriptedReviews.Watchlists.Dtos;

namespace ScriptedReviews.Application.Tests.Notifications
{
    public abstract class NotificationAppService_Tests<TStartupModule> : ScriptedReviewsApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly NotificationAppService _notificationAppService;
        private readonly Mock<IRepository<Notification, int>> _mockNotificationRepository;
        private readonly Mock<IRepository<Watchlist, int>> _mockWatchlistRepository;
        private readonly Mock<IWatchlistAppService> _mockWatchlistAppService;

        protected NotificationAppService_Tests()
        {
            _mockNotificationRepository = new Mock<IRepository<Notification, int>>();
            _mockWatchlistRepository = new Mock<IRepository<Watchlist, int>>();
            _mockWatchlistAppService = new Mock<IWatchlistAppService>();

            _notificationAppService = new NotificationAppService(
                _mockNotificationRepository.Object,
                _mockWatchlistRepository.Object,
                _mockWatchlistAppService.Object
            );
        }

        [Fact]
        public async Task Should_Generate_Notifications_When_Watchlist_Has_Changes()
        {
            // Arrange
            var watchlistWithChanges = new List<WatchlistDto>
        {
            new WatchlistDto { Id = 1, Name = "Serie A", HasChanges = true },
            new WatchlistDto { Id = 2, Name = "Serie B", HasChanges = true }
        };

            _mockWatchlistAppService
                .Setup(s => s.GetSeriesWithChangesAsync())
                .ReturnsAsync(watchlistWithChanges);

            // Act
            await _notificationAppService.GenerateNotificationsAsync();

            // Assert
            _mockNotificationRepository.Verify(
                repo => repo.InsertAsync(It.IsAny<Notification>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
                Times.Exactly(watchlistWithChanges.Count)
            );
        }
    }
}