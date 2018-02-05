using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEUS.Modelss
{
    public class StudentModel
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Jmbag { get; set; }
        
    }

    public class StudentiInfo
    {
        public string Godina { get; set; }
        public List<StudentModel> StudentiList { get; set; }

        public StudentiInfo()
        {
            StudentiList = new List<StudentModel>();
        }
    }
}
