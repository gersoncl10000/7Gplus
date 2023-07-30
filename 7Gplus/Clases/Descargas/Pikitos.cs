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
using _7Gplus.Clases.Calculos;

namespace _7Gplus.Clases.Descargas
{
    public class Pikitos
    {
        public MainWindow ma   { get; set; }
        public Consola    con { get; set; }

        public Pikitos()
        {
            ma = (MainWindow)System.Windows.Application.Current.MainWindow;
            con = new Consola();
        }
        public void descargar()
        {
            RellenarPikitos();
        }

        private async Task RellenarPikitos() //Descarga los Valalores del Mercado de Futuros del CME 
        {
            await Task.Factory.StartNew(() =>
            {
                ma.Dispatcher.Invoke(() =>
                {
                    //Creo las Variables de la Consola de Resultados (RichTextBox)            
                    con.escribirLinea("Calculado Pikitos ..");
                });
                using (GEntities _7G = new GEntities())
                {
                    var f = _7G.tEURUSDfed.Select(x => x.Date).ToList();

                    foreach ( var d in f)
                    {
                        try
                        {

                            var pi = _7G.tpikitosT.Where(x => x.date == (DateTime)d).SingleOrDefault();
                            DateTime? dia = d;
                            LineaDeTendencia LineaCME = new LineaDeTendencia((DateTime)dia);
                            if (pi == null)
                            {

                                tpikitosT Pk = new tpikitosT();
                                // Calculo de los precios Proyectados Segun CME
                                Pk.date = dia;
                                Pk.pCMEclose = LineaCME.getY((DateTime)dia);
                                Pk.pCMEproy = LineaCME.getY(((DateTime)dia).AddDays(1));
                                _7G.tpikitosT.Add(Pk);

                            }
                            else
                            {
                                ;
                                pi.pCMEclose = LineaCME.getY((DateTime)dia);
                                pi.pCMEproy = LineaCME.getY(((DateTime)dia).AddDays(1));
                                _7G.Entry(pi).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        catch { }
                    }
                    _7G.SaveChanges();
                    ma.Dispatcher.Invoke(() =>
                    {
                        //Creo las Variables de la Consola de Resultados (RichTextBox)            
                        con.escribirLinea("Pikitos Calculados Pikitos ..");
                    });
                }

            });
        }
    }
}
