using _7Gplus.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Gplus.Clases.Calculos
{
    public class cLineaDeTendencia
    {
        public int? n { get; set; }        
        public double? sumX {get; set;}
        public double? sumY { get; set; }
        public double? mX { get; set; }
        public double? mY { get; set; }
        
        public double? sumX2xFi { get; set; } // Calcula X^2 por Fi (frecuncia) por defecto fi es 1
        public double? sumXxYxF { get; set; } // Sumatoria de X*Y*Fi   

        public double? varz { get; set; } // Calculamos la Varianza
        public double? Covarz { get; set; } // Calculamos la Covarianza


        public cLineaDeTendencia(List<oListaLineaTendenciaTiempo> f)
        {

            if (f != null)
            {

                n = f.Count;

                sumX = 0;
                sumY = 0;
                sumX2xFi = 0;
                sumXxYxF = 0;
                mX = 0;
                mY = 0;
                varz = 0;
                Covarz = 0;

                foreach (var t in f)
                {
                    sumX = sumX + DateTime.Parse(t.x.ToString()).ToOADate();
                    sumY = sumY + t.y;
                    sumX2xFi = sumX2xFi + Math.Pow(DateTime.Parse(t.x.ToString()).ToOADate(), 2);
                    sumXxYxF = sumXxYxF + (DateTime.Parse(t.x.ToString()).ToOADate() * t.y);
                }

                mX = sumX / n;
                mY = sumY / n;
                varz = (sumX2xFi / n) - (Math.Pow((double)mX, 2));
                Covarz = (sumXxYxF / n) - (mX * mY);
            }
            else
            {
                sumX = 0;
                sumY = 0;
                sumX2xFi = 0;
                sumXxYxF = 0;
                mX = 0;
                mY = 0;
                varz = 0;
                Covarz = 0;

            }

        }

        public double? getY(DateTime dateX)
        {
            double? r;
            if (varz == 0 || double.IsNaN((double)varz) || double.IsInfinity((double)varz))
            {
                r = 0;
            }
            else
            {
                r = ((Covarz / varz) * (DateTime.Parse(dateX.ToString()).ToOADate() - mX)) + mY;
            }

            return r;
        }
    }
}
