using Microsoft.Azure.NotificationHubs;
using SamplePushNotification.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamplePushNotification.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel() {
            SendCommand = new Command(() => { Send(); }, ()=> { return !String.IsNullOrWhiteSpace(Message); });

            CheckSettings();
        }

        public ICommand SendCommand { get; private set; }

        public string Tags
        {
            get => tags;
            set
            {
                tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }

        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public string OutcomeState
        {
            get => outcomeState;
            set
            {
                outcomeState = value;
                OnPropertyChanged(nameof(outcomeState));
            }
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public string AlertMessage
        {
            get => alertMessage;
            set
            {
                alertMessage = value;
                OnPropertyChanged(nameof(AlertMessage));
            }
        }

        private async void Send()
        {
            OutcomeState = String.Empty;

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = Settings.NotificationHubName;
            string notificationHubConnection = Settings.NotificationHubConnectionString;

            // Create a new Notification Hub client.
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            // Sending the message so that all template registrations that contain "messageParam"
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();
            //templateParams["messageParam"] = Message;

            templateParams.Add("message", Message);

            try
            {
                // Send the push notification and log the results.
                var result = await hub.SendTemplateNotificationAsync(templateParams, Tags);

                // Write the success result to the logs.
                //config.Services.GetTraceWriter().Info(result.State.ToString());

                Message = String.Empty;

                OutcomeState = result.State.ToString();
            }
            catch (Exception ex)
            {
                // Write the failure result to the logs.
                //config.Services.GetTraceWriter()
                //    .Error(ex.Message, null, "Push.SendAsync Error");
                OutcomeState = "Push.SendAsync Error";
            }
        }

        private void CheckSettings()
        {
            string notificationHubName = Settings.NotificationHubName;
            string notificationHubConnection = Settings.NotificationHubConnectionString;

            IsEnabled = !String.IsNullOrWhiteSpace(notificationHubName) &&
                !String.IsNullOrWhiteSpace(notificationHubConnection);

            if (!isEnabled)
            {
                AlertMessage = "Check app settings.";
            }
        }

        string tags;
        string message;
        string outcomeState;
        string alertMessage;
        bool isEnabled;
    }
}
