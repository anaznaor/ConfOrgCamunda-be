using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Util.Dto
{
    public class ReviewDto
    {
        public string Reviewer { get; set; }
        public string ProcessId { get; set; }
        public int IdPaper { get; set; }
        public int Grade { get; set; }
        public string Description { get; set; }
    }
}
