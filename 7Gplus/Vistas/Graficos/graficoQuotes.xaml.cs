using _7Gplus.Clases.Calculos;
using _7Gplus.Clases.Procesos;
using _7Gplus.Modelo;
using LiveCharts;
using LiveCharts.Defaults;
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
    public partial class graficoQuotes : UserControl
    {
        private MainWindow m { get; set; }


        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SerieDiferenciaPSvsLAst { get; set; }
        public String[] Labels { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public graficoQuotes()
        {
            InitializeComponent();
            m = (MainWindow)System.Windows.Application.Current.MainWindow;
            proceso();
        }

        private async void proceso()
        {
            await Task.Run(() =>
            {
                ChartValues<double> priorSettle; // Valores de Y
                ChartValues<double> last; // Valores de Y
                ChartValues<double> last2; // Valores de Y
                ChartValues<double> last3; // Valores de Y
                ChartValues<double> last4; // Valores de Y
                ChartValues<double> last5; // Valores de Y
                ChartValues<double> last6; // Valores de Y
                ChartValues<double> last7; // Valores de Y
                ChartValues<double> close; // Valores de Y
                ChartValues<double> open; // Valores de Y
                ChartValues<double> difPSvsLast; // Valores de Y
                ChartValues<double> difPSvsLast2; // Valores de Y
                ChartValues<double> difPSvsLast3; // Valores de Y
                ChartValues<double> difPSvsLast4; // Valores de Y
                ChartValues<double> difPSvsLast5; // Valores de Y
                ChartValues<double> difPSvsLast6; // Valores de Y
                ChartValues<double> difPSvsLast7; // Valores de Y


                ObservableCollection<DateTime> l = new ObservableCollection<DateTime>(); //Labes



                cLineaDeTendencia clPriorSettle;
                cLineaDeTendencia clLast;
                cLineaDeTendencia clLast2;
                cLineaDeTendencia clLast3;
                cLineaDeTendencia clLast4;
                cLineaDeTendencia clLast5;
                cLineaDeTendencia clLast6;
                cLineaDeTendencia clLast7;
                DateTime date = DateTime.Now;
                List<lineChartData> dPriorSettle = new List<lineChartData>();
                List<lineChartData> dLast = new List<lineChartData>();
                List<lineChartData> dLast2 = new List<lineChartData>();
                List<lineChartData> dLast3 = new List<lineChartData>();
                List<lineChartData> dLast4 = new List<lineChartData>();
                List<lineChartData> dLast5 = new List<lineChartData>();
                List<lineChartData> dLast6 = new List<lineChartData>();
                List<lineChartData> dLast7 = new List<lineChartData>();
                List<lineChartData> dDifPSvsLast = new List<lineChartData>();
                List<lineChartData> dDifPSvsLast2 = new List<lineChartData>();
                List<lineChartData> dDifPSvsLast3 = new List<lineChartData>();
                List<lineChartData> dDifPSvsLast4 = new List<lineChartData>();
                List<lineChartData> dDifPSvsLast5 = new List<lineChartData>();
                List<lineChartData> dDifPSvsLast6 = new List<lineChartData>();
                List<lineChartData> dDifPSvsLast7 = new List<lineChartData>();
                
                tCotizacionSpot tCspot;

                for (int i = 0; i < 180; i++)
                {
                    l.Add(date.AddDays(i));
                }


                #region//--------------------------------- 1er Grafico ------------------------------------

                using (GEntities b = new GEntities())
                {
                    DateTime? hoy = b.tFuturosCMEquotes.Select(x => x.date).OrderByDescending(x => x.Value).FirstOrDefault();
                    DateTime mesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTime? UltimaActCME = b.tFuturosCMEquotes.Where(x => x.date == hoy && x.Last != 0 && x.Month >= mesActual && x.Updated != null).Select(x => x.Updated).OrderByDescending(x => x.Value).FirstOrDefault();

                    List<oListaLineaTendenciaTiempo> li;
                    // PriorSettle
                    li = b.tFuturosCMEquotes.Where(x => x.date == hoy && x.PriorSettle != 0 && x.Month >= mesActual).OrderBy(x => x.Month).Select(x => new oListaLineaTendenciaTiempo { x = ((DateTime)x.Month), y = (double)x.PriorSettle }).ToList();
                    li.ForEach(x => x.x = x.x.AddMonths(1).AddDays(-1));
                    clPriorSettle = new cLineaDeTendencia(li);
                    foreach (var d in l)
                    {
                        double Y = (double)clPriorSettle.getY(d);
                        dPriorSettle.Add(new lineChartData { x = d, y = Y });
                    }
                    
                    RegistrarLineadeTendeciaEnBaseDeDatos(dPriorSettle, "PriorSettle", UltimaActCME);

                    // Last 
                    li = b.tFuturosCMEquotes.Where(x => x.date == hoy && x.Last != 0 && x.Month >= mesActual).Select(x => new oListaLineaTendenciaTiempo { x = ((DateTime)x.Month), y = (double)x.Last }).ToList();
                    li.ForEach(x => x.x = x.x.AddMonths(1).AddDays(-1));
                    clLast = new cLineaDeTendencia(li);
                    foreach (var d in l)
                    {
                        double Y = (double)clLast.getY(d);
                        dLast.Add(new lineChartData { x = d, y = Y });
                    }
                    RegistrarLineadeTendeciaEnBaseDeDatos(dLast, "Last", UltimaActCME);
                    // Last 2 
                    li = b.tFuturosCMEquotes.Where(x => x.date == hoy && x.Last != 0 && x.Month >= mesActual).OrderByDescending(x => x.Updated).Take(2).Select(x => new oListaLineaTendenciaTiempo { x = ((DateTime)x.Month), y = (double)x.Last }).ToList();
                    li.ForEach(x => x.x = x.x.AddMonths(1).AddDays(-1));
                    clLast2 = new cLineaDeTendencia(li);
                    foreach (var d in l)
                    {
                        double Y = (double)clLast2.getY(d);
                        dLast2.Add(new lineChartData { x = d, y = Y });
                    }
                    RegistrarLineadeTendeciaEnBaseDeDatos(dLast2, "Last2", UltimaActCME);
                    // Last 3 
                    li = b.tFuturosCMEquotes.Where(x => x.date == hoy && x.Last != 0 && x.Month >= mesActual).OrderByDescending(x => x.Updated).Take(3).Select(x => new oListaLineaTendenciaTiempo { x = ((DateTime)x.Month), y = (double)x.Last }).ToList();
                    li.ForEach(x => x.x = x.x.AddMonths(1).AddDays(-1));
                    clLast3 = new cLineaDeTendencia(li);
                    foreach (var d in l)
                    {
                        double Y = (double)clLast3.getY(d);
                        dLast3.Add(new lineChartData { x = d, y = Y });
                    }
                    RegistrarLineadeTendeciaEnBaseDeDatos(dLast3, "Last3", UltimaActCME);
                    // Last 4
                    li = b.tFuturosCMEquotes.Where(x => x.date == hoy && x.Last != 0 && x.Month >= mesActual).OrderByDescending(x => x.Updated).Take(4).Select(x => new oListaLineaTendenciaTiempo { x = ((DateTime)x.Month), y = (double)x.Last }).ToList();
                    li.ForEach(x => x.x = x.x.AddMonths(1).AddDays(-1));
                    clLast4 = new cLineaDeTendencia(li);
                    foreach (var d in l)
                    {
                        double Y = (double)clLast4.getY(d);
                        dLast4.Add(new lineChartData { x = d, y = Y });
                    }
                    RegistrarLineadeTendeciaEnBaseDeDatos(dLast4, "Last4", UltimaActCME);
                    // Last 5 
                    li = b.tFuturosCMEquotes.Where(x => x.date == hoy && x.Last != 0 && x.Month >= mesActual).OrderByDescending(x => x.Updated).Take(5).Select(x => new oListaLineaTendenciaTiempo { x = ((DateTime)x.Month), y = (double)x.Last }).ToList();
                    li.ForEach(x => x.x = x.x.AddMonths(1).AddDays(-1));
                    clLast5 = new cLineaDeTendencia(li);
                    foreach (var d in l)
                    {
                        double Y = (double)clLast5.getY(d);
                        dLast5.Add(new lineChartData { x = d, y = Y });
                    }
                    RegistrarLineadeTendeciaEnBaseDeDatos(dLast5, "Last5", UltimaActCME);
                    // Last 6 
                    li = b.tFuturosCMEquotes.Where(x => x.date == hoy && x.Last != 0 && x.Month >= mesActual).OrderByDescending(x => x.Updated).Take(6).Select(x => new oListaLineaTendenciaTiempo { x = ((DateTime)x.Month), y = (double)x.Last }).ToList();
                    li.ForEach(x => x.x = x.x.AddMonths(1).AddDays(-1));
                    clLast6 = new cLineaDeTendencia(li);
                    foreach (var d in l)
                    {
                        double Y = (double)clLast6.getY(d);
                        dLast6.Add(new lineChartData { x = d, y = Y });
                    }
                    RegistrarLineadeTendeciaEnBaseDeDatos(dLast6, "Last6", UltimaActCME);
                    // Last 7 
                    li = b.tFuturosCMEquotes.Where(x => x.date == hoy && x.Last != 0 && x.Month >= mesActual).OrderByDescending(x => x.Updated).Take(7).Select(x => new oListaLineaTendenciaTiempo { x = ((DateTime)x.Month), y = (double)x.Last }).ToList();
                    li.ForEach(x => x.x = x.x.AddMonths(1).AddDays(-1));
                    clLast7 = new cLineaDeTendencia(li);
                    foreach (var d in l)
                    {
                        double Y = (double)clLast7.getY(d);
                        dLast7.Add(new lineChartData { x = d, y = Y });
                    }
                    RegistrarLineadeTendeciaEnBaseDeDatos(dLast7, "Last7", UltimaActCME);
                    
                    // Cotizacion Spot 
                    tCspot = b.tCotizacionSpot.OrderByDescending(x => x.date).FirstOrDefault();

                    //Ultima Actualizacion de la Data
                    this.Dispatcher.Invoke(() =>
                    {
                        txblActtualizacion.Text = DateTime.Now.ToString();
                        txblActtualizacionCME.Text = b.tFuturosCMEquotes.Where(x => x.date == hoy && x.Last != 0 && x.Month >= mesActual && x.Updated != null).Select(x => x.Updated).OrderByDescending(x => x.Value).FirstOrDefault().ToString();
                    });
                }



                priorSettle = dPriorSettle.Select(x => x.y ).AsChartValues();
                last = dLast.Select(x => x.y ).AsChartValues();
                last2 = dLast2.Select(x => x.y).AsChartValues();
                last3 = dLast3.Select(x => x.y).AsChartValues();
                last4 = dLast4.Select(x => x.y).AsChartValues();
                last5 = dLast5.Select(x => x.y).AsChartValues();
                last6 = dLast6.Select(x => x.y).AsChartValues();
                last7 = dLast7.Select(x => x.y).AsChartValues();
                close = new ChartValues<double> { tCspot.Close };
                open = new ChartValues<double> { (double)tCspot.Open };

                this.Dispatcher.Invoke(() =>
                {



                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "PriorSettle",
                        Values = priorSettle,
                        PointGeometry = null,

                        Fill = Brushes.Transparent
                    },
                new LineSeries
                {
                    Title = "Last",
                    Values = last,
                    PointGeometry = null,
                    Fill = Brushes.Transparent,


                },
                new LineSeries
                {
                    Title = "Last2",
                    Values = last2,
                    PointGeometry = null,
                    Fill = Brushes.Transparent,
                    Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 1 }




                },
                new LineSeries
                {
                    Title = "Last3",
                    Values = last3,
                    PointGeometry = null,
                    Fill = Brushes.Transparent,
                    Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 0.70 }


                },
                new LineSeries
                {
                    Title = "Last4",
                    Values = last4,
                    PointGeometry = null,
                    Fill = Brushes.Transparent,
                    Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 0.40 }

                },
                new LineSeries
                {
                    Title = "Last5",
                    Values = last5,
                    PointGeometry = null,
                    Fill = Brushes.Transparent,
                    Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 0.20 }

                },
                new LineSeries
                {
                    Title = "Last6",
                    Values = last6,
                    PointGeometry = null,
                    Fill = Brushes.Transparent,
                    Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 0.10 }

                },
                new LineSeries
                {
                    Title = "Last7",
                    Values = last7,
                    PointGeometry = null,
                    Fill = Brushes.Transparent,
                    Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 0.05 }

                },
                new OhlcSeries()
                {
                    Title = "Cot Spot",
                    Values = new ChartValues<OhlcPoint>
                    {
                        new OhlcPoint((double)tCspot.Open, (double)tCspot.High, (double)tCspot.Low, (double)tCspot.Close)
                    }

                },
                new LineSeries
                {
                    Title = "Close",
                    Values = close,
                    PointForeground = Brushes.LightBlue,
                    Foreground = Brushes.Transparent,
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "Open",
                    Values = open,
                    PointForeground = Brushes.CadetBlue,
                    Fill = Brushes.Transparent
                }
                };




                });

                #endregion

                #region//--------------------------------- 2do Grafico ------------------------------------

                // Last 
                foreach (var d in l)
                {
                    double Y = (dLast.Where(x => x.x == d).Select(x => x.y).FirstOrDefault()/dPriorSettle.Where(x => x.x == d).Select(x => x.y).FirstOrDefault())-1;
                    dDifPSvsLast.Add(new lineChartData { x = d, y = Y });
                    

                }

                difPSvsLast = dDifPSvsLast.Select(x => x.y).AsChartValues();
                
                // Last2 
                foreach (var d in l)
                {
                    double Y = (dLast2.Where(x => x.x == d).Select(x => x.y).FirstOrDefault() / dPriorSettle.Where(x => x.x == d).Select(x => x.y).FirstOrDefault()) - 1;
                    dDifPSvsLast2.Add(new lineChartData { x = d, y = Y });

                }

                difPSvsLast2 = dDifPSvsLast2.Select(x => x.y).AsChartValues();

                // Last3 
                foreach (var d in l)
                {
                    double Y = (dLast3.Where(x => x.x == d).Select(x => x.y).FirstOrDefault() / dPriorSettle.Where(x => x.x == d).Select(x => x.y).FirstOrDefault()) - 1;
                    dDifPSvsLast3.Add(new lineChartData { x = d, y = Y });

                }

                difPSvsLast3 = dDifPSvsLast3.Select(x => x.y).AsChartValues();

                // Last4 
                foreach (var d in l)
                {
                    double Y = (dLast4.Where(x => x.x == d).Select(x => x.y).FirstOrDefault() / dPriorSettle.Where(x => x.x == d).Select(x => x.y).FirstOrDefault()) - 1;
                    dDifPSvsLast4.Add(new lineChartData { x = d, y = Y });

                }

                difPSvsLast4 = dDifPSvsLast4.Select(x => x.y).AsChartValues();

                // Last5
                foreach (var d in l)
                {
                    double Y = (dLast5.Where(x => x.x == d).Select(x => x.y).FirstOrDefault() / dPriorSettle.Where(x => x.x == d).Select(x => x.y).FirstOrDefault()) - 1;
                    dDifPSvsLast5.Add(new lineChartData { x = d, y = Y });

                }

                difPSvsLast5 = dDifPSvsLast5.Select(x => x.y).AsChartValues();

                // Last6
                foreach (var d in l)
                {
                    double Y = (dLast6.Where(x => x.x == d).Select(x => x.y).FirstOrDefault() / dPriorSettle.Where(x => x.x == d).Select(x => x.y).FirstOrDefault()) - 1;
                    dDifPSvsLast6.Add(new lineChartData { x = d, y = Y });

                }

                difPSvsLast6 = dDifPSvsLast6.Select(x => x.y).AsChartValues();

                // Last7 
                foreach (var d in l)
                {
                    double Y = (dLast7.Where(x => x.x == d).Select(x => x.y).FirstOrDefault() / dPriorSettle.Where(x => x.x == d).Select(x => x.y).FirstOrDefault()) - 1;
                    dDifPSvsLast7.Add(new lineChartData { x = d, y = Y });

                }

                difPSvsLast7 = dDifPSvsLast7.Select(x => x.y).AsChartValues();



                this.Dispatcher.Invoke(() =>
                {
                SerieDiferenciaPSvsLAst = new SeriesCollection
                {
                        new LineSeries
                        {
                            Title = "Dif. Last vs PriorSettle ",
                            Values = difPSvsLast,
                            PointGeometry = null,

                            Fill = Brushes.Transparent
                        },
                        new LineSeries
                        {
                            Title = "Dif. Last vs PriorSettle 2",
                            Values = difPSvsLast2,
                            Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 1 },
                            PointGeometry = null,
                            Fill = Brushes.Transparent
                        },
                        new LineSeries
                        {
                            Title = "Dif. Last vs PriorSettle 3",
                            Values = difPSvsLast3,
                            Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 0.7 },
                            PointGeometry = null,
                            Fill = Brushes.Transparent
                        },
                        new LineSeries
                        {
                            Title = "Dif. Last vs PriorSettle 4",
                            Values = difPSvsLast4,
                            Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 0.5 },
                            PointGeometry = null,
                            Fill = Brushes.Transparent
                        },
                        new LineSeries
                        {
                            Title = "Dif. Last vs PriorSettle 5",
                            Values = difPSvsLast5,
                            Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 0.2 },
                            PointGeometry = null,
                            Fill = Brushes.Transparent
                        },
                        new LineSeries
                        {
                            Title = "Dif. Last vs PriorSettle 6",
                            Values = difPSvsLast6,
                            Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 0.1 },
                            PointGeometry = null,
                            Fill = Brushes.Transparent
                        },
                        new LineSeries
                        {
                            Title = "Dif. Last vs PriorSettle 7",
                            Values = difPSvsLast7,
                            Stroke = new SolidColorBrush{ Color = Color.FromRgb(255,137,0) ,Opacity = 0.05 },
                            PointGeometry = null,
                            Fill = Brushes.Transparent
                        }
                    };

                });


                Labels = l.Select(x => x.ToString("yyyy-MM-dd")).ToArray();
                YFormatter = value => value.ToString("N6");

                #endregion


                this.Dispatcher.Invoke(() =>
               {
                   
                   DataContext = this;
               });
                


            });
        }

        public async void RegistrarLineadeTendeciaEnBaseDeDatos(List<lineChartData> linechart, string Tipo, DateTime? UltimaActCME)
        {
            using (GEntities b = new GEntities())
            {
                int cant = 0;
                foreach (var line in linechart.OrderBy(x => x.x))
                {
                    var Line = b.LineaTendeciaFuturos.Where(x => x.x == cant && x.Tipo == Tipo).FirstOrDefault();
                    if(Line == null)
                    {
                        LineaTendeciaFuturos nLine = new LineaTendeciaFuturos();
                        nLine.x = cant;
                        nLine.y = line.y;
                        nLine.Tipo = Tipo;
                        nLine.ActtualizacionCME = UltimaActCME;
                        nLine.Acttualizacion = DateTime.Now;
                        b.LineaTendeciaFuturos.Add(nLine);
                    }
                    else
                    {
                        Line.x = cant;
                        Line.y = line.y;
                        Line.Tipo = Tipo;
                        Line.ActtualizacionCME = UltimaActCME;
                        Line.Acttualizacion = DateTime.Now;
                        b.Entry(Line).State = System.Data.Entity.EntityState.Modified;
                        
                    }
                    cant++;
                }
                await b.SaveChangesAsync();
            }
        }

    }
}

