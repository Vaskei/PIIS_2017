using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEUS.Podatkomat
{
    public class Roditelj
    {
        public string otacILImajka { get; set; }
        public string zanimanje { get; set; }
        public string polozajUZanimanju { get; set; }
        public string postignutoObrazovanje { get; set; }
        public string strucnaSprema { get; set; }
    }

    public class Studij
    {
        public string nazivJednogOdStudija { get; set; }
        public string tipIndexa { get; set; }
        public string doKadTrajuStudPrava { get; set; }
        public string prosjekStudenta { get; set; }
        public string tezinskiProsjekStudenta { get; set; }
        public string razinaINazivStudija { get; set; }
        public string redniBrojStudija { get; set; }
    }

    public class Student
    {
        public string ime { get; set; }
        public string prezime { get; set; }
        public string slikaLink { get; set; }
        public string jmbag { get; set; }
        public string nazivStudija { get; set; }
        public string nazivPodrucja { get; set; }
        public string datumUpisaStudija { get; set; }
        public string jmbg { get; set; }
        public string oib { get; set; }
        public string datumRodenja { get; set; }
        public string adresaStudenta { get; set; }
        public string postanskiBrStudenta { get; set; }
        public string slikaBase64 { get; set; }
        public string mjesto { get; set; }
        public string drzava { get; set; }
        // uzdrzavatelj
        public string uzdrzavatelj { get; set; }

        //podaci o roditeljima
        public string imeOca { get; set; }
        public string imeMajke { get; set; }

        public List<Roditelj> Roditelji { get; set; }
        public List<Studij> Studiji { get; set; }
        public Student()
        {
            this.Roditelji = new List<Roditelj>();
            this.Studiji = new List<Studij>();
        }

        //podaci o ustanovi
        public string nazivUstanove { get; set; }
        public string sifraUstanove { get; set; }
        public string emailStudenta { get; set; }
        public string emailAktivan { get; set; }


        //podaci u zavrsenoj srednjoj skoli
        public string srednjaSkola { get; set; }
        public string godinaZavrsetkaSrednje { get; set; }
        public string strukovnoPodrucjeSrednje { get; set; }
        public string smjerSrednjeSkole { get; set; }


        //studentov prelazak
        public string prelazakIzUstanove { get; set; }
        public string prelazakDatumUpisa { get; set; }
        public string prelazakDatumRjesenja { get; set; }
        public string prelazakIskoristenoSemestra { get; set; }
        //public string prelazakRjesenje { get; set; }




        //Testni podaci
        public int godinaUpisa { get; set; }
        public string podaciURL { get; set; }
        public string studijURL { get; set; }
        public string privatnoURL { get; set; }
        public string studentJSON { get; set; }
    }
}
