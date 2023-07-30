using _7Gplus.Clases.MainWinows;
using _7Gplus.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace _7Gplus.Clases.Procesos
{
    public class eurofxquotesglobex
    {
        public MainWindow m { get; set; }
        public Consola con { get; set; }

        public eurofxquotesglobex()
        {
            m = (MainWindow)System.Windows.Application.Current.MainWindow;
            con = new Consola();
        }

        public void iniciar()
        {


            m.Dispatcher.Invoke(() =>
            {







            });
        }


    }
}
