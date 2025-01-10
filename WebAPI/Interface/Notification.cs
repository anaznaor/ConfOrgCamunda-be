using WebAPI.Models;

namespace WebAPI.Interface
{
    public class Notification
    {
        public string User {  get; set; }
        public string Message { get; set; }
        public NotificationStatus Status { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
