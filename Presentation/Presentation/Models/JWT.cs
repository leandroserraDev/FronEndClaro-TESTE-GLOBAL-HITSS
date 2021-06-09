using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Models
{
    public class JWT
    {
        public int id { get; set; }
        public string login { get; set; }
        public string senha{ get; set; }
        public string token { get; set; }
    }
}
