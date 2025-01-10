using WebAPI.Models;

namespace WebAPI.Interface
{
    public class SaveRes
    {
        public string Message {  get; set; }
        public int CreatedId { get; set; } 
        public SaveStatus Status { get; set; }
    }
}
