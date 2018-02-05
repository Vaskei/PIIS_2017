using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEUS.Modelss
{
    public class SkolarineModel
    {
        public int sifraUpisaGodine { get; set; }
        public int akademskaGodina { get; set; }
        public int nastavnaGodina { get; set; }
        public string paralelniStudij { get; set; }
        public double ukupniSaldo { get; set; }
        public double saldoNaDanasnjiDan { get; set; }

    }

    public class TransakcijeModel
    {
        public int sifra { get; set; }
        public int redniBroj { get; set; }
        public string datumTransakcije { get; set; }
        public string vrstaTransakcije { get; set; }
        public double iznosDugovanja { get; set; }
        public string knjizeno { get; set; }
        public double? iznosPotrazivanja { get; set; }

    }

    public class SkolarineInfo
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Naziv { get; set; }

        public List<SkolarineModel> SkolarinaList { get; set; }

        public List<TransakcijeModel> TransakcijeList { get; set; }

        public SkolarineInfo()
        {
            SkolarinaList = new List<SkolarineModel>();
            TransakcijeList = new List<TransakcijeModel>();
        }

    }

    

}
