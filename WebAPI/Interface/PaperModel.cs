using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Interface
{
    public class PaperModel
    {
        public int Id { get; set; }

        public int IdRegistration { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public bool Decision { get; set; }
        public byte[]? HashPaper { get; set; }

        public List<ReviewModel>? Reviews { get; set; }
    }
}