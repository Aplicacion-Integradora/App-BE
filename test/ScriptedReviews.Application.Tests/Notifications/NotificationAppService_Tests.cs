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

namespace ScriptedReviews.Application.Tests.Notifications
{
    public abstract class NotificationAppService_Tests<TStartupModule> : ScriptedReviewsApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly NotificationAppService _notificationAppService;
        private readonly Mock<IRepository<Watchlist, int>> _mockWatchlistRepository;
        private readonly Mock<IRepository<Notification, int>> _mockNotificationRepository;

        protected NotificationAppService_Tests()
        {
            // Configurar los repositorios simulados
            _mockWatchlistRepository = new Mock<IRepository<Watchlist, int>>();
            _mockNotificationRepository = new Mock<IRepository<Notification, int>>();

            // Crear la instancia del servicio bajo prueba con los repositorios simulados
            _notificationAppService = new NotificationAppService(
                _mockNotificationRepository.Object,
                _mockWatchlistRepository.Object
            );
        }

        [Fact]
        public async Task Should_Generate_Notifications_When_Watchlist_Has_Changes()
        {
            // Arrange
            var watchlistWithChanges = new List<Watchlist>
            {
                new Watchlist { Id = 1, Name = "Serie A", HasChanges = true },
                new Watchlist { Id = 2, Name = "Serie B", HasChanges = true }
            };

            // Configurar el repositorio para que devuelva la lista de series con cambios
            _mockWatchlistRepository
                .Setup(repo => repo.GetQueryableAsync())
                .ReturnsAsync(watchlistWithChanges.AsQueryable());

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