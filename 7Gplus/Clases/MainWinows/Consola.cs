using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace _7Gplus.Clases.MainWinows
{
    public class Consola
    {
        public MainWindow m { get; set; }


        public Consola()
        {
            m = (MainWindow)System.Windows.Application.Current.MainWindow;
        }

        public void escribirLinea(string linea)
        {
            m.Dispatcher.Invoke(() =>
            {
                Run InRun = new Run(linea);
                Paragraph pIni = new Paragraph();
                pIni.Inlines.Add(InRun);
                m.FlowDocConsola.Blocks.Add(pIni);
                m.richText.Document = m.FlowDocConsola;
            });
        }
    }
}
