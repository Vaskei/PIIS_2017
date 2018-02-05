using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEUS.Modelss
{
    public class Self
    {
        public string href { get; set; }
    }

    public class UpisgodineSkolarinaStudentRjesenjaopromjeniskolarine
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
        public UpisgodineSkolarinaStudentRjesenjaopromjeniskolarine upisgodine_skolarina_student_rjesenjaopromjeniskolarine { get; set; }
        public StudentStudentSlika student_student_slika { get; set; }
        public StudentStudentStudentovstudij student_student_studentovstudij { get; set; }
        public StudentStudentSumarnipodaci student_student_sumarnipodaci { get; set; }
        public StudentStudentStudentovprelazak student_student_studentovprelazak { get; set; }
    }

    public class UpisgodineSkolarinaStudent
    {
        public string href { get; set; }
    }

    public class StudentStudent
    {
        public string href { get; set; }
    }

    public class StudentStudentV2
    {
        public string href { get; set; }
    }

    public class Links2
    {
        public StudentStudent student_student { get; set; }
        public StudentStudentV2 __invalid_name__student_student_v2 { get; set; }
    }

    public class Studenti
    {
        public string jmbag { get; set; }
        public string ime { get; set; }
        public string prezime { get; set; }
        public Links2 _links { get; set; }
    }

    public class Skolarine
    {
        public int sifraUpisaGodine { get; set; }
        public int akademskaGodina { get; set; }
        public int nastavnaGodina { get; set; }
        public string paralelniStudij { get; set; }
        public double ukupniSaldo { get; set; }
        public double saldoNaDanasnjiDan { get; set; }
        public Links2 _links { get; set; }
    }

    public class Skolarina
    {
        public double ukupniSaldo { get; set; }
        public double saldoNaDanasnjiDan { get; set; }
        public double iznosSkolarineNaVu { get; set; }
    }

    public class Student
    {
        public string jmbag { get; set; }
        public string ime { get; set; }
        public string prezime { get; set; }
        public Links2 _links { get; set; }
    }

    public class Embedded
    {
        public List<Studenti> studenti { get; set; }
        public List<Skolarine> skolarine { get; set; }
        public Skolarina skolarina { get; set; }
        public Student student { get; set; }
        public Transakcije transakcije { get; set; }
        public OsobniPodaci osobniPodaci { get; set; }
    }

    public class RootObject
    {
        public Links _links { get; set; }
        public Embedded _embedded { get; set; }      
    }

    public class Self2
    {
        public string href { get; set; }
    }

    public class Links4
    {
        public Self2 self { get; set; }
    }

    public class Storno
    {
        public string stornirano { get; set; }
    }

    public class KatalogUpisgodineNacinplacanja
    {
        public string href { get; set; }
    }

    public class Links5
    {
        public KatalogUpisgodineNacinplacanja katalog_upisgodine_nacinplacanja { get; set; }
    }

    public class NacinPlacanja
    {
        public int sifra { get; set; }
        public string opis { get; set; }
        public Links5 _links { get; set; }
    }

    public class Embedded3
    {
        public Storno storno { get; set; }
        public NacinPlacanja nacinPlacanja { get; set; }
        public Mjesto mjesto { get; set; }
    }

    public class Transakcije2
    {
        public int sifra { get; set; }
        public int redniBroj { get; set; }
        public string datumTransakcije { get; set; }
        public string vrstaTransakcije { get; set; }
        public double iznosDugovanja { get; set; }
        public string knjizeno { get; set; }
        public Links4 _links { get; set; }
        public Embedded3 _embedded { get; set; }
        public double? iznosPotrazivanja { get; set; }
    }

    public class Embedded2
    {
        public List<Transakcije2> transakcije { get; set; }
        public StudiraVisokoUciliste studiraVisokoUciliste { get; set; }
    }
    public class Links3
    {
        public Post post { get; set; }
    }

    public class Post
    {
        public string href { get; set; }
    }

    public class Transakcije
    {
        public Links3 _links { get; set; }
        public Embedded2 _embedded { get; set; }
    }

    //---------------------------------------------//

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

    public class StudiraVisokoUciliste
    {
        public int sifra { get; set; }
        public string naziv { get; set; }
    }

    public class StudentNaVisokomUcilistu
    {
        public string lokalniMaticniBr { get; set; }
        public string datumUpis { get; set; }
        public string eMailStud { get; set; }
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

    public class MaticnoVisokoUciliste
    {
        public int sifra { get; set; }
        public string naziv { get; set; }
    }

    public class Embedded4
    {       
        public MaticnoVisokoUciliste maticnoVisokoUciliste { get; set; }    
    }

    public class OsobniPodaci
    {
        public string jmbag { get; set; }
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

}
