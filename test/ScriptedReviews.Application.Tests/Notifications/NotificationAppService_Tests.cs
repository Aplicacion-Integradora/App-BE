using Moq;
using NSubstitute;
using ScriptedReviews.Notifications;
using ScriptedReviews.Notifications.Dtos;
using ScriptedReviews.Seasons;
using ScriptedReviews.Series;
using ScriptedReviews.Watchlists;
using ScriptedReviews.Watchlists.Dtos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace ScriptedReviews.Notifications;

    public abstract class NotificationAppService_Tests<TStartupModule> : ScriptedReviewsApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
    // 1. En lugar de Mocks, declaramos las interfaces reales
        private readonly INotificationAppService _notificationAppService;
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IRepository<Watchlist, int> _watchlistRepository;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    protected NotificationAppService_Tests()
    {
        // 2. Le pedimos a ABP que nos dé las instancias listas para usar
        _notificationAppService = GetRequiredService<INotificationAppService>();
        _notificationRepository = GetRequiredService<IRepository<Notification, int>>();
        _watchlistRepository = GetRequiredService<IRepository<Watchlist, int>>();
        _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
    }

    [Fact]
    public async Task Should_Generate_Notifications_For_Each_Serie_Inside_Watchlist()
    {
        // Arrange
        var myUserId = Guid.NewGuid();
        var imdbIdBreakingBad = "tt_bb";
        var imdbIdGOT = "tt_got";

        var seriesApiServiceMock = GetRequiredService<ISeriesApiService>();

        seriesApiServiceMock.ClearReceivedCalls();

        // Para que cuando busque Breaking Bad diga que tiene 5 temporadas"
        seriesApiServiceMock.ImportarSerieAsync(imdbIdBreakingBad)
            .Returns(Task.FromResult(new SerieDto { TotalSeasons = 5 }));

        // Para que cuando busque GOT diga que tiene 8 temporadas"
        seriesApiServiceMock.ImportarSerieAsync(imdbIdGOT)
            .Returns(Task.FromResult(new SerieDto { TotalSeasons = 8 }));

        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var watchlist = new Watchlist(myUserId)
                {
                    Name = "Mis Favoritas",
                    HasChanges = true,
                };
                watchlist.Series = new List<Serie>
                {
                    new Serie
                    {
                        Title = "Breaking Bad",
                        ImdbId = imdbIdBreakingBad,
                        Description = "Un profesor de química con cáncer...",
                        Image = "bb.jpg",
                        Genre = "Drama",
                        Language = "English",
                        ReleaseDate = "2008-01-20",
                        Duration = "45 min",
                        Rating = "9.5",
                        Country = "USA",
                        Director = "Vince Gilligan",
                        Cast = "Bryan Cranston, Aaron Paul",
                        Writer = "Vince Gilligan",
                        UserId = myUserId,
                        Seasons = new List<Season>()
                    },

                    new Serie
                    {
                        Title = "Game of Thrones",
                        ImdbId = imdbIdGOT,
                        Description = "Familias nobles luchan por el trono...",
                        Image = "got.jpg",
                        Genre = "Fantasy",
                        Language = "English",
                        ReleaseDate = "2011-04-17",
                        Duration = "60 min",
                        Rating = "9.3",
                        Country = "USA",
                        Director = "Alan Taylor",
                        Cast = "Emilia Clarke, Kit Harington",
                        Writer = "George R.R. Martin",
                        UserId = myUserId,
                        Seasons = new List<Season>()
                    }
                };

                await _watchlistRepository.InsertAsync(watchlist, autoSave: true);
            });
        }

        // Act
        await _notificationAppService.GenerateNotificationsAsync();

        // Assert
        var notifications = await _notificationRepository.GetListAsync();

        notifications.Count.ShouldBe(2); // Esperamos dos notificaciones ya que tenemos dos series que han sufrido cambios recientes
        notifications.ShouldContain(n => n.Description.Contains("Breaking Bad"));
        notifications.ShouldContain(n => n.Description.Contains("Game of Thrones"));
    }

    [Fact]
    public async Task Should_Get_My_Notifications_Ordered_By_Date()
    {
        // Arrange
        var myUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _notificationRepository.InsertAsync(new Notification
            {
                UserId = otherUserId,
                Description = "Ajena",
                Type = "Email",
                SentTime = DateTime.Now,
                WasRead = false
            });

            await _notificationRepository.InsertAsync(new Notification
            {
                UserId = myUserId,
                Description = "Mia Vieja",
                Type = "Email",
                SentTime = DateTime.Now.AddDays(-2), // Hace 2 días
                WasRead = false
            });

            await _notificationRepository.InsertAsync(new Notification
            {
                UserId = myUserId,
                Description = "Mia Nueva",
                Type = "Email",
                SentTime = DateTime.Now, // Hoy
                WasRead = false
            });
        });

        // Act
        List<NotificationDto> result = null;

        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            result = await _notificationAppService.GetMyNotificationsAsync();
        }

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Description.ShouldBe("Mia Nueva"); // Verifica el orden
        result[1].Description.ShouldBe("Mia Vieja");
    }

    [Fact]
    public async Task Should_Mark_As_Read()
    {
        // Arrange
        var myUserId = Guid.NewGuid();
        var notifId = 0;

        await WithUnitOfWorkAsync(async () =>
        {
            var notif = await _notificationRepository.InsertAsync(new Notification
            {
                UserId = myUserId,
                Description = "Por leer",
                Type = "Email",
                SentTime = DateTime.Now,
                WasRead = false
            }, true); // true = auto-save para obtener el ID

            notifId = notif.Id;
        });

        // Act
        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            await _notificationAppService.MarkAsReadAsync(notifId);
        }

        // Assert
        var dbNotif = await _notificationRepository.GetAsync(notifId);
        dbNotif.WasRead.ShouldBeTrue();
    }

    private ClaimsPrincipal GetClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new Claim(AbpClaimTypes.UserId, userId.ToString()),
            new Claim(AbpClaimTypes.UserName, "user_test")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }
}
