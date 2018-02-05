using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEUS.PodatkomatStudijKlase
{
    public class Self
    {
        public string href { get; set; }
    }

    public class Profile
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public Profile profile { get; set; }
    }

    public class KatalogNastavniprogramTipindeksa
    {
        public string href { get; set; }
    }

    public class Links2
    {
        public KatalogNastavniprogramTipindeksa katalog_nastavniprogram_tipindeksa { get; set; }
    }

    public class TipIndeksa
    {
        public string oznaka { get; set; }
        public string opis { get; set; }
        public Links2 _links { get; set; }
    }

    public class Embedded3
    {
        public TipIndeksa tipIndeksa { get; set; }
    }

    public class Indeks
    {
        public Embedded3 _embedded { get; set; }
    }

    public class KatalogNastavniprogramRazinastudija
    {
        public string href { get; set; }
    }

    public class Links3
    {
        public KatalogNastavniprogramRazinastudija katalog_nastavniprogram_razinastudija { get; set; }
    }

    public class RazinaStudija
    {
        public int sifra { get; set; }
        public string naziv { get; set; }
        public Links3 _links { get; set; }
    }

    public class Embedded4
    {
        public RazinaStudija razinaStudija { get; set; }
    }

    public class UpisNaRazinuStudija
    {
        public int redniBrojStudija { get; set; }
        public string datumUpisa { get; set; }
        public int akademskaGodinaUpisa { get; set; }
        public Embedded4 _embedded { get; set; }
    }

    public class NastavniprogramEss
    {
        public string href { get; set; }
    }

    public class Links4
    {
        public NastavniprogramEss nastavniprogram_ess { get; set; }
    }

    public class UpisaniElementStruktureStudija
    {
        public int sifra { get; set; }
        public string naziv { get; set; }
        public Links4 _links { get; set; }
    }

    public class StudentskaPrava
    {
        public string datumDoKojegTrajuStudentskaPrava { get; set; }
    }

    public class Embedded2
    {
        public Indeks indeks { get; set; }
        public UpisNaRazinuStudija upisNaRazinuStudija { get; set; }
        public UpisaniElementStruktureStudija upisaniElementStruktureStudija { get; set; }
        public StudentskaPrava studentskaPrava { get; set; }
    }

    public class StudentoviStudiji
    {
        public string paralelniStudij { get; set; }
        public string glavniStudij { get; set; }
        public double prosjek { get; set; }
        public double tezinskiProsjek { get; set; }
        public Embedded2 _embedded { get; set; }
    }

    public class Embedded
    {
        public List<StudentoviStudiji> studentoviStudiji { get; set; }
    }

    public class RootObject
    {
        public Links _links { get; set; }
        public Embedded _embedded { get; set; }
    }
}
