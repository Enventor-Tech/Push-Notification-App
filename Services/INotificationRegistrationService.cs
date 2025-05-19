using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotifications.Services
{
    public interface INotificationRegistrationService
    {
        Task DeregisterDeviceAsync();
        Task RegisterDeviceAsync(params string[] tags);
        Task RefreshRegistrationAsync();
        Task ScheduleNotificationAsync(string message, int minutesBefore); 
        Task UpdateNotificationAsync(string message, int newMinutesBefore);
    }
}
