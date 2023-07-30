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

namespace _7Gplus.Vistas.Contenedores
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml    
    /// </summary>
    public partial class CotizacionSpot : UserControl
    {
        public MainWindow m { get; set; }
        public Consola con { get; set; }
        public string ConnectionStrings = ConfigurationManager.ConnectionStrings["Bd_7G"].ConnectionString;



        public CotizacionSpot()
        {
            InitializeComponent();
            m = (MainWindow)System.Windows.Application.Current.MainWindow;
            con = new Consola();
            ActualizaTablero();
            InstanceDependency();
        }

        private async void InstanceDependency()
        {
            SqlDependency.Stop(ConnectionStrings);
            SqlDependency.Start(ConnectionStrings);
            string sQuery = "SELECT [Close],[Change] FROM [dbo].[tCotizacionSpot];"; //"SELECT [id],[Close],[Change] FROM [dbo].[tCotizacionSpot];"
            SqlConnection sqlConnection = new SqlConnection(ConnectionStrings);
            SqlCommand sqlCommand = new SqlCommand(sQuery, sqlConnection);
            sqlCommand.CommandType = CommandType.Text;
            SqlDependency sqlDependency = new SqlDependency(sqlCommand);
            sqlDependency.OnChange += SqlDependencyOnChange;
            await sqlCommand.Connection.OpenAsync();
            await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }


        private void SqlDependencyOnChange(object sender, SqlNotificationEventArgs eventArgs) //Cuando se Registre una Nueva Cotizacion en BD

        {
            if (eventArgs.Info == SqlNotificationInfo.Invalid)

            {
                m.Dispatcher.Invoke(() =>
                {
                    con.escribirLinea("The query is not valid." + eventArgs.Type);
                });
            }
            else

            {
                m.Dispatcher.Invoke(() =>
                {
                    con.escribirLinea("Your data was changed!");
                });
                try
                {
                    ActualizaTablero();
                }
                catch (Exception e)
                {
                    m.Dispatcher.Invoke(() =>
                    {
                        con.escribirLinea(e.Message.ToString());
                    });
                }
            }

            InstanceDependency();
        }


        private async Task  ActualizaTablero()
        {
            await Task.Factory.StartNew(() =>
            {
                using (GEntities _7G = new GEntities())
                {
                    DateTime d   = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    DateTime u1d  = d.AddDays(-1);
                    DateTime u1M  = d.AddDays(-30);
                    DateTime u1Y = d.AddDays(-360);
                    DateTime u5Y = d.AddDays(-1800);

                    var c = _7G.tCotizacionSpot.OrderByDescending(x => x.date);
                    var cot = c.FirstOrDefault();
                    var m30volUSD = c.Where(x => x.date >= u1M && x.date <= u1d).Select( x => x.VolUSD).Average(x => x.Value);
                    var m30volEUR = c.Where(x => x.date >= u1M && x.date <= u1d).Select(x => x.VolEUR).Average(x => x.Value);
                    var dif = c.FirstOrDefault().Close - c.FirstOrDefault().Open;

                    m.Dispatcher.Invoke(() =>
                    {
                        if ( dif > 0)
                        {
                            Close.Foreground = Brushes.Green;
                            Close.Opacity = 0.7;
                        }
                        else
                        {
                            Close.Foreground = Brushes.Red;
                            Close.Opacity = 0.7;
                        }
                        cCotizaciones.DataContext = cot;
                        volP30EUR.Text = m30volEUR.ToString("N2");
                        volP30USD.Text = m30volUSD.ToString("N2");


                        dvolP30EUR.Text = (((((m30volEUR / m30volUSD))/((double)(cot.VolEUR / cot.VolUSD)))-1)).ToString("P4");

                        if ((((((m30volEUR / m30volUSD)) / ((double)(cot.VolEUR / cot.VolUSD))) - 1)) > 0)
                        {
                            dvolP30EUR.Foreground = Brushes.Green;
                            dvolP30EUR.Opacity = 0.7;
                        }
                        else
                        {
                            dvolP30EUR.Foreground = Brushes.Red;
                            dvolP30EUR.Opacity = 0.7;
                        }


                    });
                }
            });


        }
    }
}
