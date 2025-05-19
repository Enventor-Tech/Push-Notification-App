using PushNotifications.Services;
using PushNotifications.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PushNotifications
{
    public partial class MainPage : ContentPage
    {
        readonly INotificationRegistrationService _notificationRegistrationService;
#if ANDROID
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            PermissionStatus status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        }
#endif
        public MainPage(INotificationRegistrationService service)
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
            _notificationRegistrationService = service;
        }
        void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            _notificationRegistrationService.RegisterDeviceAsync()
                .ContinueWith((task) =>
                {
                    ShowAlert(task.IsFaulted ? task.Exception.Message : $"Device registered");
                });
        }

        void OnDeregisterButtonClicked(object sender, EventArgs e)
        {
            _notificationRegistrationService.DeregisterDeviceAsync()
                .ContinueWith((task) =>
                {
                    ShowAlert(task.IsFaulted ? task.Exception.Message : $"Device deregistered");
                });
        }
        void OnScheduleNotificationClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageEntry.Text) || string.IsNullOrWhiteSpace(MinutesBeforeEntry.Text))
            {
                ShowAlert("Please enter a message and minutes before.");
                return;
            }

            if (!int.TryParse(MinutesBeforeEntry.Text, out int minutesBefore) || minutesBefore < 0)
            {
                ShowAlert("Please enter a valid number of minutes.");
                return;
            }

            _notificationRegistrationService.ScheduleNotificationAsync(MessageEntry.Text, minutesBefore)
                .ContinueWith((task) =>
                {
                    ShowAlert(task.IsFaulted
                        ? task.Exception.Message
                        : $"Notification scheduled for tomorrow at {CalculateScheduledTime(minutesBefore):hh:mm tt}.");
                });
        }

        void OnUpdateNotificationClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageEntry.Text) || string.IsNullOrWhiteSpace(MinutesBeforeEntry.Text))
            {
                ShowAlert("Please enter a message and minutes before.");
                return;
            }

            if (!int.TryParse(MinutesBeforeEntry.Text, out int newMinutesBefore) || newMinutesBefore < 0)
            {
                ShowAlert("Please enter a valid number of minutes.");
                return;
            }

            _notificationRegistrationService.UpdateNotificationAsync(MessageEntry.Text, newMinutesBefore)
                .ContinueWith((task) =>
                {
                    ShowAlert(task.IsFaulted
                        ? task.Exception.Message
                        : $"Notification updated to tomorrow at {CalculateScheduledTime(newMinutesBefore):hh:mm tt}.");
                });
        }


        void ShowAlert(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DisplayAlert("Push notifications demo", message, "OK")
                    .ContinueWith((task) =>
                    {
                        if (task.IsFaulted)
                            throw task.Exception;
                    });
            });
        }

        DateTime CalculateScheduledTime(int minutesBefore)
        {
            var now = DateTime.Now; // Local time (PKT)
            var tomorrow = now.AddDays(1);
            return new DateTime(
                tomorrow.Year, tomorrow.Month, tomorrow.Day,
                now.Hour, now.Minute, now.Second).AddMinutes(-minutesBefore);
        }


    }
}
