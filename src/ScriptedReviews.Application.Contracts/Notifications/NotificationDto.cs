using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ScriptedReviews.Notifications.Dtos
{
    public class NotificationDto : EntityDto<int>
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool WasRead { get; set; }
        public DateTime? SentTime { get; set; }
    }

}