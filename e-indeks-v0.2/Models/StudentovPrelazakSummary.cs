using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEUS.PodatkomatPrelazakKlase
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

    public class RjesenjeOPrelasku
    {
        public int generiraniBroj { get; set; }
        public string datumRjesenje { get; set; }
        public string brojRjesenje { get; set; }
    }

    public class DodatniPodaciUzPrelazNaVU
    {
        public int sifra { get; set; }
        public string tekst { get; set; }
    }

    public class OrganizacijskaJedinica
    {
        public int sifra { get; set; }
        public string naziv { get; set; }
    }

    public class UstanovaCentrivisokogucilista
    {
        public string href { get; set; }
    }

    public class Links2
    {
        public UstanovaCentrivisokogucilista ustanova_centrivisokogucilista { get; set; }
    }

    public class CentarVisokogUcilista
    {
        public string oznaka { get; set; }
        public string naziv { get; set; }
        public Links2 _links { get; set; }
    }

    public class KatalogUpisgodineIndikatorupisa
    {
        public string href { get; set; }
    }

    public class Links3
    {
        public KatalogUpisgodineIndikatorupisa katalog_upisgodine_indikatorupisa { get; set; }
    }

    public class IndikatorUpisa
    {
        public int sifra { get; set; }
        public string opis { get; set; }
        public Links3 _links { get; set; }
    }

    public class KatalogUpisgodineTemeljfinanciranja
    {
        public string href { get; set; }
    }

    public class Links4
    {
        public KatalogUpisgodineTemeljfinanciranja katalog_upisgodine_temeljfinanciranja { get; set; }
    }

    public class TemeljFinanciranja
    {
        public int sifra { get; set; }
        public string opis { get; set; }
        public Links4 _links { get; set; }
    }

    public class Embedded2
    {
        public CentarVisokogUcilista centarVisokogUcilista { get; set; }
        public IndikatorUpisa indikatorUpisa { get; set; }
        public TemeljFinanciranja temeljFinanciranja { get; set; }
    }

    public class StudentovPrelazak
    {
        public OrganizacijskaJedinica organizacijskaJedinica { get; set; }
        public int akademskaGodina { get; set; }
        public int nastavnaGodina { get; set; }
        public int trajanjeUSemestrima { get; set; }
        public string datumUpisa { get; set; }
        public int iskoristenoSemestaraNaTeretMinistarstva { get; set; }
        public Embedded2 _embedded { get; set; }
    }

    public class Embedded
    {
        public RjesenjeOPrelasku rjesenjeOPrelasku { get; set; }
        public DodatniPodaciUzPrelazNaVU dodatniPodaciUzPrelazNaVU { get; set; }
        public StudentovPrelazak studentovPrelazak { get; set; }
    }

    public class RootObject
    {
        public Links _links { get; set; }
        public Embedded _embedded { get; set; }
    }
}
