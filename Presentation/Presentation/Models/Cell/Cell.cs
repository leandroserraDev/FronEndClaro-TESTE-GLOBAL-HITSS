using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Models
{
    public class Cell
    {
        public Guid Code { get; set; }
        public string Model { get; set; }
        public int Price { get; set; }
        public string Brand { get; set; }
        public string Photo { get; set; }

        [DisplayName("Photo")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public DateTime Date { get; set; }
    }
}
