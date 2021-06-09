using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Models
{
    public class CellViewModel
    {
        public int id { get; set; }
        public Guid code { get; set; }
        public string model { get; set; }
        public int price { get; set; }
        public string brand { get; set; }
        public string photo { get; set; }
        public DateTime date { get; set; }
    }
}
