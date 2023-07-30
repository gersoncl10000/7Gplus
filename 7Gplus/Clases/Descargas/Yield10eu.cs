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
    public class Yield10eu
    {
        public MainWindow m   { get; set; }
        public Consola    con { get; set; }

        public Yield10eu()
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

            string sBeu = String.Format("http://sdw.ecb.europa.eu/quickviewexport.do?SERIES_KEY=YC.B.U2.EUR.4F.G_N_C.SV_C_YM.SR_10Y&type=sdmx");
            try
            {
                XElement xmlDoc = XElement.Load(sBeu);
                var ns = xmlDoc.GetDefaultNamespace(); // Carga el Espacio de Nombre por defecto
                List<tbondEU10Y> tBeun = new List<tbondEU10Y>();
                //var xmlQ = xmlDoc.Descendants(ns + "Series").Elements(ns + "Obs");

                foreach (var n in xmlDoc.Descendants(ns + "Series").Elements(ns + "Obs").ToList())
                {
                    tBeun.Add(new tbondEU10Y
                    {
                        date = DateTime.Parse(n.Attributes("TIME_PERIOD").FirstOrDefault().Value == "" ? "0" : n.Attributes("TIME_PERIOD").FirstOrDefault().Value, CultureInfo.InvariantCulture),
                        value = Double.Parse(n.Attributes("OBS_VALUE").FirstOrDefault().Value == "" ? "0" : n.Attributes("OBS_VALUE").FirstOrDefault().Value, CultureInfo.InvariantCulture),
                        OBS_STATUS = n.Attributes("OBS_STATUS").FirstOrDefault().Value.ToString(),
                        database_code = "ECB",
                        dataset_code = "YC.B.U2.EUR.4F.G_N_C.SV_C_YM.SR_10"
                    });
                }
                GEntities _7G = new GEntities();
                foreach (tbondEU10Y tc in tBeun)
                {
                    var tC = (from p in _7G.tbondEU10Y
                              where p.date == tc.date && p.database_code == tc.database_code && p.dataset_code == tc.dataset_code && p.value == tc.value && p.OBS_STATUS == tc.OBS_STATUS
                              select p).SingleOrDefault();
                    if (tC == null)
                    {
                        _7G.tbondEU10Y.Add(tc);
                    }
                    else
                    {
                        tC.date = tc.date;
                        tC.value = tc.value;
                        tC.OBS_STATUS = tc.OBS_STATUS;
                        tC.database_code = tc.database_code;
                        tC.dataset_code = tc.dataset_code;

                        _7G.Entry(tC).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                _7G.SaveChanges();

                    con.escribirLinea(String.Format("Descargado Exitosamente! dataset Yield curve spot rate, 10-year maturity - Government bond, nominal, all issuers all ratings included - Euro area "));
  
            }
            catch (Exception ex)
            {

                    con.escribirLinea(String.Format("{0}", ex.Message.ToString()));

            }
        }
    }
}
