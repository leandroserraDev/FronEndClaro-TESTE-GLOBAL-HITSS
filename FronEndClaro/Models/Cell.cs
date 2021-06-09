using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FronEndClaro.Models
{
    public class Cell
    {
        public Guid Code { get;  set; }
        public string Model { get; set; }
        public int Price { get; set; }
        public string Brand { get; set; }
        public string Photo { get; set; }
        public DateTime Date { get; set; }
    }
}
