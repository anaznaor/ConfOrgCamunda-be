using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Interface
{
    public class ReviewModel
    {
        public int Id { get; set; }

        public int IdPaper { get; set; }

        public int IdReviewer { get; set; }

        public int Grade { get; set; }
        public string Description { get; set; }
        public byte[]? HashReview { get; set; }
    }
}
