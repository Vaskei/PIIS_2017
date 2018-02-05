using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEUS.PodatkomatPrivatnoKlase
{
    public class UpisgodineStudentuakademskojgodiniKategorijastudenta
    {
        public string href { get; set; }
    }

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
        public UpisgodineStudentuakademskojgodiniKategorijastudenta upisgodine_studentuakademskojgodini_kategorijastudenta { get; set; }
        public Self self { get; set; }
        public Profile profile { get; set; }
    }

    public class Mjesto
    {
        public string postanskaOznaka { get; set; }
        public string naziv { get; set; }
    }

    public class Drzava
    {
        public string oznaka { get; set; }
        public string naziv { get; set; }
    }

    public class Embedded2
    {
        public Mjesto mjesto { get; set; }
        public Drzava drzava { get; set; }
    }

    public class Prebivaliste
    {
        public string datumIzdavanjaPotvrdeOPrebivalistu { get; set; }
        public string ulicaIBroj { get; set; }
        public string telefon { get; set; }
        public Embedded2 _embedded { get; set; }
    }

    public class KatalogUpisgodineZanimanje
    {
        public string href { get; set; }
    }

    public class Links2
    {
        public KatalogUpisgodineZanimanje katalog_upisgodine_zanimanje { get; set; }
    }

    public class Zanimanje
    {
        public string sifra { get; set; }
        public string opis { get; set; }
        public Links2 _links { get; set; }
    }

    public class KatalogUpisgodinePolozajuzanimanju
    {
        public string href { get; set; }
    }

    public class Links3
    {
        public KatalogUpisgodinePolozajuzanimanju katalog_upisgodine_polozajuzanimanju { get; set; }
    }

    public class PolozajUZanimanju
    {
        public string sifra { get; set; }
        public string opis { get; set; }
        public Links3 _links { get; set; }
    }

    public class KatalogUpisgodinePostignutoobrazovanje
    {
        public string href { get; set; }
    }

    public class Links4
    {
        public KatalogUpisgodinePostignutoobrazovanje katalog_upisgodine_postignutoobrazovanje { get; set; }
    }

    public class PostignutoObrazovanje
    {
        public int sifra { get; set; }
        public string naziv { get; set; }
        public Links4 _links { get; set; }
    }

    public class KatalogStrucnasprema
    {
        public string href { get; set; }
    }

    public class Links5
    {
        public KatalogStrucnasprema katalog_strucnasprema { get; set; }
    }

    public class StrucnaSprema
    {
        public int sifra { get; set; }
        public string kratica { get; set; }
        public string naziv { get; set; }
        public Links5 _links { get; set; }
    }

    public class Embedded3
    {
        public Zanimanje zanimanje { get; set; }
        public PolozajUZanimanju polozajUZanimanju { get; set; }
        public PostignutoObrazovanje postignutoObrazovanje { get; set; }
        public StrucnaSprema strucnaSprema { get; set; }
    }

    public class Roditelji
    {
        public string oznaka { get; set; }
        public Embedded3 _embedded { get; set; }
    }

    public class KatalogUpisgodineZdravstvenoosiguranje
    {
        public string href { get; set; }
    }

    public class Links6
    {
        public KatalogUpisgodineZdravstvenoosiguranje katalog_upisgodine_zdravstvenoosiguranje { get; set; }
    }

    public class ZdravstvenoOsiguranje
    {
        public int sifra { get; set; }
        public string opis { get; set; }
        public Links6 _links { get; set; }
    }

    public class KatalogUpisgodineZanimanje2
    {
        public string href { get; set; }
    }

    public class Links7
    {
        public KatalogUpisgodineZanimanje2 katalog_upisgodine_zanimanje { get; set; }
    }

    public class Zanimanje2
    {
        public string sifra { get; set; }
        public string opis { get; set; }
        public Links7 _links { get; set; }
    }

    public class KatalogUpisgodinePolozajuzanimanju2
    {
        public string href { get; set; }
    }

    public class Links8
    {
        public KatalogUpisgodinePolozajuzanimanju2 katalog_upisgodine_polozajuzanimanju { get; set; }
    }

    public class PolozajUZanimanju2
    {
        public string sifra { get; set; }
        public string opis { get; set; }
        public Links8 _links { get; set; }
    }

    public class KatalogUpisgodineUzdrzavatelj
    {
        public string href { get; set; }
    }

    public class Links9
    {
        public KatalogUpisgodineUzdrzavatelj katalog_upisgodine_uzdrzavatelj { get; set; }
    }

    public class NacinUzdrzavanja
    {
        public string sifra { get; set; }
        public string opis { get; set; }
        public Links9 _links { get; set; }
    }

    public class Embedded4
    {
        public Zanimanje2 zanimanje { get; set; }
        public PolozajUZanimanju2 polozajUZanimanju { get; set; }
        public NacinUzdrzavanja nacinUzdrzavanja { get; set; }
    }

    public class Uzdrzavatelj
    {
        public Embedded4 _embedded { get; set; }
    }

    public class StatusStanovanja
    {
    }

    public class Embedded5
    {
        public StatusStanovanja statusStanovanja { get; set; }
    }

    public class Stanovanje
    {
        public Embedded5 _embedded { get; set; }
    }

    public class KatalogStudentRazinapravanaprehranu
    {
        public string href { get; set; }
    }

    public class Links10
    {
        public KatalogStudentRazinapravanaprehranu katalog_student_razinapravanaprehranu { get; set; }
    }

    public class RazinaPravaNaPrehranu
    {
        public string naziv { get; set; }
        public int sifraNivoPrava { get; set; }
        public Links10 _links { get; set; }
    }

    public class Embedded
    {
        public Prebivaliste prebivaliste { get; set; }
        public List<Roditelji> roditelji { get; set; }
        public ZdravstvenoOsiguranje zdravstvenoOsiguranje { get; set; }
        public Uzdrzavatelj uzdrzavatelj { get; set; }
        public Stanovanje stanovanje { get; set; }
        public RazinaPravaNaPrehranu razinaPravaNaPrehranu { get; set; }
    }

    public class RootObject
    {
        public string jmbag { get; set; }
        public string ime { get; set; }
        public string prezime { get; set; }
        public string bracnoStanje { get; set; }
        public string primaStipendiju { get; set; }
        public string studiraNaDrugomVisokomUcilistu { get; set; }
        public Links _links { get; set; }
        public Embedded _embedded { get; set; }
    }
}
