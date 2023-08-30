using _7Gplus.Clases.MainWinows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;

namespace _7Gplus.Clases.Descargas
{
    public class Yield10us
    {
        public MainWindow m   { get; set; }
        public Consola    con { get; set; }

        public Yield10us()
        {
            m = (MainWindow)System.Windows.Application.Current.MainWindow;
            con = new Consola();
        }
        public void descargar()
        {
            System.Threading.Thread des = new System.Threading.Thread(ejecutarDescargar);
            des.Start();
        }

        private void ejecutarDescargar()
        {

            string sBond = String.Format("https://www.quandl.com/api/v3/datasets/FRED/DGS10.xml?api_key=&&limit=300");
            try
            {
                con.escribirLinea(String.Format("Iniciando Descarga de  dataset FRED 10-Year Treasury Constant Maturity Rate"));
                XElement xmlDoc = XElement.Load(sBond);
                List<tbondUS10Y> tBondus = new List<tbondUS10Y>();
                var r = xmlDoc.Descendants("datum").Descendants("datum").LongCount();
                for (int i = 0; i < r; i += 2) // xmlDoc.Descendants("datum").Descendants("datum").LongCount()
                {
                    tBondus.Add(new tbondUS10Y
                    {
                        date = DateTime.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i).Value.ToString(), CultureInfo.InvariantCulture),
                        value = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 1).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 1).Value.ToString(), CultureInfo.InvariantCulture),
                        database_code = xmlDoc.Descendants("dataset").Descendants("database-code").ElementAtOrDefault(0).Value.ToString(),
                        dataset_code = xmlDoc.Descendants("dataset").Descendants("dataset-code").ElementAtOrDefault(0).Value.ToString()
                    });
                }
                GEntities _7G = new GEntities();
                foreach (tbondUS10Y tb in tBondus)
                {
                    var tB = (from p in _7G.tbondUS10Y
                              where p.date == tb.date && p.database_code == tb.database_code && p.dataset_code == tb.dataset_code && p.value == tb.value
                              select p).FirstOrDefault();
                    if (tB == null)
                    {
                        _7G.tbondUS10Y.Add(tb);
                    }
                    else
                    {
                        tB.date = tb.date;
                        tB.value = tb.value;
                        tB.database_code = tb.database_code;
                        tB.dataset_code = tb.dataset_code;

                        _7G.Entry(tB).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                _7G.SaveChanges();
                con.escribirLinea(String.Format("Descargado Exitosamente! dataset FRED 10-Year Treasury Constant Maturity Rate"));
            }
            catch (Exception ex)
            {
                con.escribirLinea(String.Format("{0}", ex.Message.ToString()));
            }
        }
    }
}
