using _7Gplus.Clases.Procesos;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _7Gplus.Vistas.Graficos
{
    /// <summary>
    /// Lógica de interacción para graficoPPA.xaml
    /// </summary>
    public partial class graficoPPA : UserControl
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public graficoPPA()
        {
            InitializeComponent();

            ChartValues<double> v;
            ChartValues<double> eurusd;
            ObservableCollection<string> l;


            pData data = new pData();
            data.cargarDatos();

            v = data.lData.Select(x => x.PPA??0).AsChartValues();
            eurusd = data.lData.Select(x => x.Value??0).AsChartValues();
            l = new ObservableCollection<string>(data.lData.Select(x => x.Date.ToString("dd-MM-yy")));



            SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "PPA EUR/USD",
                        Values = v,
                        PointGeometry = null
                    },
                    new LineSeries
                    {
                        Title = "EUR/USD Spot",
                        Values = eurusd,
                        PointGeometry = null,
                         

                    },




                };

            Labels = l.ToArray();
            YFormatter = value => value.ToString("N6");



            DataContext = this;
        }
    }
}
