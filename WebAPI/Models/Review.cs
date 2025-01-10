using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int IdPaper { get; set; }
        public Paper Paper { get; set; }

        public int IdReviewer { get; set; }

        public ProgramCommittee Reviewer { get; set; }

        public int Grade { get; set; }
        [Column(TypeName = "NVARCHAR(500)")]
        public string Description { get; set; }
    }
}
