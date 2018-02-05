using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEUS.Models
{
    public class Excel_Nepolozeni_Model
    {
        // sljdeca public klasa sluzi da upisemo neke demo primjere
        //DEMO primjeri
        // tu cemo spojiti trenutno prijavljenog korisnika te cemo ispisivati njegove podatke.
        public List<Excel_Nepolozeni> FindAll() { 

            List<Excel_Nepolozeni> listaNepolozenih = new List<Excel_Nepolozeni>();
            listaNepolozenih.Add(new Excel_Nepolozeni { ID = "1", Kolegij = "Elektrotehnika", Polozeni_da_ne = "Ne", Semestar="1" });
            listaNepolozenih.Add(new Excel_Nepolozeni { ID = "2", Kolegij = "Matematika I", Polozeni_da_ne = "Ne", Semestar = "1" });
            listaNepolozenih.Add(new Excel_Nepolozeni { ID = "3", Kolegij = "Programiranje", Polozeni_da_ne = "Ne", Semestar = "1" });
            listaNepolozenih.Add(new Excel_Nepolozeni { ID = "4", Kolegij = "Ekonomika", Polozeni_da_ne = "Ne", Semestar = "2" });
            listaNepolozenih.Add(new Excel_Nepolozeni { ID = "5", Kolegij = "XML programiranje", Polozeni_da_ne = "Ne", Semestar = "4" });

            return listaNepolozenih;
        }
    }
}
