using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScriptedReviews.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;

namespace ScriptedReviews.BackgroundWorkers
{
    public class NotificationBackgroundWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency, IHostedService
    {
        private readonly INotificationAppService _notificationAppService;

        public NotificationBackgroundWorker(INotificationAppService notificationAppService, AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory)
        : base(timer, serviceScopeFactory)
        {
            _notificationAppService = notificationAppService;
            Timer.Period = 3600000;  // Ejecutar cada hora (3600000 ms)
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            // Llama al servicio para generar las notificaciones
            await _notificationAppService.GenerateNotificationsAsync();
        }

        // Implementación de IHostedService
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return StartAsync();  // Llama al método StartAsync del BackgroundWorkerBase
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return StopAsync();  // Llama al método StopAsync del BackgroundWorkerBase
        }
    }
}
