using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Interface
{
    public class ProgramCommitteeModel
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdConference { get; set; }
        public List<ReviewModel> Reviews { get; set; }
        public List<SessionModel> Sessions { get; set; }
    }
}
