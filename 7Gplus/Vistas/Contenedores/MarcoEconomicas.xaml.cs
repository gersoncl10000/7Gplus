using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using _7Gplus.Clases.MainWinows;
using System.Windows.Threading;
using _7Gplus.Clases.Calculos;

namespace _7Gplus.Vistas.Contenedores
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class MacroEconomicas : UserControl
    {
        public MainWindow m { get; set; }
        public Consola con { get; set; }
        public string ConnectionStrings = ConfigurationManager.ConnectionStrings["Bd_7G"].ConnectionString;



        public MacroEconomicas()
        {
            InitializeComponent();
            m = (MainWindow)System.Windows.Application.Current.MainWindow;
            con = new Consola();

            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();

            ActualizaTablero();
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ActualizaTablero();
        }

        private async Task  ActualizaTablero()
        {
            await Task.Factory.StartNew(() =>
            {
                using (GEntities _7G = new GEntities())
                {
                    proyecionPPA pPPA = new proyecionPPA();

                    
                    var _tcPPAact = DateTime.Now.ToString();
                    var _tcPPA = string.Format("{0:N6}", pPPA.getTipoCambioPPAestimado(DateTime.Now.AddDays(-1), DateTime.Now));
                    var _tcPPApy = string.Format("{0:N6}", pPPA.getTipoCambioPPAestimado(DateTime.Now.AddDays(-1), DateTime.Now.AddYears(1)));
                    DateTime lastMonthYearUS = ((DateTime)_7G.tCPIus.OrderByDescending(x => x.date).FirstOrDefault().date).AddMonths(-12);
                    DateTime lastMonthYearEU = ((DateTime)_7G.tCPIeux.OrderByDescending(x => x.date).FirstOrDefault().date).AddMonths(-12);
                    var _infUSA = string.Format("{0:P3}", (_7G.tCPIus.OrderByDescending(x => x.date).FirstOrDefault().value / _7G.tCPIus.Where(x => x.date == lastMonthYearUS).Select(x => x.value).FirstOrDefault()) - 1); //
                    var _finfUSA = string.Format("{0:dd/MM/yyyy}", (_7G.tCPIus.OrderByDescending(x => x.date).FirstOrDefault().date )); //
                    var _difInfUSA = (((_7G.tCPIus.OrderByDescending(x => x.date).FirstOrDefault().value / _7G.tCPIus.OrderByDescending(x => x.date).Skip(1).FirstOrDefault().value) - 1) - ((_7G.tCPIus.OrderByDescending(x => x.date).Skip(1).FirstOrDefault().value / _7G.tCPIus.OrderByDescending(x => x.date).Skip(2).FirstOrDefault().value) - 1));
                    var _infEU = string.Format("{0:P3}", (_7G.tCPIeux.OrderByDescending(x => x.date).FirstOrDefault().value / _7G.tCPIeux.Where(x => x.date == lastMonthYearUS).Select(x => x.value).FirstOrDefault()) - 1); //
                    var _finfEU = string.Format("{0:dd/MM/yyyy}", (_7G.tCPIeux.OrderByDescending(x => x.date).FirstOrDefault().date )); //
                    var _difInfEU = (((_7G.tCPIeux.OrderByDescending(x => x.date).FirstOrDefault().value / _7G.tCPIeux.OrderByDescending(x => x.date).Skip(1).FirstOrDefault().value) - 1) - ((_7G.tCPIeux.OrderByDescending(x => x.date).Skip(1).FirstOrDefault().value / _7G.tCPIeux.OrderByDescending(x => x.date).Skip(2).FirstOrDefault().value) - 1));
                    var _difInf = string.Format("{0:P3}", ((_7G.tCPIus.OrderByDescending(x => x.date).FirstOrDefault().value / _7G.tCPIus.Where(x => x.date == lastMonthYearUS).Select(x => x.value).FirstOrDefault())/(_7G.tCPIeux.OrderByDescending(x => x.date).FirstOrDefault().value / _7G.tCPIeux.Where(x => x.date == lastMonthYearUS).Select(x => x.value).FirstOrDefault()))-1);
                    var _ndifInf = ((_7G.tCPIus.OrderByDescending(x => x.date).FirstOrDefault().value / _7G.tCPIus.Where(x => x.date == lastMonthYearUS).Select(x => x.value).FirstOrDefault()) / (_7G.tCPIeux.OrderByDescending(x => x.date).FirstOrDefault().value / _7G.tCPIeux.Where(x => x.date == lastMonthYearUS).Select(x => x.value).FirstOrDefault())) - 1;
                    var _yieldUS10Y = string.Format("{0:P3}", (_7G.tbondUS10Y.OrderByDescending(x => x.date).FirstOrDefault().value/100)); //
                    var _fyieldUS10Y = string.Format("{0:dd/MM/yyyy}", (_7G.tbondUS10Y.OrderByDescending(x => x.date).FirstOrDefault().date)); //
                    var _difyieldUS10Y = (_7G.tbondUS10Y.OrderByDescending(x => x.date).FirstOrDefault().value / 100)-(_7G.tbondUS10Y.OrderByDescending(x => x.date).Skip(1).FirstOrDefault().value / 100); //
                    var _yieldEU10Y = string.Format("{0:P3}", (_7G.tbondEU10Y.OrderByDescending(x => x.date).FirstOrDefault().value / 100)); //
                    var _fyieldEU10Y = string.Format("{0:dd/MM/yyyy}", (_7G.tbondEU10Y.OrderByDescending(x => x.date).FirstOrDefault().date)); //
                    var _difyieldEU10Y = (_7G.tbondEU10Y.OrderByDescending(x => x.date).FirstOrDefault().value / 100)- (_7G.tbondEU10Y.OrderByDescending(x => x.date).Skip(1).FirstOrDefault().value / 100); //
                    var _difYieldUSEU10Y = string.Format("{0:P3}", (1 + (_7G.tbondUS10Y.OrderByDescending(x => x.date).FirstOrDefault().value / 100)) / (1 + (_7G.tbondEU10Y.OrderByDescending(x => x.date).FirstOrDefault().value / 100))-1);
                    var _ndifYieldUSEU10Y = (1 + (_7G.tbondUS10Y.OrderByDescending(x => x.date).FirstOrDefault().value / 100)) / (1 + (_7G.tbondEU10Y.OrderByDescending(x => x.date).FirstOrDefault().value / 100)) - 1;

                    this.Dispatcher.Invoke(() =>
                    {
                        tcPPAact.Text = _tcPPAact;
                        tcPPA.Text = _tcPPA;

                        if (_difInfUSA > 0)
                        {
                            infUSA.Foreground = Brushes.Green;
                            infUSA.Opacity = 0.5;
                        }
                        else
                        {
                            infUSA.Foreground = Brushes.Red;
                            infUSA.Opacity = 0.5;
                        }

                        infUSA.Text = _infUSA; //
                        finfUSA.Text = _finfUSA; //

                        if (_difInfEU > 0)
                        {
                            infEU.Foreground = Brushes.Green;
                            infEU.Opacity = 0.5;
                        }
                        else
                        {
                            infEU.Foreground = Brushes.Red;
                            infEU.Opacity = 0.5;
                        }

                        infEU.Text = _infEU;
                        finfEU.Text = _finfEU;
                        if (_ndifInf > 0)
                        {
                            difInf.Foreground = Brushes.Green;
                            difInf.Opacity = 0.7;
                        }
                        else
                        {
                            difInf.Foreground = Brushes.Red;
                            difInf.Opacity = 0.7;
                        }
                        difInf.Text = _difInf;
                        tcPPApy.Text = _tcPPApy;

                        if (_difyieldUS10Y > 0)
                        {
                            yieldUS10Y.Foreground = Brushes.Green;
                            yieldUS10Y.Opacity = 0.5;
                        }
                        else
                        {
                            yieldUS10Y.Foreground = Brushes.Red;
                            yieldUS10Y.Opacity = 0.5;
                        }
                        yieldUS10Y.Text = _yieldUS10Y;
                        fyieldUS10Y.Text = _fyieldUS10Y;

                        if (_difyieldEU10Y > 0)
                        {
                            yieldEU10Y.Foreground = Brushes.Green;
                            yieldEU10Y.Opacity = 0.5;
                        }
                        else
                        {
                            yieldEU10Y.Foreground = Brushes.Red;
                            yieldEU10Y.Opacity = 0.5;
                        }
                        yieldEU10Y.Text = _yieldEU10Y;
                        fyieldEU10Y.Text = _fyieldEU10Y;

                        if (_ndifYieldUSEU10Y > 0)
                        {
                            difYieldUSEU10Y.Foreground = Brushes.Green;
                            difYieldUSEU10Y.Opacity = 0.7;
                        }
                        else
                        {
                            difYieldUSEU10Y.Foreground = Brushes.Red;
                            difYieldUSEU10Y.Opacity = 0.7;
                        }

                        difYieldUSEU10Y.Text = _difYieldUSEU10Y;

                    });
                        



                }
            });


        }
    }
}
