using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;

namespace ScriptedReviews.Notifications
{
    public class EmailNotificationSender : IEmailNotificationSender, ITransientDependency
    {
        private readonly IEmailSender _emailSender;

        public EmailNotificationSender(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendSeriesUpdateAsync(
            string email,
            string seriesName)
        {
            await _emailSender.SendAsync(
                email,
                "Actualización de serie",
                $"La serie '{seriesName}' tuvo cambios recientes."
            );
        }
    }
}
