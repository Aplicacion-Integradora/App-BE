using NSubstitute;
using ScriptedReviews.Notifications;
using ScriptedReviews.Seasons;
using ScriptedReviews.Series;
using ScriptedReviews.Settings;
using ScriptedReviews.Watchlists;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Users;
using Xunit;
using System.Linq;


namespace ScriptedReviews.Notifications;

public abstract class EmailNotificationSender_Tests<TStartupModule> : ScriptedReviewsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IEmailSender _emailSenderMock;
    private readonly EmailNotificationSender _emailNotificationSender;

    protected EmailNotificationSender_Tests()
    {
        _emailSenderMock = Substitute.For<IEmailSender>();
        _emailNotificationSender = new EmailNotificationSender(_emailSenderMock);
    }

    [Fact]
    public async Task Should_Send_Email_With_Correct_Subject_And_Body()
    {
        // Arrange
        var email = "test@example.com";
        var seriesName = "Breaking Bad";

        // Act
        await _emailNotificationSender.SendSeriesUpdateAsync(email, seriesName);

        // Assert
        await _emailSenderMock.Received(1).SendAsync(
            email,
            "Actualización de serie",
            $"La serie '{seriesName}' tuvo cambios recientes."
        );
    }

    [Fact]
    public async Task Should_Send_Email_To_Correct_Recipient()
    {
        // Arrange
        var email = "user@domain.com";
        var seriesName = "Game of Thrones";

        // Act
        await _emailNotificationSender.SendSeriesUpdateAsync(email, seriesName);

        // Assert
        await _emailSenderMock.Received(1).SendAsync(
            Arg.Is<string>(e => e == email),
            Arg.Any<string>(),
            Arg.Any<string>()
        );
    }

    [Fact]
    public async Task Should_Include_Series_Name_In_Email_Body()
    {
        // Arrange
        var email = "test@example.com";
        var seriesName = "The Mandalorian";

        // Act
        await _emailNotificationSender.SendSeriesUpdateAsync(email, seriesName);

        // Assert
        await _emailSenderMock.Received(1).SendAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Is<string>(body => body.Contains(seriesName))
        );
    }

    [Fact]
    public async Task Should_NOT_Send_Email_When_EmailSetting_Is_Disabled()
    {
        // ---------- Arrange ----------

        var myUserId = Guid.NewGuid();

        var serie =

        new Serie
        {
            Title = "Breaking Bad",
            ImdbId = "tt0903747",
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
        };


        var watchlist = new Watchlist(myUserId)
        {
            Series = new List<Serie> { serie }
        };

        // Repositories
        var serieRepo = Substitute.For<IRepository<Serie, int>>();
        var watchlistRepo = Substitute.For<IRepository<Watchlist, int>>();
        var notificationRepo = Substitute.For<IRepository<Notification, int>>();

        serieRepo.GetListAsync(true).Returns(new List<Serie> { serie });

        watchlistRepo
            .WithDetailsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Watchlist, object>>[]>())
            .Returns(new List<Watchlist> { watchlist }.AsQueryable());

        // API devuelve más temporadas => dispara notificación
        var seriesApi = Substitute.For<ISeriesApiService>();
        seriesApi.ImportarSerieAsync(serie.ImdbId)
            .Returns(new SerieDto { TotalSeasons = 6 });

        // Usuario
        var user = new IdentityUser(watchlist.UserId, "test", "test@test.com");
        var userRepo = Substitute.For<IIdentityUserRepository>();
        userRepo.FindAsync(watchlist.UserId).Returns(user);

        // ❌ Email deshabilitado
        var settingManager = Substitute.For<ISettingManager>();
        settingManager.GetOrNullAsync(
            NotificationSettings.EmailEnabled,
            UserSettingValueProvider.ProviderName,
            watchlist.UserId.ToString()
        ).Returns("false");

        // Email sender
        var emailSender = Substitute.For<IEmailNotificationSender>();

        // Current user (no se usa acá, pero se requiere)
        var currentUser = Substitute.For<ICurrentUser>();

        var appService = new NotificationAppService(
            notificationRepo,
            serieRepo,
            watchlistRepo,
            seriesApi,
            currentUser,
            userRepo,
            settingManager,
            emailSender
        );

        // ---------- Act ----------
        await appService.GenerateNotificationsAsync();

        // ---------- Assert ----------

        // ❌ NO se debe enviar mail
        await emailSender
            .DidNotReceive()
            .SendSeriesUpdateAsync(Arg.Any<string>(), Arg.Any<string>());
    }

}
