using ScriptedReviews.ErrorLogs;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace ScriptedReviews.ErrorLogs;

public abstract class ErrorLogAppService_Tests<TStartupModule> : ScriptedReviewsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IErrorLogAppService _errorLogAppService;
    private readonly IRepository<ErrorLog, int> _errorLogRepository;

    protected ErrorLogAppService_Tests()
    {
        _errorLogAppService = GetRequiredService<IErrorLogAppService>();
        _errorLogRepository = GetRequiredService<IRepository<ErrorLog, int>>();
    }

    [Fact]
    public async Task Should_Get_Empty_Error_Logs()
    {
        // Act
        var result = await _errorLogAppService.GetErrorLogsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Get_Error_Logs()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () =>
        {
            await _errorLogRepository.InsertAsync(new ErrorLog(
                level: "Error",
                message: "Test error message",
                exception: "System.Exception: Test exception",
                source: "TestClass.TestMethod",
                endpoint: "/api/test",
                httpMethod: "GET",
                httpStatusCode: 500,
                userId: Guid.NewGuid().ToString(),
                ipAddress: "127.0.0.1"
            ), autoSave: true);

            await _errorLogRepository.InsertAsync(new ErrorLog(
                level: "Warning",
                message: "Test warning message",
                exception: null,
                source: "AnotherClass",
                endpoint: "/api/another",
                httpMethod: "POST",
                httpStatusCode: 400,
                userId: null,
                ipAddress: "192.168.1.1"
            ), autoSave: true);
        });

        // Act
        var result = await _errorLogAppService.GetErrorLogsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Should_Get_Error_Logs_By_Level()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () =>
        {
            await _errorLogRepository.InsertAsync(new ErrorLog(
                level: "Error",
                message: "Error message 1",
                exception: "Stack trace here",
                source: "Class1",
                endpoint: "/api/endpoint1",
                httpMethod: "GET",
                httpStatusCode: 500
            ), autoSave: true);

            await _errorLogRepository.InsertAsync(new ErrorLog(
                level: "Error",
                message: "Error message 2",
                exception: null,
                source: "Class2",
                endpoint: "/api/endpoint2",
                httpMethod: "POST",
                httpStatusCode: 500
            ), autoSave: true);

            await _errorLogRepository.InsertAsync(new ErrorLog(
                level: "Warning",
                message: "Warning message",
                exception: null,
                source: "Class3",
                endpoint: "/api/endpoint3",
                httpMethod: "PUT",
                httpStatusCode: 400
            ), autoSave: true);
        });

        // Act
        var errorLogs = await _errorLogAppService.GetErrorLogsByLevelAsync("Error");
        var warningLogs = await _errorLogAppService.GetErrorLogsByLevelAsync("Warning");

        // Assert
        errorLogs.ShouldNotBeNull();
        errorLogs.Count.ShouldBe(2);
        errorLogs.ShouldAllBe(e => e.Level == "Error");

        warningLogs.ShouldNotBeNull();
        warningLogs.Count.ShouldBe(1);
        warningLogs[0].Level.ShouldBe("Warning");
        warningLogs[0].Message.ShouldBe("Warning message");
    }

    [Fact]
    public async Task Should_Return_Logs_Ordered_By_CreatedAt_Descending()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () =>
        {
            await _errorLogRepository.InsertAsync(new ErrorLog(
                level: "Error",
                message: "Primer error (más viejo)",
                endpoint: "/api/first"
            ), autoSave: true);

            await Task.Delay(10); // Delay para asegurar diferentes timestamps

            await _errorLogRepository.InsertAsync(new ErrorLog(
                level: "Error",
                message: "Segundo error (más nuevo)",
                endpoint: "/api/second"
            ), autoSave: true);
        });

        // Act
        var result = await _errorLogAppService.GetErrorLogsAsync();

        // Assert
        result.Count.ShouldBe(2);
        result[0].Message.ShouldBe("Segundo error (más nuevo)");
        result[1].Message.ShouldBe("Primer error (más viejo)");
    }
}
