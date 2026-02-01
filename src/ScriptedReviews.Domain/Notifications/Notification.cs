using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace ScriptedReviews.Notifications
{
    public class Notification : AggregateRoot<int>
    {
        public string Description { get; set; }

        public DateTime? SentTime { get; set; }

        public string Type { get; set; }

        public bool WasRead { get; set; }
    }
}
