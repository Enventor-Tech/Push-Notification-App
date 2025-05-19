using PushNotifications.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;

namespace PushNotifications.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    private readonly NotificationService _notificationService;
    private string _easternTime;
    private DateTime _lastCapturedTime;

    public string EasternTime
    {
        get => _easternTime;
        set => SetProperty(ref _easternTime, value);
    }

    public ICommand RegisterNotificationCommand { get; }
    public ICommand DeregisterNotificationCommand { get; }

    public MainPageViewModel()
    {
        _notificationService = new NotificationService();
        RegisterNotificationCommand = new Command(async () => await RegisterNotificationAsync());
        DeregisterNotificationCommand = new Command(async () => await DeregisterNotificationAsync());

        UpdateEasternTime(); // Set initial time
    }

    private void UpdateEasternTime()
    {
        try
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            EasternTime = $"Eastern Time: {easternTime:MMMM dd, yyyy hh:mm:ss tt}";
            _lastCapturedTime = easternTime;
        }
        catch (TimeZoneNotFoundException)
        {
            EasternTime = "Eastern Time Zone not found.";
        }
        catch (Exception ex)
        {
            EasternTime = $"Error: {ex.Message}";
        }
    }

    private async Task RegisterNotificationAsync()
    {
        try
        {
           /* await _notificationService.RegisterDeviceAsync();
            SchedulePushNotifications(_lastCapturedTime);
            await ShowAlertAsync("Device registered and push scheduled.");*/
        }
        catch (Exception ex)
        {
            await ShowAlertAsync($"Error: {ex.Message}");
        }
    }

    private async Task DeregisterNotificationAsync()
    {
        try
        {
           /* await _notificationService.DeregisterDeviceAsync();
            await ShowAlertAsync("Device deregistered.");*/
        }
        catch (Exception ex)
        {
            await ShowAlertAsync($"Error: {ex.Message}");
        }
    }

    private void SchedulePushNotifications(DateTime originalEasternTime)
    {
        string nowMessage = $"Registered Time: {originalEasternTime:MMM dd, yyyy hh:mm tt}";
        DateTime nextDayScheduledTime = originalEasternTime.AddDays(1).AddMinutes(-45);
        string scheduledMessage = $"Reminder: Scheduled time at {originalEasternTime:hh:mm tt}";

        // Simulated logs / stubbed scheduling
        Console.WriteLine($"[Push Now] {nowMessage}");
        Console.WriteLine($"[Scheduled for: {nextDayScheduledTime}] {scheduledMessage}");

        // TODO: Replace with real service calls to backend / notification hub
    }

    private Task ShowAlertAsync(string message)
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            // You can hook this method to an event, message bus, or shell to show alerts.
            Application.Current?.MainPage?.DisplayAlert("Push Notifications", message, "OK");
        });
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}
