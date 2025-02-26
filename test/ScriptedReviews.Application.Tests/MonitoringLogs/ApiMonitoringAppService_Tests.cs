using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Xunit;
using Volo.Abp.Domain.Repositories;


namespace ScriptedReviews.MonitoringLogs;

public abstract class ApiMonitoringAppService_Tests<TStartupModule> : ScriptedReviewsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IApiMonitoringAppService _apiMonitoringAppService;
    private readonly IRepository<ApiMonitoringLog, int> _apiMonitoringRepository;

    protected ApiMonitoringAppService_Tests()
    {
        _apiMonitoringAppService = GetRequiredService<IApiMonitoringAppService>();
        _apiMonitoringRepository = GetRequiredService<IRepository<ApiMonitoringLog, int>>();
    }

    [Fact]
    public async Task Should_Save_ApiMonitoringLog()
    {
        // Arrange: Crear un log de prueba
        var log = new ApiMonitoringLog(
            endpoint: "/api/test",
            httpMethod: "GET",
            responseTime: 120,
            httpStatusCode: 200,
            userAgent: "TestAgent",
            ipAddress: "127.0.0.1"
        );

        // Act: Guardar el log en la base de datos
        await _apiMonitoringRepository.InsertAsync(log);

        // Assert: Verificar que el log se ha guardado correctamente
        var logs = await _apiMonitoringRepository.GetListAsync();
        logs.ShouldContain(l => l.Endpoint == "/api/test");
    }

    [Fact]
    public async Task Should_Return_ApiLogs()
    {
        // Arrange: Insertar un log de prueba en la base de datos
        var log = new ApiMonitoringLog(
            endpoint: "/api/test",
            httpMethod: "GET",
            responseTime: 120,
            httpStatusCode: 200,
            userAgent: "TestAgent",
            ipAddress: "127.0.0.1"
        );

        await _apiMonitoringRepository.InsertAsync(log);

        // Asegurar que se guardan los cambios
        //await _apiMonitoringRepository.GetDbContextAsync().Result.SaveChangesAsync();

        // Act: Obtener la lista de logs desde la API
        var result = await _apiMonitoringAppService.GetApiLogsAsync();

        // Assert: Verificar que devuelve al menos un registro
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }
}
