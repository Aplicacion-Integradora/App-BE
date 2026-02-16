using NSubstitute;
using ScriptedReviews.Notifications;
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

    protected NotificationGenerator_Tests()
    {
        _notificationRepository = GetRequiredService<IRepository<Notification, int>>();
    }

    [Fact]
    public async Task Should_Check_EmailEnabled_Before_Sending_Email()
    {        
        // Arrange
        var watchlistServiceMock = Substitute.For<IWatchlistAppService>();
        var emailSenderMock = Substitute.For<IEmailNotificationSender>();
        var userRepoMock = Substitute.For<IIdentityUserRepository>();
        var settingProviderMock = Substitute.For<ISettingProvider>();

        // Empty list - no series with changes
        watchlistServiceMock.GetSeriesWithChangesAsync()
            .Returns(new List<WatchlistDto>());

        var generator = new NotificationGenerator(
            _notificationRepository,
            watchlistServiceMock,
            userRepoMock,
            settingProviderMock,
            emailSenderMock
        );

        // Act
        await generator.GenerateAsync();

        // Assert
        await emailSenderMock.DidNotReceive().SendSeriesUpdateAsync(Arg.Any<string>(), Arg.Any<string>());
        
        await watchlistServiceMock.Received(1).GetSeriesWithChangesAsync();
    }
}
