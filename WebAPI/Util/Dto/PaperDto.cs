using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Util.Dto
{
    public class PaperDto
    {
        public string ProcessId { get; set; }
        public int IdPaper { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string? FullPaper { get; set; }
    }
}
