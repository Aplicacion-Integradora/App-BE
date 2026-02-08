using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Settings;

namespace ScriptedReviews.Settings
{
    public static class NotificationSettings
    {
        public const string GroupName = "Notifications";

        public const string EmailEnabled = GroupName + ".EmailEnabled";
    }
}