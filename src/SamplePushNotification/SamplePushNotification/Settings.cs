using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePushNotification
{
    public class Settings
    {
        public static string NotificationHubName
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.AZURE_NOTIFICATION_HUB_NAME];
            }
        }

        public static string NotificationHubConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.AZURE_NOTIFICATION_HUB_CONNECTION_STRING];
            }
        }
    }
}
