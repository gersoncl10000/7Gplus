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
    public class CPIeu
    {
        public MainWindow m   { get; set; }
        public Consola    con { get; set; }

        public CPIeu()
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

            string CPIeu = String.Format("http://sdw.ecb.europa.eu/quickviewexport.do?SERIES_KEY=ICP.M.U2.N.000000.4.INX&type=sdmx");
            try
            {
                XElement xmlDoc = XElement.Load(CPIeu);
                var ns = xmlDoc.GetDefaultNamespace(); // Carga el Espacio de Nombre por defecto
                List<tCPIeu> tCPIeun = new List<tCPIeu>();
                //var xmlQ = xmlDoc.Descendants(ns + "Series").Elements(ns + "Obs");

                foreach (var n in xmlDoc.Descendants(ns + "Series").Elements(ns + "Obs").ToList())
                {
                    tCPIeun.Add(new tCPIeu
                    {
                        date = new DateTime(
                            int.Parse(n.Attributes("TIME_PERIOD").FirstOrDefault().Value.ToString().Substring(0, 4)),
                            int.Parse(n.Attributes("TIME_PERIOD").FirstOrDefault().Value.ToString().Substring(5, 2)),
                            1),
                        value = Double.Parse(n.Attributes("OBS_VALUE").FirstOrDefault().Value == "" ? "0" : n.Attributes("OBS_VALUE").FirstOrDefault().Value, CultureInfo.InvariantCulture),
                        OBS_STATUS = n.Attributes("OBS_STATUS").FirstOrDefault().Value.ToString(),
                        database_code = "ECB",
                        dataset_code = "ICP.M.U2.N.000000.4.INX"
                    });
                }
                using (GEntities _7G = new GEntities())
                {
                    foreach (tCPIeu tc in tCPIeun)
                    {
                        var tC = (from p in _7G.tCPIeux
                                  where p.date == tc.date && p.database_code == tc.database_code && p.dataset_code == tc.dataset_code && p.value == tc.value
                                  select p).SingleOrDefault();
                        if (tC == null)
                        {
                            _7G.tCPIeux.Add(tc);
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
                }
                con.escribirLinea(String.Format("Descargado Exitosamente! dataset Harmonised Consumer Prices — All Items, Euro Area (19 Countries) "));
            }
            catch (Exception ex)
            {
                con.escribirLinea(String.Format("{0}", ex.Message.ToString()));

            }
        }
    }
}
