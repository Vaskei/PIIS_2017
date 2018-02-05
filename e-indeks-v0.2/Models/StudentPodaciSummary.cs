using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//generirane klase iz JSON kljuceva na linku "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}"
namespace SEUS.PodatkomatKlase
{
    public class Self
    {
        public string href { get; set; }
    }

    public class StudentStudentSlika
    {
        public string href { get; set; }
    }

    public class StudentStudentStudentovstudij
    {
        public string href { get; set; }
    }

    public class StudentStudentSumarnipodaci
    {
        public string href { get; set; }
    }

    public class StudentStudentStudentovprelazak
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
        public StudentStudentSlika student_student_slika { get; set; }
        public StudentStudentStudentovstudij student_student_studentovstudij { get; set; }
        public StudentStudentSumarnipodaci student_student_sumarnipodaci { get; set; }
        public StudentStudentStudentovprelazak student_student_studentovprelazak { get; set; }
        public Profile profile { get; set; }
    }

    public class StudiraVisokoUciliste
    {
        public int sifra { get; set; }
        public string naziv { get; set; }
    }

    public class Embedded2
    {
        public StudiraVisokoUciliste studiraVisokoUciliste { get; set; }
    }

    public class StudentNaVisokomUcilistu
    {
        public string lokalniMaticniBr { get; set; }
        public string datumUpis { get; set; }
        public string sviPodaciUISVU { get; set; }
        public string emailAktivan { get; set; }
        public string komentar { get; set; }
        public Embedded2 _embedded { get; set; }
    }

    public class Drzava
    {
        public string oznaka { get; set; }
        public string naziv { get; set; }
    }

    public class Mjesto
    {
        public string postanskaOznaka { get; set; }
        public string naziv { get; set; }
        public Drzava drzava { get; set; }
    }

    public class Embedded3
    {
        public Mjesto mjesto { get; set; }
    }

    public class ElektronickiIdentitet
    {
        public string oznaka { get; set; }
        public string uid { get; set; }
        public string status { get; set; }
        public string email { get; set; }
        public string adresa { get; set; }
        public Embedded3 _embedded { get; set; }
    }

    public class ProgramIzobrazbe
    {
        public string sifraIzobrazba { get; set; }
        public string naziv { get; set; }
    }

    public class Nacionalnost
    {
        public string sifraNacionalnost { get; set; }
        public string naziv { get; set; }
    }

    public class MaticnoVisokoUciliste
    {
        public int sifra { get; set; }
        public string naziv { get; set; }
    }

    public class StrukovnoPodrucje
    {
        public string sifraStruka { get; set; }
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

    public class RazinaPravaPrehrana
    {
        public string naziv { get; set; }
        public int sifraNivoPrava { get; set; }
        public Links2 _links { get; set; }
    }

    public class Drzava2
    {
        public string oznaka { get; set; }
        public string naziv { get; set; }
    }

    public class Embedded5
    {
        public Drzava2 drzava { get; set; }
    }

    public class MjestoRodjenja
    {
        public string postanskaOznaka { get; set; }
        public string naziv { get; set; }
        public Embedded5 _embedded { get; set; }
    }

    public class KatalogSkola
    {
        public string href { get; set; }
    }

    public class Links3
    {
        public KatalogSkola katalog_skola { get; set; }
    }

    public class ZavrsenaSkola
    {
        public string sifraSkola { get; set; }
        public string naziv { get; set; }
        public Links3 _links { get; set; }
    }

    public class Embedded4
    {
        public ProgramIzobrazbe programIzobrazbe { get; set; }
        public object opcinaRodjenja { get; set; }
        public Nacionalnost nacionalnost { get; set; }
        public MaticnoVisokoUciliste maticnoVisokoUciliste { get; set; }
        public StrukovnoPodrucje strukovnoPodrucje { get; set; }
        public RazinaPravaPrehrana razinaPravaPrehrana { get; set; }
        public MjestoRodjenja mjestoRodjenja { get; set; }
        public ZavrsenaSkola zavrsenaSkola { get; set; }
    }

    public class OsobniPodaci
    {
        public string jmbag { get; set; }
        public string jmbg { get; set; }
        public string oib { get; set; }
        public string ime { get; set; }
        public string prezime { get; set; }
        public string spol { get; set; }
        public string datumRodjenja { get; set; }
        public string godinaZavrsetkaSkola { get; set; }
        public string imeMajka { get; set; }
        public string imeOtac { get; set; }
        public string datumPravaOd { get; set; }
        public string datumPravaDo { get; set; }
        public Embedded4 _embedded { get; set; }
    }

    public class Embedded
    {
        public StudentNaVisokomUcilistu studentNaVisokomUcilistu { get; set; }
        public ElektronickiIdentitet elektronickiIdentitet { get; set; }
        public OsobniPodaci osobniPodaci { get; set; }
    }

    public class RootObject
    {
        public Links _links { get; set; }
        public Embedded _embedded { get; set; }
    }
}
