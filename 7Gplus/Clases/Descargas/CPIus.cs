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
    public class CPIus
    {
        public MainWindow m   { get; set; }
        public Consola    con { get; set; }

        public CPIus()
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

            string CPIus = String.Format("https://www.quandl.com/api/v3/datasets/FRED/CPIAUCNS.xml?api_key=");
            try
            {
                XElement xmlDoc = XElement.Load(CPIus);
                List<tCPIu> tCPIus = new List<tCPIu>();
                for (int i = 0; i < xmlDoc.Descendants("datum").Descendants("datum").LongCount(); i += 2)
                {
                    tCPIus.Add(new tCPIu
                    {
                        date = DateTime.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i).Value.ToString(), CultureInfo.InvariantCulture),
                        value = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 1).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 1).Value.ToString(), CultureInfo.InvariantCulture),
                        database_code = xmlDoc.Descendants("dataset").Descendants("database-code").ElementAtOrDefault(0).Value.ToString(),
                        dataset_code = xmlDoc.Descendants("dataset").Descendants("dataset-code").ElementAtOrDefault(0).Value.ToString()
                    });
                }
                GEntities _7G = new GEntities();
                foreach (tCPIu tc in tCPIus)
                {
                    var tC = (from p in _7G.tCPIus
                              where p.date == tc.date && p.database_code == tc.database_code && p.dataset_code == tc.dataset_code && p.value == tc.value
                              select p).SingleOrDefault();
                    if (tC == null)
                    {
                        _7G.tCPIus.Add(tc);
                    }
                    else
                    {
                        tC.date = tc.date;
                        tC.value = tc.value;
                        tC.database_code = tc.database_code;
                        tC.dataset_code = tc.dataset_code;

                        _7G.Entry(tC).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                _7G.SaveChanges();

                con.escribirLinea(String.Format("Descargado Exitosamente! dataset U.S. Consumer Price Index for All Urban Consumers: All Items (CPIAUCNS) "));
            }
            catch (Exception ex)
            {
                con.escribirLinea(String.Format("{0}", ex.Message.ToString()));
            }
        }
    }
}
