using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using e_Index.Misc;

// taj namespace potrebno je ukljuciti u potrebne klase koje ce koristiti navedeno ispod...
namespace SEUS.EXCELKlase
{
    // ova klasa ima potrebne get i set metode koje se primjenjuju
    // kod excel Controller.. i tamo se poziva "using SEUS.EXCELPodaciOStudentu" tj. 
    // taj tu namespace koji sadrzava classe. u ovom slucaju samo studentPodaci..
    // EXCEL
    public class StudentPodaci
    {
        public string ime { get; set; }
        public string prezime { get; set; }
        public string email { get; set; }
        public string jmbag { get; set; }
        public string jmbg { get; set; }
        public string oib { get; set; }
        public string datumrodenja { get; set; }
        public string spol { get; set; }
        public string godinazavrsetkaskole { get; set; }
        public string imemajke { get; set; }
        public string imeoca { get; set; }
    }

    // ova klasa je za upisane kolegije...
    // to je treci link na padajucem izborniku
    // EXCEL public class Kolegij
    public class Kolegij
    {
        public string Naziv { get; set; }
        public string Profesor { get; set; }
        public string Ocjena { get; set; }
        public bool Polozen { get; set; }
        public List<Kolegij> kolegijlista { get; set; }
        public Kolegij()
        {
            this.kolegijlista = new List<Kolegij>();
        }
    }

  

    // ova klasa je za provjeru semestra...
    public class Semestar
    {
        public int Oznaka { get; set; }
        public List<Kolegij> Kolegiji { get; set; }
        public Semestar()
        {
            this.Kolegiji = new List<Kolegij>();
        }

        public static implicit operator Semestar(e_Index.Misc.Semestar v)
        {
            throw new NotImplementedException();
        }
    }
    // ova klasa sluzi da bismo odredili studij na kojem se studira...
    public class Studij
    {
        public string Naziv { get; set; }
        public List<Semestar> Semestri { get; set; }
        public Studij()
        {
            this.Semestri = new List<Semestar>();
        }
    }

    public class StudentInfo
    {
        public string Naziv { get; set; }
        public int ectsKolegij { get; set; }
        public int kolegijOcjena { get; set; }
        public List<Studij> Studiji { get; set; }
        public StudentInfo()
        {
            this.Studiji = new List<Studij>();
        }
    }


}
