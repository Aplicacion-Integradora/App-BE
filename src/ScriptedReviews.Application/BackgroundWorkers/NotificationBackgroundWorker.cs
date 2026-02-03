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
    public class NotificationBackgroundWorker
    : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly NotificationGenerator _notificationGenerator;
        public NotificationBackgroundWorker(
        NotificationGenerator notificationGenerator,
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory)
        : base(timer, serviceScopeFactory)
        {
            _notificationGenerator = notificationGenerator;
            Timer.Period = 3600000;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            await _notificationGenerator.GenerateAsync();
        }
    }


}
