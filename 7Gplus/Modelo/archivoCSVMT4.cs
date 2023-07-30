using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Gplus.Modelo
{
    public class archivoCSVMT4
    {
        public int id { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<double> pFXclose { get; set; }
        public Nullable<double> pCMEclose { get; set; }
        public Nullable<double> pCMEproy { get; set; }
        public Nullable<double> difFXaCMEclose { get; set; }
        public Nullable<double> difCMEcCMEproy { get; set; }

        public Nullable<double> varFXaCMEclose { get; set; }



    }
}
