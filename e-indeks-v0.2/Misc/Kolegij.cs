using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace e_Index.Misc
{
    public class Kolegij
    {
        public string Naziv { get; set; }
        public string Profesor { get; set; }
        public int ECTS { get; set; }
        public string Ocjena { get; set; }
        public bool Polozen { get; set; }
    }

    public class Semestar
    {
        public int Oznaka { get; set; }
        public List<Kolegij> Kolegiji { get; set; }

        public Semestar()
        {
            this.Kolegiji = new List<Kolegij>();
        }
    }

    public class Studij
    {
        public string Naziv { get; set; }
        public List<Semestar> Semestri { get; set; }
        public int ECTS_Ukupno { get; set; }
        public int ECTS_Osvojeno { get; set; }

        public Studij()
        {
            this.Semestri = new List<Semestar>();
        }
    }

    public class StudentInfo
    {
        public string Naziv { get; set; }
        public List<Studij> Studiji { get; set; }

        public StudentInfo()
        {
            this.Studiji = new List<Studij>();
        }
    }
}
