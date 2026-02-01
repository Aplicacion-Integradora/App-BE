using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ScriptedReviews.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        /// <summary>
        /// Método para generar notificaciones basado en cambios en la lista de seguimiento.
        /// </summary>
        Task GenerateNotificationsAsync();
    }
}
