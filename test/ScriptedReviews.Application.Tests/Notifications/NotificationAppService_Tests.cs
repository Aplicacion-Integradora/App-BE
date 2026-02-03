using Moq;
using ScriptedReviews.Notifications;
using ScriptedReviews.Series;
using ScriptedReviews.Watchlists;
using ScriptedReviews.Watchlists.Dtos;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace ScriptedReviews.Notifications;

public abstract class NotificationAppService_Tests<TStartupModule> : ScriptedReviewsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly NotificationAppService _notificationAppService;
    private readonly Mock<IRepository<Watchlist, int>> _mockWatchlistRepository;
    private readonly Mock<IRepository<Notification, int>> _mockNotificationRepository;
    private readonly Mock<IWatchlistAppService> _mockWatchlistAppService;

    protected NotificationAppService_Tests()
    {
        // Configurar los repositorios simulados
        _mockWatchlistRepository = new Mock<IRepository<Watchlist, int>>();
        _mockNotificationRepository = new Mock<IRepository<Notification, int>>();
        _mockWatchlistAppService = new Mock<IWatchlistAppService>();

        // Crear la instancia del servicio bajo prueba con los repositorios simulados
        _notificationAppService = new NotificationAppService(
            _mockNotificationRepository.Object,
            _mockWatchlistRepository.Object,
            _mockWatchlistAppService.Object
        );
    }

    [Fact]
    public async Task Should_Generate_Notifications_For_Each_Serie_Inside_Watchlist()
    {
        // Arrange
        var watchlistsFromService = new List<WatchlistDto>
        {
            new WatchlistDto
            {
                Id = 1,
                Name = "Mis Favoritas",
                HasChanges = true,
                Series = new List<SerieDto>
                {
                    new SerieDto { Title = "Breaking Bad" },      // Serie 1
                    new SerieDto { Title = "Game of Thrones" }    // Serie 2
                }
            }
        };

        _mockWatchlistAppService
            .Setup(service => service.GetSeriesWithChangesAsync())
            .ReturnsAsync(watchlistsFromService);

        // Act
        await _notificationAppService.GenerateNotificationsAsync();
        
        // Assert
        _mockNotificationRepository.Verify(
            repo => repo.InsertAsync(
                It.IsAny<Notification>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Exactly(2) // Esperamos dos notificaciones ya que InsertAsync se llama dos veces (una por cada serie)
        );

        // Verificación de detalle (Opcional): Confirmamos que una de las notificaciones menciona "Breaking Bad"
        _mockNotificationRepository.Verify(
            repo => repo.InsertAsync(
                It.Is<Notification>(n => n.Description.Contains("Breaking Bad")),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}