using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Models
{
    public class CellViewModel
    {
        public int id { get; set; }

        [Display(Name = "Code")]
        public Guid code { get; set; }

        [Display(Name = "Model")]
        public string model { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public int price { get; set; }

        [Display(Name = "Brand")]
        public string brand { get; set; }


        [Display(Name = "Photo")]
        public string photo { get; set; }

        [Display(Name ="Photo")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date")]

        public DateTime date { get; set; }
    }
}
