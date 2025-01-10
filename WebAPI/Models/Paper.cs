using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class Paper
    {
        public int Id { get; set; }

        public int? IdRegistration { get; set; }

        public Registration Registration { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string Title { get; set; }
        [Column(TypeName = "VARBINARY(MAX)")]
        public byte[] Abstract { get; set; }
        [Column(TypeName = "VARBINARY(MAX)")]
        public byte[]? FullPaper { get; set; } 
        public PaperDecision Decision { get; set; } = PaperDecision.Rejected; 

        public IList<Review>? Reviews { get; set; }
    }
}