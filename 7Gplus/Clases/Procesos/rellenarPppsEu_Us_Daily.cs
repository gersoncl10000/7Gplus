using _7Gplus.Clases.Calculos;
using _7Gplus.Clases.MainWinows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Gplus.Clases.Procesos
{


    public class rellenarPppsEu_Us_Daily
    {
        public MainWindow m { get; set; }
        public Consola con { get; set; }



        public rellenarPppsEu_Us_Daily()
        {
            m = (MainWindow)System.Windows.Application.Current.MainWindow;
            con = new Consola();
        }
        public void rellenaTabla()
        {
                System.Threading.Thread rell = new System.Threading.Thread(rellenar);
                rell.Start();
        }





        private void rellenar()
        {
            DateTime desde = DateTime.Now.AddYears(-14);
            DateTime hasta = DateTime.Now;


            using (GEntities _7G = new GEntities())
            {
                if ( desde < hasta )
                {
                    proyecionPPA proyPPA = new proyecionPPA();
                    
                    var count = (int)(hasta.ToOADate() - desde.ToOADate());
                    for ( int i =0;i<=count;i++)
                    {
                        DateTime ndesde = desde.AddDays(i);
                        var ppa = _7G.tPppsEuroUsdDaily.Where(x => x.Date == ndesde).SingleOrDefault();
                        if (ppa == null)
                        {
                            tPppsEuroUsdDaily ppad = new tPppsEuroUsdDaily();
                            ppad.Date = ndesde;
                            ppad.Value = proyPPA.getTipoCambioPPAestimado(ndesde, ndesde);
                            _7G.tPppsEuroUsdDaily.Add(ppad);
                        }
                        else
                        {
                            ppa.Value = proyPPA.getTipoCambioPPAestimado(ndesde, ndesde);
                            _7G.Entry(ppa).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    _7G.SaveChanges();
                    con.escribirLinea("Proyeccion del Tc por PPA Generada");
                }
                else
                {
                    con.escribirLinea("La fecha desde no puede ser mayor a la fecha haste");
                }
            }
        }
    }
}
