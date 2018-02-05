using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEUS.Models
{
    public class BrojIzlazaka
    {
        public string ID { get; set; }
        public string kolegij { get; set; }
        public bool polozeni { get; set; }
        public string semestar { get; set; }
        public int brojIzlazaka { get; set; }
        public DateTime datumIspitnogRoka { get; set; }
    }
}
