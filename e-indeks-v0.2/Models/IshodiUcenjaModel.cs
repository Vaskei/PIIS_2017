using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEUS.Models
{
    public class IshodiUcenjaModel
    {
        public class Self
        {
            public string href { get; set; }
        }

        public class Post
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
            public Post post { get; set; }
            public Profile profile { get; set; }
        }

        public class NastavniprogramPredmetuakademskojgodiniIshodiucenja
        {
            public string href { get; set; }
        }

        public class Edit
        {
            public string href { get; set; }
        }

        public class Links2
        {
            public NastavniprogramPredmetuakademskojgodiniIshodiucenja nastavniprogram_predmetuakademskojgodini_ishodiucenja { get; set; }
            public Edit edit { get; set; }
        }

        public class IshodiUcenja
        {
            public int redniBroj { get; set; }
            public string opis { get; set; }
            public Links2 _links { get; set; }
        }

        public class Embedded
        {
            public List<IshodiUcenja> ishodiUcenja { get; set; }
        }

        public class RootObject
        {
            public Links _links { get; set; }
            public Embedded _embedded { get; set; }
        }
    }
}
