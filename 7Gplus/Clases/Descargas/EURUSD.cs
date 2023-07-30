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
    public class EURUSD
    {
        public MainWindow m   { get; set; }
        public Consola    con { get; set; }

        public EURUSD()
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
            m.Dispatcher.Invoke(() =>
            {
                //Creo las Variables de la Consola de Resultados (RichTextBox)
                con.escribirLinea("Iniciando..");

                // Inicio de la Descarga de Datos de Quandl

                // Describimos el inicio del proceso
                con.escribirLinea(String.Format("Descargando dataset SPOT EXCHANGE RATE - EURO AREA, Business day"));
            });


            string CME = String.Format("https://www.quandl.com/api/v3/datasets/FED/RXI_US_N_B_EU.xml?api_key=Kx7z33eMkxL5j29Jx1xe");

            try
            {
                XElement xmlDoc = XElement.Load(CME);
                List<tEURUSDfed> tPpps = new List<tEURUSDfed>();
                for (int i = 0; i < xmlDoc.Descendants("datum").Descendants("datum").LongCount(); i += 2)
                {
                    tPpps.Add(new tEURUSDfed
                    {
                        Date = DateTime.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i).Value.ToString(), CultureInfo.InvariantCulture),
                        Value = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 1).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 1).Value.ToString(), CultureInfo.InvariantCulture),
                        database_code = xmlDoc.Descendants("dataset").Descendants("database-code").ElementAtOrDefault(0).Value.ToString(),
                        dataset_code = xmlDoc.Descendants("dataset").Descendants("dataset-code").ElementAtOrDefault(0).Value.ToString()
                    });
                }
                using (GEntities _7G = new GEntities())
                {
                    foreach (tEURUSDfed tp in tPpps)
                    {
                        var tP = (from p in _7G.tEURUSDfed
                                  where p.Date == tp.Date && p.database_code == tp.database_code && p.dataset_code == tp.dataset_code && p.Value == tp.Value
                                  select p).SingleOrDefault();
                        if (tP == null)
                        {
                            _7G.tEURUSDfed.Add(tp);
                        }
                        else
                        {
                            tP.Date = tp.Date;
                            tP.Value = tp.Value;
                            tP.database_code = tp.database_code;
                            tP.dataset_code = tp.dataset_code;

                            _7G.Entry(tP).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    _7G.SaveChanges();
                }
                    con.escribirLinea("Descargado Exitosamente! dataset SPOT EXCHANGE RATE - EURO AREA, Business day");

            }

            catch (Exception ex)
            {
                con.escribirLinea(String.Format("{0}", ex.Message.ToString()));
            }

        }
    }
}
