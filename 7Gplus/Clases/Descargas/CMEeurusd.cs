using _7Gplus.Clases.MainWinows;
using _7Gplus.Clases.CME;
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
    public class CMEeurusd
    {
        public MainWindow ma   { get; set; }
        public Consola    con { get; set; }

        public CMEeurusd()
        {
            ma = (MainWindow)System.Windows.Application.Current.MainWindow;
            con = new Consola();
        }
        public void descargar()
        {
            DescargarFuturos();
        }

        private async Task DescargarFuturos() //Descarga los Valalores del Mercado de Futuros del CME 
        {
            await Task.Factory.StartNew(() =>
            {
                ma.Dispatcher.Invoke(() =>
                {
                    ma.vDescargandoFuturos = 1;
                });
                mes m = new mes();
                string anno;
                string mes;

                ma.Dispatcher.Invoke(() =>
                {
                    //Creo las Variables de la Consola de Resultados (RichTextBox)            
                    con.escribirLinea("Iniciando descarga CME ..");
                });



                // Inicio de la Descarga de Datos de Quandl

                for (int a = 2004; a < (DateTime.Now.Year + 3); a++)
                {
                    for (int j = 1; j < 13; j++)
                    {
                        mes = m.getLetterMesCME(j);
                        anno = a.ToString();

                        // Describimos el inicio del proceso
                        ma.Dispatcher.Invoke(() =>
                        {
                            con.escribirLinea(String.Format("Descargando dataset EC{0}{1} del mes de {2} del año {1} ...", mes, anno, m.getNombreMesCME(j)));
                            //ma.txtprogBar.Text = String.Format("Descargando dataset EC{0}{1} del mes de {2} del año {1} ...", mes, anno, m.getNombreMesCME(j));

                        });


                        string CME = String.Format("https://www.quandl.com/api/v3/datasets/CME/EC{0}{1}.xml?limit=4&api_key=Kx7z33eMkxL5j29Jx1xe", mes, anno);
                        try
                        {
                            XElement xmlDoc = XElement.Load(CME);
                            List<tFuturo> lFuturos = new List<tFuturo>();
                            long NroFilas = xmlDoc.Descendants("datum").Descendants("datum").LongCount();
                            double cuentaxml = 0;
                            for (int i = 0; i < NroFilas; i += 9)
                            {
                                cuentaxml = i;
                                lFuturos.Add(new tFuturo
                                {

                                    date = DateTime.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i).Value.ToString(), CultureInfo.InvariantCulture),
                                    open = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 1).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 1).Value.ToString(), CultureInfo.InvariantCulture),
                                    high = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 2).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 2).Value.ToString(), CultureInfo.InvariantCulture),
                                    low = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 3).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 3).Value.ToString(), CultureInfo.InvariantCulture),
                                    last = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 4).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 4).Value.ToString(), CultureInfo.InvariantCulture),
                                    change = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 5).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 5).Value.ToString(), CultureInfo.InvariantCulture),
                                    settle = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 6).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 6).Value.ToString(), CultureInfo.InvariantCulture),
                                    volume = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 7).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 7).Value.ToString(), CultureInfo.InvariantCulture),
                                    previousDayOpenInterest = Double.Parse(xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 8).Value == "" ? "0" : xmlDoc.Descendants("datum").Descendants("datum").ElementAtOrDefault(i + 8).Value.ToString(), CultureInfo.InvariantCulture),
                                    database_code = xmlDoc.Descendants("dataset").Descendants("database-code").ElementAtOrDefault(0).Value.ToString(),
                                    dataset_code = xmlDoc.Descendants("dataset").Descendants("dataset-code").ElementAtOrDefault(0).Value.ToString(),
                                    endContractDate = new DateTime((j + 1) == 13 ? a + 1 : a, (j + 1) == 13 ? 1 : j, 1)
                                });
                                ma.Dispatcher.Invoke(() =>
                                {
                                    //ma.txtprogBar.Text = String.Format("Leyendo archivo XML de dataset EC{0}{1} del mes de {2} del año {1} ... {3} de {4}", mes, anno, m.getNombreMesCME(j), i, NroFilas);
                                    //ma.pgrsBarEstado.Value = cuentaxml / NroFilas;
                                });
                            }
                            GEntities _7G = new GEntities();
                            double cuenta = 0;
                            double TotalItems = lFuturos.Count;

                            foreach (tFuturo tf in lFuturos)
                            {
                                cuenta++;
                                var tF = (from f in _7G.tFuturos
                                          where f.date == tf.date && f.database_code == tf.database_code && f.dataset_code == tf.dataset_code
                                          select f).SingleOrDefault();
                                if (tF == null)
                                {
                                    _7G.tFuturos.Add(tf);
                                }
                                else
                                {
                                    if (tF.settle != tf.settle)
                                    {

                                        tF.date = tf.date;
                                        tF.open = tf.open;
                                        tF.high = tf.high;
                                        tF.low = tf.low;
                                        tF.change = tf.change;
                                        tF.settle = tf.settle;
                                        tF.volume = tf.volume;
                                        tF.previousDayOpenInterest = tf.previousDayOpenInterest;
                                        tF.database_code = tf.database_code;
                                        tF.dataset_code = tf.dataset_code;
                                        tF.endContractDate = tf.endContractDate;
                                        _7G.Entry(tF).State = System.Data.Entity.EntityState.Modified;
                                    }
                                }
                                ma.Dispatcher.Invoke(() =>
                                {
                                    //ma.txtprogBar.Text = String.Format("Guardando en BD dataset EC{0}{1} del mes de {2} del año {1} ... {3} de {4}", mes, anno, m.getNombreMesCME(j), cuenta, TotalItems);
                                    //ma.pgrsBarEstado.Value = cuenta / TotalItems;
                                });
                            }
                            _7G.SaveChanges();
                            con.escribirLinea(String.Format("Descargado Exitosamente! dataset EC{0}{1} del mes de {2} del año {1}", mes, anno, m.getNombreMesCME(j)));
                        }
                        catch (Exception ex)
                        {
                            ma.Dispatcher.Invoke(() =>
                            {

                                con.escribirLinea(String.Format("{3} / dataset EC{0}{1} del mes de {2} del año {1}", mes, anno, m.getNombreMesCME(j), ex.Message.ToString()));
                                ma.vDescargandoFuturos = 0;
                            });
                        }
                    }
                }
                ma.Dispatcher.Invoke(() =>
                {
                    ma.vDescargandoFuturos = 0;
                });

            });
        }
    }
}
