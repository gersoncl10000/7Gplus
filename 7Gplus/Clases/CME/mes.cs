using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Gplus.Clases.CME
{
    public class mes
    {
        // Codigo de letra en Dataset de Quandql
        private string ene = "F";
        private string feb = "G";
        private string mar = "H";
        private string apr = "J";
        private string may = "K";
        private string jun = "M";
        private string jul = "N";
        private string aug = "Q";
        private string sep = "U";
        private string oct = "V";
        private string nov = "X";
        private string dec = "Z";

        // Nombre del Mes
        private string eneN = "Enero";
        private string febN = "Febrero";
        private string marN = "Marzo";
        private string aprN = "Abril";
        private string mayN = "Mayo";
        private string junN = "Junio";
        private string julN = "Julio";
        private string augN = "Agosto";
        private string sepN = "Septimembre";
        private string octN = "Octubre";
        private string novN = "Noviembre";
        private string decN = "Diciembre";


        public string getLetterMesCME(int m) //retun the month letter of Quandl API for CME Futures 
        {

            switch (m)
            {
                case 1: return ene;
                case 2: return feb;
                case 3: return mar;
                case 4: return apr;
                case 5: return may;
                case 6: return jun;
                case 7: return jul;
                case 8: return aug;
                case 9: return sep;
                case 10: return oct;
                case 11: return nov;
                case 12: return dec;
                default: return "";
            }

        }
        public string getNombreMesCME(int m) //retun the month name of Quandl API for CME Futures 
        {

            switch (m)
            {
                case 1: return eneN;
                case 2: return febN;
                case 3: return marN;
                case 4: return aprN;
                case 5: return mayN;
                case 6: return junN;
                case 7: return julN;
                case 8: return augN;
                case 9: return sepN;
                case 10: return octN;
                case 11: return novN;
                case 12: return decN;
                default: return "";
            }

        }



    }
}
