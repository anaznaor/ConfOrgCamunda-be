using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class GuestLecturer
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public UserConf User { get; set; }
        public int IdConference { get; set; }
        public Conference Conference { get; set; }
    }
}
