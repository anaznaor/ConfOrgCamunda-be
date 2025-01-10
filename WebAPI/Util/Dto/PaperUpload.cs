using Microsoft.AspNetCore.Hosting.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Util.Dto
{
    public class PaperUpload
    {
        public string Guest { get; set; }
        public string? Title { get; set; }

        public IFormFile? PaperAbstract { get; set; }
        public IFormFile? FullPaper { get; set; }
    }
}
