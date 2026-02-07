using NSubstitute;
using ScriptedReviews.Notifications;
using ScriptedReviews.Settings;
using ScriptedReviews.Watchlists;
using ScriptedReviews.Watchlists.Dtos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Settings;
using Xunit;

namespace ScriptedReviews.Notifications;

public abstract class NotificationGenerator_Tests<TStartupModule> : ScriptedReviewsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IRepository<Notification, int> _notificationRepository;
    private readonly IWatchlistAppService _watchlistAppServiceMock;
    private readonly IEmailNotificationSender _emailSenderMock;
    private readonly IIdentityUserRepository _userRepositoryMock;
    private readonly ISettingProvider _settingProviderMock;
    private readonly NotificationGenerator _notificationGenerator;

    protected NotificationGenerator_Tests()
    {
        _notificationRepository = GetRequiredService<IRepository<Notification, int>>();
        
        // Create mocks
        _watchlistAppServiceMock = Substitute.For<IWatchlistAppService>();
        _emailSenderMock = Substitute.For<IEmailNotificationSender>();
        _userRepositoryMock = Substitute.For<IIdentityUserRepository>();
        _settingProviderMock = Substitute.For<ISettingProvider>();

        _notificationGenerator = new NotificationGenerator(
            _notificationRepository,
            _watchlistAppServiceMock,
            _userRepositoryMock,
            _settingProviderMock,
            _emailSenderMock
        );
    }

    [Fact]
    public async Task Should_Send_Email_When_EmailEnabled_Is_True()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userEmail = "test@example.com";
        var seriesName = "Breaking Bad";

        var watchlistWithChanges = new List<WatchlistDto>
        {
            new WatchlistDto { UserId = userId, Name = seriesName, HasChanges = true }
        };

        _watchlistAppServiceMock.GetSeriesWithChangesAsync()
            .Returns(Task.FromResult(watchlistWithChanges));

        _settingProviderMock.GetAsync<bool>(NotificationSettings.EmailEnabled)
            .Returns(Task.FromResult(true));

        var mockUser = Substitute.For<IdentityUser>();
        mockUser.Email.Returns(userEmail);
        _userRepositoryMock.GetAsync(userId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(mockUser));

        // Act
        await _notificationGenerator.GenerateAsync();

        // Assert
        await _emailSenderMock.Received(1).SendSeriesUpdateAsync(userEmail, seriesName);
    }

    [Fact]
    public async Task Should_Not_Send_Email_When_EmailEnabled_Is_False()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userEmail = "test@example.com";
        var seriesName = "Game of Thrones";

        var watchlistWithChanges = new List<WatchlistDto>
        {
            new WatchlistDto { UserId = userId, Name = seriesName, HasChanges = true }
        };

        _watchlistAppServiceMock.GetSeriesWithChangesAsync()
            .Returns(Task.FromResult(watchlistWithChanges));

        _settingProviderMock.GetAsync<bool>(NotificationSettings.EmailEnabled)
            .Returns(Task.FromResult(false)); // EmailEnabled = false

        var mockUser = Substitute.For<IdentityUser>();
        mockUser.Email.Returns(userEmail);
        _userRepositoryMock.GetAsync(userId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(mockUser));

        // Act
        await _notificationGenerator.GenerateAsync();

        // Assert - Email should NOT be sent
        await _emailSenderMock.DidNotReceive().SendSeriesUpdateAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Should_Not_Send_Email_When_User_Has_No_Email()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var seriesName = "The Mandalorian";

        var watchlistWithChanges = new List<WatchlistDto>
        {
            new WatchlistDto { UserId = userId, Name = seriesName, HasChanges = true }
        };

        _watchlistAppServiceMock.GetSeriesWithChangesAsync()
            .Returns(Task.FromResult(watchlistWithChanges));

        _settingProviderMock.GetAsync<bool>(NotificationSettings.EmailEnabled)
            .Returns(Task.FromResult(true));

        var mockUser = Substitute.For<IdentityUser>();
        mockUser.Email.Returns(string.Empty); // No email
        _userRepositoryMock.GetAsync(userId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(mockUser));

        // Act
        await _notificationGenerator.GenerateAsync();

        // Assert - Email should NOT be sent
        await _emailSenderMock.DidNotReceive().SendSeriesUpdateAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Should_Create_InApp_Notification_Regardless_Of_EmailEnabled()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var seriesName = "Stranger Things";

        var watchlistWithChanges = new List<WatchlistDto>
        {
            new WatchlistDto { UserId = userId, Name = seriesName, HasChanges = true }
        };

        _watchlistAppServiceMock.GetSeriesWithChangesAsync()
            .Returns(Task.FromResult(watchlistWithChanges));

        _settingProviderMock.GetAsync<bool>(NotificationSettings.EmailEnabled)
            .Returns(Task.FromResult(false)); // Emails disabled

        var mockUser = Substitute.For<IdentityUser>();
        mockUser.Email.Returns("test@example.com");
        _userRepositoryMock.GetAsync(userId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(mockUser));

        // Act
        await _notificationGenerator.GenerateAsync();

        // Assert - In-app notification should still be created
        var notifications = await _notificationRepository.GetListAsync();
        notifications.Count.ShouldBe(1);
        notifications[0].UserId.ShouldBe(userId);
        notifications[0].Description.ShouldContain(seriesName);
    }
}
