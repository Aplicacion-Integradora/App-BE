using NSubstitute;
using ScriptedReviews.Notifications;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Xunit;

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
}
