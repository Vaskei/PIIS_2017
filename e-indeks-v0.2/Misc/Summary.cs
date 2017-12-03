using System.Collections.Generic;

namespace e_Index.Studomat
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

public class VisokoUcilisteNaKojemOstvarujePravo
{
    public int sifra { get; set; }
    public string naziv { get; set; }
}

public class KatalogStudentRazinapravanaprehranu
{
    public string href { get; set; }
}

public class Links2
{
    public KatalogStudentRazinapravanaprehranu katalog_student_razinapravanaprehranu { get; set; }
}

public class RazinaPravaNaPrehranu
{
    public string naziv { get; set; }
    public int sifraNivoPrava { get; set; }
    public Links2 _links { get; set; }
}

public class Embedded3
{
    public VisokoUcilisteNaKojemOstvarujePravo visokoUcilisteNaKojemOstvarujePravo { get; set; }
    public RazinaPravaNaPrehranu razinaPravaNaPrehranu { get; set; }
}

public class PravoNaPrehranu
{
    public string datumUpisaZadnjegUpisnogLista { get; set; }
    public string datumPravaOd { get; set; }
    public string datumPravaDo { get; set; }
    public Embedded3 _embedded { get; set; }
}

public class KatalogStudentRazlogprestankaprava
{
    public string href { get; set; }
}

public class Links3
{
    public KatalogStudentRazlogprestankaprava katalog_student_razlogprestankaprava { get; set; }
}

public class RazlogPrestankaPrava
{
    public int sifra { get; set; }
    public string naziv { get; set; }
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

public class KatalogNastavniprogramNacinizvedbestudija
{
    public string href { get; set; }
}

public class Links5
{
    public KatalogNastavniprogramNacinizvedbestudija katalog_nastavniprogram_nacinizvedbestudija { get; set; }
}

public class NacinIzvedbeStudijaUzKojiSeTemeljFinanaciranjaMozePojaviti
{
    public string oznaka { get; set; }
    public string naziv { get; set; }
    public Links5 _links { get; set; }
}

public class Embedded4
{
    public NacinIzvedbeStudijaUzKojiSeTemeljFinanaciranjaMozePojaviti nacinIzvedbeStudijaUzKojiSeTemeljFinanaciranjaMozePojaviti { get; set; }
}

public class TemeljFinanciranja
{
    public int sifra { get; set; }
    public string opis { get; set; }
    public string placanje { get; set; }
    public Links4 _links { get; set; }
    public Embedded4 _embedded { get; set; }
}

public class KatalogUpisgodineNacinupisapredmeta
{
    public string href { get; set; }
}

public class Links6
{
    public KatalogUpisgodineNacinupisapredmeta katalog_upisgodine_nacinupisapredmeta { get; set; }
}

public class NacinUpisa
{
    public int sifra { get; set; }
    public string opis { get; set; }
    public Links6 _links { get; set; }
}

public class Semestar
{
    public int redniBroj { get; set; }
}

public class KatalogUpisgodineStatusupisanogpredmeta
{
    public string href { get; set; }
}

public class Links7
{
    public KatalogUpisgodineStatusupisanogpredmeta katalog_upisgodine_statusupisanogpredmeta { get; set; }
}

public class Status
{
    public int sifra { get; set; }
    public string opis { get; set; }
    public Links7 _links { get; set; }
}

public class IzbornaGrupa
{
    public int sifra { get; set; }
    public string naziv { get; set; }
}

public class Embedded6
{
    public NacinUpisa nacinUpisa { get; set; }
    public Semestar semestar { get; set; }
    public Status status { get; set; }
    public IzbornaGrupa izbornaGrupa { get; set; }
}

public class Predmet
{
    public int sifra { get; set; }
    public string naziv { get; set; }
    public double ectsBodovi { get; set; }
    public int satiUkupno { get; set; }
    public string ulaziUProsjek { get; set; }
    public string zaPraviloPrijenosa { get; set; }
    public Embedded6 _embedded { get; set; }
}

public class Ocjenjivac
{
    public string oznaka { get; set; }
    public string ime { get; set; }
    public string prezime { get; set; }
}

public class Embedded7
{
    public Ocjenjivac ocjenjivac { get; set; }
}

public class Ispit
{
    public string datumIspita { get; set; }
    public string datumRoka { get; set; }
    public string ocjena { get; set; }
    public Embedded7 _embedded { get; set; }
}

public class Embedded5
{
    public Predmet predmet { get; set; }
    public Ispit ispit { get; set; }
}

public class PolozeniPredmeti
{
    public Embedded5 _embedded { get; set; }
}

public class Embedded2
{
    public List<object> nepolozeniPredmeti { get; set; }
    public PravoNaPrehranu pravoNaPrehranu { get; set; }
    public RazlogPrestankaPrava razlogPrestankaPrava { get; set; }
    public TemeljFinanciranja temeljFinanciranja { get; set; }
    public List<PolozeniPredmeti> polozeniPredmeti { get; set; }
}

public class UpisaniElementiStruktureStudija
{
    public int sifra { get; set; }
    public string paralelni { get; set; }
    public string naziv { get; set; }
    public int akademskaGodinaZadnjegUpisnogLista { get; set; }
    public int nastavnaGodina { get; set; }
    public Embedded2 _embedded { get; set; }
}

public class Student
{
    public string jmbag { get; set; }
    public string ime { get; set; }
    public string prezime { get; set; }
}

public class Embedded
{
    public List<UpisaniElementiStruktureStudija> upisaniElementiStruktureStudija { get; set; }
    public Student student { get; set; }
}

public class RootObject
{
    public Links _links { get; set; }
    public Embedded _embedded { get; set; }
}
}
