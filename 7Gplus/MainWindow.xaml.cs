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
using _7Gplus.Clases.CME;
using _7Gplus.Clases.Calculos;
using _7Gplus.Modelo;
using System.Xml.Linq;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Threading;
using System.Reflection;
using _7Gplus.Clases.Descargas;
using _7Gplus.Vistas.Graficos;
using _7Gplus.Vistas.Contenedores;
using _7Gplus.Clases.Procesos;

namespace _7Gplus
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Creo el Flow Documente
        public FlowDocument FlowDocConsola;
        //Variables Globales
        public int vDescargandoFuturos = 0;

        public MainWindow()
        {
            InitializeComponent();

            //visorCotizaciones.Content = new CotizacionSpot();
            //visorGraficos.Content     = new graficoQuotes();
            //visorMacroEconomicas.Content = new MacroEconomicas();

            FlowDocConsola = new FlowDocument();

 
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0,3,0);
            dispatcherTimer.Start();


            DispatcherTimer dispatcherTimer6h = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer6h.Tick += new EventHandler(dispatcherTimer_Tick12h);
            dispatcherTimer6h.Interval = new TimeSpan(12,0, 0);
            dispatcherTimer6h.Start();

            descargar12h();
            Actualizar_optionCMEDataGrid();
            Actualizar_futurosDataGrid();
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            visorGraficos.Content = new graficoQuotes();
            Actualizar_optionCMEDataGrid();
            Actualizar_futurosDataGrid();
        }

        private void dispatcherTimer_Tick12h(object sender, EventArgs e)
        {
            descargar12h();
        }

        public async Task Actualizar_optionCMEDataGrid() //Rellena el CME Data Grid con los Valalores del Mercado de Futuros del CME 
        {
            await Task.Factory.StartNew(() =>
            {
                using (GEntities _7G = new GEntities())
                {
                    //try
                    //{

                        DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        var h = (DateTime)_7G.tOpcionesCMEquotes.Select(x => x.date).OrderByDescending(x => x.Value).FirstOrDefault();
                        //var fd = (DateTime)_7G.tFuturosCMEquotes.Select(x => x.date).OrderByDescending(x => x.Value).FirstOrDefault();
                        var mes = _7G.tOpcionesCMEquotes.Where(x => x.date == h).Min( x => x.Month).Value.ToString("dd/MM/yyyy");
                        string hoy = h.ToString("dd/MM/yyyy");
                        //string fdate = fd.ToString("dd/MM/yyyy");

                        double rango = 0.0005;


                        List<oOptionCMEDataGrid> d = _7G.Database.SqlQuery<oOptionCMEDataGrid>(string.Format(@"

                    declare @dia  date; -- Dia actual
                    declare @MonthOp  date; -- Mes Opciones
                    declare @rango  float; -- Mes Opciones


                    set @dia = '{2}';
                    set @MonthOp = '{1}';
                    set @rango = {0};


                    select 

                    c.Month,c.StrikePrice, f.Last as fLast,c.Last as cLast,p.Last as pLast,c.Updated as cUpdated ,p.Updated as pUpdated , f.Updated,

                    iif(c.StrikePrice < f.Last-@rango*10,'Call','Put') as TipoMB10,
                    iif( c.StrikePrice < f.Last-@rango*10, ((f.Last-@rango*10) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*10) + c.Last )) as B10,
                    iif(c.StrikePrice < f.Last-@rango*9,'Call','Put') as TipoMB9,
                    iif( c.StrikePrice < f.Last-@rango*9, ((f.Last-@rango*9) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*9) + c.Last )) as B9,
                    iif(c.StrikePrice < f.Last-@rango*8,'Call','Put') as TipoMB8,
                    iif( c.StrikePrice < f.Last-@rango*8, ((f.Last-@rango*8) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*8) + c.Last )) as B8,
                    iif(c.StrikePrice < f.Last-@rango*7,'Call','Put') as TipoMB7,
                    iif( c.StrikePrice < f.Last-@rango*7, ((f.Last-@rango*7) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*7) + c.Last )) as B7,
                    iif(c.StrikePrice < f.Last-@rango*6,'Call','Put') as TipoMB6,
                    iif( c.StrikePrice < f.Last-@rango*6, ((f.Last-@rango*6) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*6) + c.Last )) as B6,
                    iif(c.StrikePrice < f.Last-@rango*5,'Call','Put') as TipoMB5,
                    iif( c.StrikePrice < f.Last-@rango*5, ((f.Last-@rango*5) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*5) + c.Last )) as B5,
                    iif(c.StrikePrice < f.Last-@rango*4,'Call','Put') as TipoMB4,
                    iif( c.StrikePrice < f.Last-@rango*4, ((f.Last-@rango*4) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*4) + c.Last )) as B4,
                    iif(c.StrikePrice < f.Last-@rango*3,'Call','Put') as TipoMB3,
                    iif( c.StrikePrice < f.Last-@rango*3, ((f.Last-@rango*3) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*3) + c.Last )) as B3,
                    iif(c.StrikePrice < f.Last-@rango*2,'Call','Put') as TipoMB2,
                    iif( c.StrikePrice < f.Last-@rango*2, ((f.Last-@rango*2) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*2) + c.Last )) as B2,
                    iif(c.StrikePrice < f.Last-@rango,'Call','Put') as TipoMB1,
                    iif( c.StrikePrice < f.Last-@rango, ((f.Last-@rango) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango) + c.Last )) as B1,

                    iif(c.StrikePrice < f.Last,'Call','Put') as TipoM,
                    iif( c.StrikePrice < f.Last, (f.Last - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - f.Last + c.Last )) as M,

                    iif(c.StrikePrice < f.Last+@rango,'Call','Put') as TipoMAL1,
                    iif( c.StrikePrice < f.Last+@rango, ((f.Last+@rango) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango) + c.Last )) as AL1,
                    iif(c.StrikePrice < f.Last+@rango*2,'Call','Put') as TipoMAL2,
                    iif( c.StrikePrice < f.Last+@rango*2, ((f.Last+@rango*2) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*2) + c.Last )) as AL2,
                    iif(c.StrikePrice < f.Last+@rango*3,'Call','Put') as TipoMAL3,
                    iif( c.StrikePrice < f.Last+@rango*3, ((f.Last+@rango*3) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*3) + c.Last )) as AL3,
                    iif(c.StrikePrice < f.Last+@rango*4,'Call','Put') as TipoMAL4,
                    iif( c.StrikePrice < f.Last+@rango*4, ((f.Last+@rango*4) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*4) + c.Last )) as AL4,
                    iif(c.StrikePrice < f.Last+@rango*5,'Call','Put') as TipoMAL5,
                    iif( c.StrikePrice < f.Last+@rango*5, ((f.Last+@rango*5) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*5) + c.Last )) as AL5,
                    iif(c.StrikePrice < f.Last+@rango*6,'Call','Put') as TipoMAL6,
                    iif( c.StrikePrice < f.Last+@rango*6, ((f.Last+@rango*6) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*6) + c.Last )) as AL6,
                    iif(c.StrikePrice < f.Last+@rango*7,'Call','Put') as TipoMAL7,
                    iif( c.StrikePrice < f.Last+@rango*7, ((f.Last+@rango*7) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*7) + c.Last )) as AL7,
                    iif(c.StrikePrice < f.Last+@rango*8,'Call','Put') as TipoMAL8,
                    iif( c.StrikePrice < f.Last+@rango*8, ((f.Last+@rango*8) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*8) + c.Last )) as AL8,
                    iif(c.StrikePrice < f.Last+@rango*9,'Call','Put') as TipoMAL9,
                    iif( c.StrikePrice < f.Last+@rango*9, ((f.Last+@rango*9) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*9) + c.Last )) as AL9,
                    iif(c.StrikePrice < f.Last+@rango*10,'Call','Put') as TipoMAL10,
                    iif( c.StrikePrice < f.Last+@rango*10, ((f.Last+@rango*10) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*10) + c.Last )) as AL10




                    from
                    (
                    select 
                    * 
                    from tOpcionesCMEquotes c where c.Tipo = 'Call' and c.date = @dia and c.Month = @MonthOp
                    ) c
                    inner join 
                    (
                    select 
                    * 
                    from tOpcionesCMEquotes p where p.Tipo = 'Put' and p.date = @dia and p.Month = @MonthOp
                    ) p on c.date = p.date and c.StrikePrice = p.StrikePrice and c.Month = p.Month and c.Last != 0 and p.Last != 0

                    left join tFuturosCMEquotes 
                    f on  c.ExpirationMonth = f.Month and c.date = f.date
                    ",rango,mes,hoy)).ToList();


                        this.Dispatcher.Invoke(() =>
                        {
                            DateTime? nhoy = _7G.tOpcionesCMEquotes.Select(x => x.date).OrderByDescending(x => x.Value).FirstOrDefault();

                            // Estilo de la Cabezera del Mes y Strike Price
                            var headerStyleO = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                            headerStyleO.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eceff1")) });
                            headerStyleO.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")) });
                            headerStyleO.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });




                            // Estilo de la Cabezera del Contrato de futuro subyasente 
                            var headerStyleM = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                            headerStyleM.Setters.Add(new Setter{  Property = BackgroundProperty,Value    = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")) });
                            headerStyleM.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")) });
                            headerStyleM.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") } );

                            // Estilo de la Cabezera Limite Bajo del Contrato de futuro subyasente 
                            var headerStyleB = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                            headerStyleB.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0026ca")) });
                            headerStyleB.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")) });
                            headerStyleB.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });


                            // Estilo de la Cabezera Bajo del Contrato de futuro subyasente 
                            var headerStylenB = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                            headerStylenB.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bbdefb")) });
                            headerStylenB.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")) });
                            headerStylenB.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });


                            // Estilo de la Cabezera Limite Alto del Contrato de futuro subyasente 
                            var headerStyleAL = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                            headerStyleAL.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d50000")) });
                            headerStyleAL.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")) });
                            headerStyleAL.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });

                            // Estilo de la Cabezera Alto del Contrato de futuro subyasente 
                            var headerStylenAL = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                            headerStylenAL.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffcdd2")) });
                            headerStylenAL.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")) });
                            headerStylenAL.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });



                            double fLast = d.Select(x => x.fLast).FirstOrDefault() != null ? (double)d.Select(x => x.fLast).First() : 0;


                            dgcolStrikePrice.HeaderStyle = headerStyleO;
                            dgcolStrikePrice.Header = "Strike Price ";
                            
                            dgcolMonth.HeaderStyle = headerStyleO;
                            dgcolMonth.Header = "Month";

                            var ResumenOpciones1 = _7G.ResumenOpciones.Where(x => x.id == 1).FirstOrDefault();
                            ResumenOpciones1.LimiteCompras = null;
                            ResumenOpciones1.LimiteVentas = null;

                            #region //Identificamos el limite Bajo

                            if (
                            d.Where(x => x.TipoMB10 == "Call" && x.B10 > 0).FirstOrDefault() == null &&
                            d.Where(x => x.TipoMB9 == "Call" && x.B9 > 0).FirstOrDefault() != null
                            )
                            {
                                B10.Header = (fLast - (rango * 10)).ToString("N5");
                                B10.HeaderStyle = headerStyleB;
                                ResumenOpciones1.LimiteCompras = (fLast - (rango * 10));

                            }
                            else
                            {
                                B10.Header = (fLast - (rango * 10)).ToString("N5");
                                B10.HeaderStyle = headerStylenB;
                            }

                            if (
                                                        d.Where(x => x.TipoMB9 == "Call" && x.B9 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMB8 == "Call" && x.B8 > 0).FirstOrDefault() != null
                                                        )
                            {
                                B9.Header = (fLast - (rango * 9)).ToString("N5");
                                B9.HeaderStyle = headerStyleB;
                                ResumenOpciones1.LimiteCompras = (fLast - (rango * 9));
                            }
                            else
                            {
                                B9.Header = (fLast - (rango * 9)).ToString("N5");
                                B9.HeaderStyle = headerStylenB;
                            }

                            if (
                                                        d.Where(x => x.TipoMB8 == "Call" && x.B8 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMB7 == "Call" && x.B7 > 0).FirstOrDefault() != null
                                                        )
                            {
                                B8.Header = (fLast - (rango * 8)).ToString("N5");
                                B8.HeaderStyle = headerStyleB;
                                ResumenOpciones1.LimiteCompras = (fLast - (rango * 8));
                            }
                            else
                            {
                                B8.Header = (fLast - (rango * 8)).ToString("N5");
                                B8.HeaderStyle = headerStylenB;
                            }

                            if (
                                                        d.Where(x => x.TipoMB7 == "Call" && x.B7 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMB6 == "Call" && x.B6 > 0).FirstOrDefault() != null
                                                        )
                            {
                                B7.Header = (fLast - (rango * 7)).ToString("N5");
                                B7.HeaderStyle = headerStyleB;
                                ResumenOpciones1.LimiteCompras = (fLast - (rango * 7));
                            }
                            else
                            {
                                B7.Header = (fLast - (rango * 7)).ToString("N5");
                                B7.HeaderStyle = headerStylenB;
                            }

                            if (
                                                        d.Where(x => x.TipoMB6 == "Call" && x.B6 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMB5 == "Call" && x.B5 > 0).FirstOrDefault() != null
                                                        )
                            {
                                B6.Header = (fLast - (rango * 6)).ToString("N5");
                                B6.HeaderStyle = headerStyleB;
                                ResumenOpciones1.LimiteCompras = (fLast - (rango * 6));
                            }
                            else
                            {
                                B6.Header = (fLast - (rango * 6)).ToString("N5");
                                B6.HeaderStyle = headerStylenB;
                            }

                            if (
                                                        d.Where(x => x.TipoMB5 == "Call" && x.B5 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMB4 == "Call" && x.B4 > 0).FirstOrDefault() != null
                                                        )
                            {
                                B5.Header = (fLast - (rango * 5)).ToString("N5");
                                B5.HeaderStyle = headerStyleB;
                                ResumenOpciones1.LimiteCompras = (fLast - (rango * 5));
                            }
                            else
                            {
                                B5.Header = (fLast - (rango * 5)).ToString("N5");
                                B5.HeaderStyle = headerStylenB;
                            }

                            if (
                                                        d.Where(x => x.TipoMB4 == "Call" && x.B4 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMB3 == "Call" && x.B3 > 0).FirstOrDefault() != null
                                                        )
                            {
                                B4.Header = (fLast - (rango * 4)).ToString("N5");
                                B4.HeaderStyle = headerStyleB;
                                ResumenOpciones1.LimiteCompras = (fLast - (rango * 4));
                            }
                            else
                            {
                                B4.Header = (fLast - (rango * 4)).ToString("N5");
                                B4.HeaderStyle = headerStylenB;
                            }

                            if (
                                                        d.Where(x => x.TipoMB3 == "Call" && x.B3 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMB2 == "Call" && x.B2 > 0).FirstOrDefault() != null
                                                        )
                            {
                                B3.Header = (fLast - (rango * 3)).ToString("N5");
                                B3.HeaderStyle = headerStyleB;
                                ResumenOpciones1.LimiteCompras = (fLast - (rango * 3));
                            }
                            else
                            {
                                B3.Header = (fLast - (rango * 3)).ToString("N5");
                                B3.HeaderStyle = headerStylenB;
                            }

                            if (
                                                        d.Where(x => x.TipoMB2 == "Call" && x.B2 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMB1 == "Call" && x.B1 > 0).FirstOrDefault() != null
                                                        )
                            {
                                B2.Header = (fLast - (rango * 2)).ToString("N5");
                                B2.HeaderStyle = headerStyleB;
                                ResumenOpciones1.LimiteCompras = (fLast - (rango * 2));
                            }
                            else
                            {
                                B2.Header = (fLast - (rango * 2)).ToString("N5");
                                B2.HeaderStyle = headerStylenB;
                            }

                            if (
                                                        d.Where(x => x.TipoMB1 == "Call" && x.B1 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoM == "Call" && x.M > 0).FirstOrDefault() != null
                                                        )
                            {
                                B1.Header = (fLast - (rango * 1)).ToString("N5");
                                B1.HeaderStyle = headerStyleB;
                                ResumenOpciones1.LimiteCompras = (fLast - (rango * 1));
                            }
                            else
                            {
                                B1.Header = (fLast - (rango * 1)).ToString("N5");
                                B1.HeaderStyle = headerStylenB;
                            }



                            #endregion

                            M.Header = fLast.ToString("N5");
                            M.HeaderStyle = headerStyleM;
                            ResumenOpciones1.CotizacionFuturos = fLast;
                            ResumenOpciones1.SumaCall = d.Where(x => x.TipoM == "Call").Sum(x => x.M);
                            ResumenOpciones1.SumaPut = d.Where(x => x.TipoM == "Put").Sum(x => x.M);
                            ResumenOpciones1.Acttualizacion = DateTime.Now;
                            ResumenOpciones1.ActtualizacionCME = _7G.tOpcionesCMEquotes.Where(x => x.date == nhoy && x.Last != 0 && x.Updated != null).Select(x => x.Updated).OrderByDescending(x => x.Value).FirstOrDefault();

                            #region // Identificmos Limites Altos

                            if (
                            d.Where(x => x.TipoMAL1 == "Put" && x.AL1 > 0).FirstOrDefault() == null &&
                            d.Where(x => x.TipoM == "Put" && x.M > 0).FirstOrDefault() != null
                            )
                            {
                                AL1.Header = (fLast - (rango * 1)).ToString("N5");
                                AL1.HeaderStyle = headerStyleAL;
                                ResumenOpciones1.LimiteVentas = (fLast - (rango * 1));
                            }
                            else
                            {
                                AL1.Header = (fLast - (rango * 1)).ToString("N5");
                                AL1.HeaderStyle = headerStylenAL;
                            }

                            if (
                                                        d.Where(x => x.TipoMAL2 == "Put" && x.AL2 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMAL1 == "Put" && x.AL1 > 0).FirstOrDefault() != null
                                                        )
                            {
                                AL2.Header = (fLast - (rango * 2)).ToString("N5");
                                AL2.HeaderStyle = headerStyleAL;
                                ResumenOpciones1.LimiteVentas = (fLast - (rango * 2));
                            }
                            else
                            {
                                AL2.Header = (fLast - (rango * 2)).ToString("N5");
                                AL2.HeaderStyle = headerStylenAL;
                            }

                            if (
                                                        d.Where(x => x.TipoMAL3 == "Put" && x.AL3 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMAL2 == "Put" && x.AL2 > 0).FirstOrDefault() != null
                                                        )
                            {
                                AL3.Header = (fLast - (rango * 3)).ToString("N5");
                                AL3.HeaderStyle = headerStyleAL;
                                ResumenOpciones1.LimiteVentas = (fLast - (rango * 3));
                            }
                            else
                            {
                                AL3.Header = (fLast - (rango * 3)).ToString("N5");
                                AL3.HeaderStyle = headerStylenAL;
                            }

                            if (
                                                        d.Where(x => x.TipoMAL4 == "Put" && x.AL4 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMAL3 == "Put" && x.AL3 > 0).FirstOrDefault() != null
                                                        )
                            {
                                AL4.Header = (fLast - (rango * 4)).ToString("N5");
                                AL4.HeaderStyle = headerStyleAL;
                                ResumenOpciones1.LimiteVentas = (fLast - (rango * 4));
                            }
                            else
                            {
                                AL4.Header = (fLast - (rango * 4)).ToString("N5");
                                AL4.HeaderStyle = headerStylenAL;
                            }

                            if (
                                                        d.Where(x => x.TipoMAL5 == "Put" && x.AL5 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMAL4 == "Put" && x.AL4 > 0).FirstOrDefault() != null
                                                        )
                            {
                                AL5.Header = (fLast - (rango * 5)).ToString("N5");
                                AL5.HeaderStyle = headerStyleAL;
                                ResumenOpciones1.LimiteVentas = (fLast - (rango * 5));
                            }
                            else
                            {
                                AL5.Header = (fLast - (rango * 5)).ToString("N5");
                                AL5.HeaderStyle = headerStylenAL;
                            }

                            if (
                                                        d.Where(x => x.TipoMAL6 == "Put" && x.AL6 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMAL5 == "Put" && x.AL5 > 0).FirstOrDefault() != null
                                                        )
                            {
                                AL6.Header = (fLast - (rango * 6)).ToString("N5");
                                AL6.HeaderStyle = headerStyleAL;
                                ResumenOpciones1.LimiteVentas = (fLast - (rango * 6));
                            }
                            else
                            {
                                AL6.Header = (fLast - (rango * 6)).ToString("N5");
                                AL6.HeaderStyle = headerStylenAL;
                            }

                            if (
                                                        d.Where(x => x.TipoMAL7 == "Put" && x.AL7 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMAL6 == "Put" && x.AL6 > 0).FirstOrDefault() != null
                                                        )
                            {
                                AL7.Header = (fLast - (rango * 7)).ToString("N5");
                                AL7.HeaderStyle = headerStyleAL;
                                ResumenOpciones1.LimiteVentas = (fLast - (rango * 7));
                            }
                            else
                            {
                                AL7.Header = (fLast - (rango * 7)).ToString("N5");
                                AL7.HeaderStyle = headerStylenAL;
                            }

                            if (
                                                        d.Where(x => x.TipoMAL8 == "Put" && x.AL8 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMAL7 == "Put" && x.AL7 > 0).FirstOrDefault() != null
                                                        )
                            {
                                AL8.Header = (fLast - (rango * 8)).ToString("N5");
                                AL8.HeaderStyle = headerStyleAL;
                                ResumenOpciones1.LimiteVentas = (fLast - (rango * 8));
                            }
                            else
                            {
                                AL8.Header = (fLast - (rango * 8)).ToString("N5");
                                AL8.HeaderStyle = headerStylenAL;
                            }

                            if (
                                                        d.Where(x => x.TipoMAL9 == "Put" && x.AL9 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMAL8 == "Put" && x.AL8 > 0).FirstOrDefault() != null
                                                        )
                            {
                                AL9.Header = (fLast - (rango * 9)).ToString("N5");
                                AL9.HeaderStyle = headerStyleAL;
                                ResumenOpciones1.LimiteVentas = (fLast - (rango * 9));
                            }
                            else
                            {
                                AL9.Header = (fLast - (rango * 9)).ToString("N5");
                                AL9.HeaderStyle = headerStylenAL;
                            }

                            if (
                                                        d.Where(x => x.TipoMAL10 == "Put" && x.AL10 > 0).FirstOrDefault() == null &&
                                                        d.Where(x => x.TipoMAL9 == "Put" && x.AL9 > 0).FirstOrDefault() != null
                                                        )
                            {
                                AL10.Header = (fLast - (rango * 10)).ToString("N5");
                                AL10.HeaderStyle = headerStyleAL;
                                ResumenOpciones1.LimiteVentas = (fLast - (rango * 10));
                            }
                            else
                            {
                                AL10.Header = (fLast - (rango * 10)).ToString("N5");
                                AL10.HeaderStyle = headerStylenAL;
                            }



                            #endregion

                            _7G.Entry(ResumenOpciones1).State = System.Data.Entity.EntityState.Modified;
                            _7G.SaveChanges();
                            optionCMEDataGrid.ItemsSource = d;
                            txblActtualizacion.Text = DateTime.Now.ToString();
                            txblActtualizacionCME.Text = _7G.tOpcionesCMEquotes.Where(x => x.date == nhoy && x.Last != 0 && x.Updated != null).Select(x => x.Updated).OrderByDescending(x => x.Value).FirstOrDefault().ToString();
                        });
                //}
                //    catch
                //{

                //}
            }
                });

            await Task.Factory.StartNew(() =>
            {
                using (GEntities _7G = new GEntities())
                {
                    //try
                    //{

                    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    var h = (DateTime)_7G.tOpcionesCMEquotes.Select(x => x.date).OrderByDescending(x => x.Value).FirstOrDefault();
                    //var fd = (DateTime)_7G.tFuturosCMEquotes.Select(x => x.date).OrderByDescending(x => x.Value).FirstOrDefault();
                    var mes = _7G.tOpcionesCMEquotes.Where(x => x.date == h).Min(x => x.Month).Value.AddMonths(1).ToString("dd/MM/yyyy");
                    string hoy = h.ToString("dd/MM/yyyy");
                    //string fdate = fd.ToString("dd/MM/yyyy");

                    double rango = 0.0005;


                    List<oOptionCMEDataGrid> d = _7G.Database.SqlQuery<oOptionCMEDataGrid>(string.Format(@"

                    declare @dia  date; -- Dia actual
                    declare @MonthOp  date; -- Mes Opciones
                    declare @rango  float; -- Mes Opciones


                    set @dia = '{2}';
                    set @MonthOp = '{1}';
                    set @rango = {0};


                    select 

                    c.Month,c.StrikePrice, f.Last as fLast,c.Last as cLast,p.Last as pLast,c.Updated as cUpdated ,p.Updated as pUpdated , f.Updated,

                    iif(c.StrikePrice < f.Last-@rango*10,'Call','Put') as TipoMB10,
                    iif( c.StrikePrice < f.Last-@rango*10, ((f.Last-@rango*10) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*10) + c.Last )) as B10,
                    iif(c.StrikePrice < f.Last-@rango*9,'Call','Put') as TipoMB9,
                    iif( c.StrikePrice < f.Last-@rango*9, ((f.Last-@rango*9) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*9) + c.Last )) as B9,
                    iif(c.StrikePrice < f.Last-@rango*8,'Call','Put') as TipoMB8,
                    iif( c.StrikePrice < f.Last-@rango*8, ((f.Last-@rango*8) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*8) + c.Last )) as B8,
                    iif(c.StrikePrice < f.Last-@rango*7,'Call','Put') as TipoMB7,
                    iif( c.StrikePrice < f.Last-@rango*7, ((f.Last-@rango*7) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*7) + c.Last )) as B7,
                    iif(c.StrikePrice < f.Last-@rango*6,'Call','Put') as TipoMB6,
                    iif( c.StrikePrice < f.Last-@rango*6, ((f.Last-@rango*6) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*6) + c.Last )) as B6,
                    iif(c.StrikePrice < f.Last-@rango*5,'Call','Put') as TipoMB5,
                    iif( c.StrikePrice < f.Last-@rango*5, ((f.Last-@rango*5) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*5) + c.Last )) as B5,
                    iif(c.StrikePrice < f.Last-@rango*4,'Call','Put') as TipoMB4,
                    iif( c.StrikePrice < f.Last-@rango*4, ((f.Last-@rango*4) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*4) + c.Last )) as B4,
                    iif(c.StrikePrice < f.Last-@rango*3,'Call','Put') as TipoMB3,
                    iif( c.StrikePrice < f.Last-@rango*3, ((f.Last-@rango*3) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*3) + c.Last )) as B3,
                    iif(c.StrikePrice < f.Last-@rango*2,'Call','Put') as TipoMB2,
                    iif( c.StrikePrice < f.Last-@rango*2, ((f.Last-@rango*2) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango*2) + c.Last )) as B2,
                    iif(c.StrikePrice < f.Last-@rango,'Call','Put') as TipoMB1,
                    iif( c.StrikePrice < f.Last-@rango, ((f.Last-@rango) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last-@rango) + c.Last )) as B1,

                    iif(c.StrikePrice < f.Last,'Call','Put') as TipoM,
                    iif( c.StrikePrice < f.Last, (f.Last - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - f.Last + c.Last )) as M,

                    iif(c.StrikePrice < f.Last+@rango,'Call','Put') as TipoMAL1,
                    iif( c.StrikePrice < f.Last+@rango, ((f.Last+@rango) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango) + c.Last )) as AL1,
                    iif(c.StrikePrice < f.Last+@rango*2,'Call','Put') as TipoMAL2,
                    iif( c.StrikePrice < f.Last+@rango*2, ((f.Last+@rango*2) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*2) + c.Last )) as AL2,
                    iif(c.StrikePrice < f.Last+@rango*3,'Call','Put') as TipoMAL3,
                    iif( c.StrikePrice < f.Last+@rango*3, ((f.Last+@rango*3) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*3) + c.Last )) as AL3,
                    iif(c.StrikePrice < f.Last+@rango*4,'Call','Put') as TipoMAL4,
                    iif( c.StrikePrice < f.Last+@rango*4, ((f.Last+@rango*4) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*4) + c.Last )) as AL4,
                    iif(c.StrikePrice < f.Last+@rango*5,'Call','Put') as TipoMAL5,
                    iif( c.StrikePrice < f.Last+@rango*5, ((f.Last+@rango*5) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*5) + c.Last )) as AL5,
                    iif(c.StrikePrice < f.Last+@rango*6,'Call','Put') as TipoMAL6,
                    iif( c.StrikePrice < f.Last+@rango*6, ((f.Last+@rango*6) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*6) + c.Last )) as AL6,
                    iif(c.StrikePrice < f.Last+@rango*7,'Call','Put') as TipoMAL7,
                    iif( c.StrikePrice < f.Last+@rango*7, ((f.Last+@rango*7) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*7) + c.Last )) as AL7,
                    iif(c.StrikePrice < f.Last+@rango*8,'Call','Put') as TipoMAL8,
                    iif( c.StrikePrice < f.Last+@rango*8, ((f.Last+@rango*8) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*8) + c.Last )) as AL8,
                    iif(c.StrikePrice < f.Last+@rango*9,'Call','Put') as TipoMAL9,
                    iif( c.StrikePrice < f.Last+@rango*9, ((f.Last+@rango*9) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*9) + c.Last )) as AL9,
                    iif(c.StrikePrice < f.Last+@rango*10,'Call','Put') as TipoMAL10,
                    iif( c.StrikePrice < f.Last+@rango*10, ((f.Last+@rango*10) - (c.StrikePrice + c.Last) + p.Last ),( (p.StrikePrice - p.Last) - (f.Last+@rango*10) + c.Last )) as AL10




                    from
                    (
                    select 
                    * 
                    from tOpcionesCMEquotes c where c.Tipo = 'Call' and c.date = @dia and c.Month = @MonthOp
                    ) c
                    inner join 
                    (
                    select 
                    * 
                    from tOpcionesCMEquotes p where p.Tipo = 'Put' and p.date = @dia and p.Month = @MonthOp
                    ) p on c.date = p.date and c.StrikePrice = p.StrikePrice and c.Month = p.Month and c.Last != 0 and p.Last != 0

                    left join tFuturosCMEquotes 
                    f on  c.ExpirationMonth = f.Month and c.date = f.date
                    ", rango, mes, hoy)).ToList();


                    this.Dispatcher.Invoke(() =>
                    {
                        DateTime? nhoy = _7G.tOpcionesCMEquotes.Select(x => x.date).OrderByDescending(x => x.Value).FirstOrDefault();

                        // Estilo de la Cabezera del Mes y Strike Price
                        var headerStyleO = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                        headerStyleO.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eceff1")) });
                        headerStyleO.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")) });
                        headerStyleO.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });




                        // Estilo de la Cabezera del Contrato de futuro subyasente 
                        var headerStyleM = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                        headerStyleM.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")) });
                        headerStyleM.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")) });
                        headerStyleM.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });

                        // Estilo de la Cabezera Limite Bajo del Contrato de futuro subyasente 
                        var headerStyleB = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                        headerStyleB.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0026ca")) });
                        headerStyleB.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")) });
                        headerStyleB.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });


                        // Estilo de la Cabezera Bajo del Contrato de futuro subyasente 
                        var headerStylenB = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                        headerStylenB.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bbdefb")) });
                        headerStylenB.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")) });
                        headerStylenB.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });


                        // Estilo de la Cabezera Limite Alto del Contrato de futuro subyasente 
                        var headerStyleAL = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                        headerStyleAL.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d50000")) });
                        headerStyleAL.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff")) });
                        headerStyleAL.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });

                        // Estilo de la Cabezera Alto del Contrato de futuro subyasente 
                        var headerStylenAL = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                        headerStylenAL.Setters.Add(new Setter { Property = BackgroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffcdd2")) });
                        headerStylenAL.Setters.Add(new Setter { Property = ForegroundProperty, Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")) });
                        headerStylenAL.Setters.Add(new Setter { Property = FontSizeProperty, Value = Convert.ToDouble("8") });



                        double fLast = d.Select(x => x.fLast).FirstOrDefault() != null ? (double)d.Select(x => x.fLast).First() : 0;


                        dgcolStrikePriceopp2.HeaderStyle = headerStyleO;
                        dgcolStrikePriceopp2.Header = "Strike Price ";

                        dgcolMonthopp2.HeaderStyle = headerStyleO;
                        dgcolMonthopp2.Header = "Month";

                        var ResumenOpciones2 = _7G.ResumenOpciones.Where(x => x.id == 2).FirstOrDefault();
                        ResumenOpciones2.LimiteCompras = null;
                        ResumenOpciones2.LimiteVentas = null;

                        #region //Identificamos el limite Bajo


                        if (
                        d.Where(x => x.TipoMB10 == "Call" && x.B10 > 0).FirstOrDefault() == null &&
                        d.Where(x => x.TipoMB9 == "Call" && x.B9 > 0).FirstOrDefault() != null
                        )
                        {
                            B10opp2.Header = (fLast - (rango * 10)).ToString("N5");
                            B10opp2.HeaderStyle = headerStyleB;
                            ResumenOpciones2.LimiteCompras = (fLast - (rango * 10));
                        }
                        else
                        {
                            B10opp2.Header = (fLast - (rango * 10)).ToString("N5");
                            B10opp2.HeaderStyle = headerStylenB;
                        }

                        if (
                                                    d.Where(x => x.TipoMB9 == "Call" && x.B9 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMB8 == "Call" && x.B8 > 0).FirstOrDefault() != null
                                                    )
                        {
                            B9opp2.Header = (fLast - (rango * 9)).ToString("N5");
                            B9opp2.HeaderStyle = headerStyleB;
                            ResumenOpciones2.LimiteCompras = (fLast - (rango * 9));

                        }
                        else
                        {
                            B9opp2.Header = (fLast - (rango * 9)).ToString("N5");
                            B9opp2.HeaderStyle = headerStylenB;
                        }

                        if (
                                                    d.Where(x => x.TipoMB8 == "Call" && x.B8 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMB7 == "Call" && x.B7 > 0).FirstOrDefault() != null
                                                    )
                        {
                            B8opp2.Header = (fLast - (rango * 8)).ToString("N5");
                            B8opp2.HeaderStyle = headerStyleB;
                            ResumenOpciones2.LimiteCompras = (fLast - (rango * 8));
                        }
                        else
                        {
                            B8opp2.Header = (fLast - (rango * 8)).ToString("N5");
                            B8opp2.HeaderStyle = headerStylenB;
                        }

                        if (
                                                    d.Where(x => x.TipoMB7 == "Call" && x.B7 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMB6 == "Call" && x.B6 > 0).FirstOrDefault() != null
                                                    )
                        {
                            B7opp2.Header = (fLast - (rango * 7)).ToString("N5");
                            B7opp2.HeaderStyle = headerStyleB;
                            ResumenOpciones2.LimiteCompras = (fLast - (rango * 7));
                        }
                        else
                        {
                            B7opp2.Header = (fLast - (rango * 7)).ToString("N5");
                            B7opp2.HeaderStyle = headerStylenB;
                        }

                        if (
                                                    d.Where(x => x.TipoMB6 == "Call" && x.B6 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMB5 == "Call" && x.B5 > 0).FirstOrDefault() != null
                                                    )
                        {
                            B6opp2.Header = (fLast - (rango * 6)).ToString("N5");
                            B6opp2.HeaderStyle = headerStyleB;
                            ResumenOpciones2.LimiteCompras = (fLast - (rango * 6));
                        }
                        else
                        {
                            B6opp2.Header = (fLast - (rango * 6)).ToString("N5");
                            B6opp2.HeaderStyle = headerStylenB;
                        }

                        if (
                                                    d.Where(x => x.TipoMB5 == "Call" && x.B5 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMB4 == "Call" && x.B4 > 0).FirstOrDefault() != null
                                                    )
                        {
                            B5opp2.Header = (fLast - (rango * 5)).ToString("N5");
                            B5opp2.HeaderStyle = headerStyleB;
                            ResumenOpciones2.LimiteCompras = (fLast - (rango * 5));
                        }
                        else
                        {
                            B5opp2.Header = (fLast - (rango * 5)).ToString("N5");
                            B5opp2.HeaderStyle = headerStylenB;
                        }

                        if (
                                                    d.Where(x => x.TipoMB4 == "Call" && x.B4 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMB3 == "Call" && x.B3 > 0).FirstOrDefault() != null
                                                    )
                        {
                            B4opp2.Header = (fLast - (rango * 4)).ToString("N5");
                            B4opp2.HeaderStyle = headerStyleB;
                            ResumenOpciones2.LimiteCompras = (fLast - (rango * 4));
                        }
                        else
                        {
                            B4opp2.Header = (fLast - (rango * 4)).ToString("N5");
                            B4opp2.HeaderStyle = headerStylenB;
                        }

                        if (
                                                    d.Where(x => x.TipoMB3 == "Call" && x.B3 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMB2 == "Call" && x.B2 > 0).FirstOrDefault() != null
                                                    )
                        {
                            B3opp2.Header = (fLast - (rango * 3)).ToString("N5");
                            B3opp2.HeaderStyle = headerStyleB;
                            ResumenOpciones2.LimiteCompras = (fLast - (rango * 3));
                        }
                        else
                        {
                            B3opp2.Header = (fLast - (rango * 3)).ToString("N5");
                            B3opp2.HeaderStyle = headerStylenB;
                        }

                        if (
                                                    d.Where(x => x.TipoMB2 == "Call" && x.B2 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMB1 == "Call" && x.B1 > 0).FirstOrDefault() != null
                                                    )
                        {
                            B2opp2.Header = (fLast - (rango * 2)).ToString("N5");
                            B2opp2.HeaderStyle = headerStyleB;
                            ResumenOpciones2.LimiteCompras = (fLast - (rango * 2));
                        }
                        else
                        {
                            B2opp2.Header = (fLast - (rango * 2)).ToString("N5");
                            B2opp2.HeaderStyle = headerStylenB;
                        }

                        if (
                                                    d.Where(x => x.TipoMB1 == "Call" && x.B1 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoM == "Call" && x.M > 0).FirstOrDefault() != null
                                                    )
                        {
                            B1opp2.Header = (fLast - (rango * 1)).ToString("N5");
                            B1opp2.HeaderStyle = headerStyleB;
                            ResumenOpciones2.LimiteCompras = (fLast - (rango * 1));
                        }
                        else
                        {
                            B1opp2.Header = (fLast - (rango * 1)).ToString("N5");
                            B1opp2.HeaderStyle = headerStylenB;
                        }



                        #endregion

                        Mopp2.Header = fLast.ToString("N5");
                        Mopp2.HeaderStyle = headerStyleM;
                        ResumenOpciones2.CotizacionFuturos = fLast;
                        ResumenOpciones2.SumaCall = d.Where(x => x.TipoM == "Call").Sum(x => x.M);
                        ResumenOpciones2.SumaPut = d.Where(x => x.TipoM == "Put").Sum(x => x.M);
                        ResumenOpciones2.Acttualizacion = DateTime.Now;
                        ResumenOpciones2.ActtualizacionCME = _7G.tOpcionesCMEquotes.Where(x => x.date == nhoy && x.Last != 0 && x.Updated != null).Select(x => x.Updated).OrderByDescending(x => x.Value).FirstOrDefault();

                        #region // Identificmos Limites Altos

                        if (
                        d.Where(x => x.TipoMAL1 == "Put" && x.AL1 > 0).FirstOrDefault() == null &&
                        d.Where(x => x.TipoM == "Put" && x.M > 0).FirstOrDefault() != null
                        )
                        {
                            AL1opp2.Header = (fLast - (rango * 1)).ToString("N5");
                            AL1opp2.HeaderStyle = headerStyleAL;
                            ResumenOpciones2.LimiteVentas = (fLast - (rango * 1));
                        }
                        else
                        {
                            AL1opp2.Header = (fLast - (rango * 1)).ToString("N5");
                            AL1opp2.HeaderStyle = headerStylenAL;
                        }

                        if (
                                                    d.Where(x => x.TipoMAL2 == "Put" && x.AL2 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMAL1 == "Put" && x.AL1 > 0).FirstOrDefault() != null
                                                    )
                        {
                            AL2opp2.Header = (fLast - (rango * 2)).ToString("N5");
                            AL2opp2.HeaderStyle = headerStyleAL;
                            ResumenOpciones2.LimiteVentas = (fLast - (rango * 2));
                        }
                        else
                        {
                            AL2opp2.Header = (fLast - (rango * 2)).ToString("N5");
                            AL2opp2.HeaderStyle = headerStylenAL;
                        }

                        if (
                                                    d.Where(x => x.TipoMAL3 == "Put" && x.AL3 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMAL2 == "Put" && x.AL2 > 0).FirstOrDefault() != null
                                                    )
                        {
                            AL3opp2.Header = (fLast - (rango * 3)).ToString("N5");
                            AL3opp2.HeaderStyle = headerStyleAL;
                            ResumenOpciones2.LimiteVentas = (fLast - (rango * 3));
                        }
                        else
                        {
                            AL3opp2.Header = (fLast - (rango * 3)).ToString("N5");
                            AL3opp2.HeaderStyle = headerStylenAL;
                        }

                        if (
                                                    d.Where(x => x.TipoMAL4 == "Put" && x.AL4 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMAL3 == "Put" && x.AL3 > 0).FirstOrDefault() != null
                                                    )
                        {
                            AL4opp2.Header = (fLast - (rango * 4)).ToString("N5");
                            AL4opp2.HeaderStyle = headerStyleAL;
                            ResumenOpciones2.LimiteVentas = (fLast - (rango * 4));
                        }
                        else
                        {
                            AL4opp2.Header = (fLast - (rango * 4)).ToString("N5");
                            AL4opp2.HeaderStyle = headerStylenAL;
                        }

                        if (
                                                    d.Where(x => x.TipoMAL5 == "Put" && x.AL5 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMAL4 == "Put" && x.AL4 > 0).FirstOrDefault() != null
                                                    )
                        {
                            AL5opp2.Header = (fLast - (rango * 5)).ToString("N5");
                            AL5opp2.HeaderStyle = headerStyleAL;
                            ResumenOpciones2.LimiteVentas = (fLast - (rango * 5));
                        }
                        else
                        {
                            AL5opp2.Header = (fLast - (rango * 5)).ToString("N5");
                            AL5opp2.HeaderStyle = headerStylenAL;
                        }

                        if (
                                                    d.Where(x => x.TipoMAL6 == "Put" && x.AL6 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMAL5 == "Put" && x.AL5 > 0).FirstOrDefault() != null
                                                    )
                        {
                            AL6opp2.Header = (fLast - (rango * 6)).ToString("N5");
                            AL6opp2.HeaderStyle = headerStyleAL;
                            ResumenOpciones2.LimiteVentas = (fLast - (rango * 6));
                        }
                        else
                        {
                            AL6opp2.Header = (fLast - (rango * 6)).ToString("N5");
                            AL6opp2.HeaderStyle = headerStylenAL;
                        }

                        if (
                                                    d.Where(x => x.TipoMAL7 == "Put" && x.AL7 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMAL6 == "Put" && x.AL6 > 0).FirstOrDefault() != null
                                                    )
                        {
                            AL7opp2.Header = (fLast - (rango * 7)).ToString("N5");
                            AL7opp2.HeaderStyle = headerStyleAL;
                            ResumenOpciones2.LimiteVentas = (fLast - (rango * 7));
                        }
                        else
                        {
                            AL7opp2.Header = (fLast - (rango * 7)).ToString("N5");
                            AL7opp2.HeaderStyle = headerStylenAL;
                        }

                        if (
                                                    d.Where(x => x.TipoMAL8 == "Put" && x.AL8 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMAL7 == "Put" && x.AL7 > 0).FirstOrDefault() != null
                                                    )
                        {
                            AL8opp2.Header = (fLast - (rango * 8)).ToString("N5");
                            AL8opp2.HeaderStyle = headerStyleAL;
                            ResumenOpciones2.LimiteVentas = (fLast - (rango * 8));
                        }
                        else
                        {
                            AL8opp2.Header = (fLast - (rango * 8)).ToString("N5");
                            AL8opp2.HeaderStyle = headerStylenAL;
                        }

                        if (
                                                    d.Where(x => x.TipoMAL9 == "Put" && x.AL9 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMAL8 == "Put" && x.AL8 > 0).FirstOrDefault() != null
                                                    )
                        {
                            AL9opp2.Header = (fLast - (rango * 9)).ToString("N5");
                            AL9opp2.HeaderStyle = headerStyleAL;
                            ResumenOpciones2.LimiteVentas = (fLast - (rango * 9));
                        }
                        else
                        {
                            AL9opp2.Header = (fLast - (rango * 9)).ToString("N5");
                            AL9opp2.HeaderStyle = headerStylenAL;
                        }

                        if (
                                                    d.Where(x => x.TipoMAL10 == "Put" && x.AL10 > 0).FirstOrDefault() == null &&
                                                    d.Where(x => x.TipoMAL9 == "Put" && x.AL9 > 0).FirstOrDefault() != null
                                                    )
                        {
                            AL10opp2.Header = (fLast - (rango * 10)).ToString("N5");
                            AL10opp2.HeaderStyle = headerStyleAL;
                            ResumenOpciones2.LimiteVentas = (fLast - (rango * 10));
                        }
                        else
                        {
                            AL10opp2.Header = (fLast - (rango * 10)).ToString("N5");
                            AL10opp2.HeaderStyle = headerStylenAL;
                        }



                        #endregion

                        _7G.Entry(ResumenOpciones2).State = System.Data.Entity.EntityState.Modified;
                        _7G.SaveChanges();
                        optionCMEDataGridopp2.ItemsSource = d;
                        txblActtualizacionopp2.Text = DateTime.Now.ToString();
                        txblActtualizacionCMEopp2.Text = _7G.tOpcionesCMEquotes.Where(x => x.date == nhoy && x.Last != 0 && x.Updated != null).Select(x => x.Updated).OrderByDescending(x => x.Value).FirstOrDefault().ToString();
                    });
                    //}
                    //    catch
                    //{

                    //}
                }
            });

        }

        public async Task Actualizar_futurosDataGrid() //Rellena el CME Data Grid con los Valalores del Mercado de Futuros del CME 
        {
            await Task.Factory.StartNew(() =>
            {
                using (GEntities _7G = new GEntities())
                {
                    try
                    {

                        var fd = (DateTime)_7G.tFuturosCMEquotes.Select(x => x.date).OrderByDescending(x => x.Value).FirstOrDefault();
                        string fdate = fd.ToString("dd/MM/yyyy");

                        List<tFuturosCMEquotes> d = _7G.Database.SqlQuery<tFuturosCMEquotes>(string.Format(@"
                    
                        DECLARE @fdate datetime;
                        SET @fdate   = '{0}'
                        select * from tFuturosCMEquotes t
                        where t.date = @fdate and t.Last != 0
                        order by date desc
                        ",fdate)).ToList();


                        this.Dispatcher.Invoke(() =>
                        {
                            DateTime? nhoy = _7G.tFuturosCMEquotes.Select(x => x.date).OrderByDescending(x => x.Value).FirstOrDefault();
                            
                            futurosCMEDataGrid.ItemsSource = d;
                            txbflActtualizacion.Text = DateTime.Now.ToString();
                            txbflActtualizacionCME.Text = _7G.tFuturosCMEquotes.Where(x => x.date == nhoy && x.Last != 0 && x.Updated != null).Select(x => x.Updated).OrderByDescending(x => x.Value).FirstOrDefault().ToString();
                        });
                    }
                    catch
                    {

                    }
                }
            });

        }


        


        [FileIOPermission(SecurityAction.Demand, Write = @"C:\Users\DIR-EJECT\AppData\Roaming\MetaQuotes\Terminal\C142B020C05FAD9EEC4BE1375F709241\MQL4\Files")]
        [FileIOPermission(SecurityAction.Demand, Write = @"C:\Users\DIR-EJECT\AppData\Roaming\MetaQuotes\Terminal\C142B020C05FAD9EEC4BE1375F709241\tester\files")]
        private void btnWriteCSV_Click(object sender, RoutedEventArgs e) // Generar archivo CSV
        {
            using (GEntities _7G = new GEntities())
            {
                List<archivoCSVMT4> dataCSV = _7G.Database.SqlQuery<archivoCSVMT4>("select p.date,p.pFXclose,p.pCMEclose,p.pCMEproy, (p.pFXclose-p.pCMEproy) as 'difFXaCMEclose',(p.pCMEproy-p.pCMEclose) as 'difCMEcCMEproy', p.diasCicloFuturos as 'varFXaCMEclose' from tpikitosT p").ToList();

                string[] rpath = new string[2];

                rpath[0] = @"C:\Users\DIR-EJECT\AppData\Roaming\MetaQuotes\Terminal\C142B020C05FAD9EEC4BE1375F709241\MQL4\Files\7Gpluss.csv";
                rpath[1] = @"C:\Users\DIR-EJECT\AppData\Roaming\MetaQuotes\Terminal\C142B020C05FAD9EEC4BE1375F709241\tester\files\7Gpluss.csv";

                try
                {
                    for (int i = 0; i < 2; i++)
                    {
                        FileIOPermission filePermissions = new FileIOPermission(FileIOPermissionAccess.Write, rpath[i]);
                        filePermissions.Demand();

                        using (var file = File.CreateText(rpath[i]))
                        {
                            DateTime dia;


                            foreach (var pk in dataCSV)
                            {
                                DateTime.TryParse(pk.date.ToString(), out dia);


                                file.WriteLine(string.Join(",",
                                    dia.ToString("yyyy.MM.dd"),
                                    pk.difFXaCMEclose.ToString(),
                                    pk.difCMEcCMEproy.ToString(),
                                    pk.varFXaCMEclose.ToString()
                                    ));
                            }
                        }
                        Run myRun = new Run("Archivo "+ rpath[i]+" Generado Exitosamente!!");
                        Paragraph err = new Paragraph();
                        err.Inlines.Add(myRun);
                        FlowDocConsola.Blocks.Add(err);
                        richText.Document = FlowDocConsola;
                    }
                }
                catch (Exception ex)
                {
                    Run myRun = new Run(String.Format("{0}", ex.Message.ToString()));
                    Paragraph err = new Paragraph();
                    err.Inlines.Add(myRun);
                    FlowDocConsola.Blocks.Add(err);
                    richText.Document = FlowDocConsola;
                }
            }
        }

        private void btnReadSchemasXML_Click(object sender, RoutedEventArgs e)
        {
            var xmlStrg = @"https://www.quandl.com/api/v3/datasets/FRED/DGS10.xml?api_key=Kx7z33eMkxL5j29Jx1xe";

            var xmlDoc = XElement.Load(xmlStrg);

            //var ns = xmlDoc.GetDefaultNamespace();

            //var xmlQ = xmlDoc.Descendants(ns + "Series").Elements(ns + "Obs").Attributes("TIME_PERIOD").FirstOrDefault().Value.ToString().Substring(0,4);
            var xmlQ = xmlDoc.Descendants("datum").Descendants("datum").LongCount();


            Run myRun = new Run(xmlQ.ToString());
            Paragraph ext = new Paragraph();
            ext.Inlines.Add(myRun);
            FlowDocConsola.Blocks.Add(ext);
            richText.Document = FlowDocConsola;


            //foreach (var n in xmlQ)
            //{
            //    Run myRun = new Run(n.ToString());
            //    Paragraph ext = new Paragraph();
            //    ext.Inlines.Add(myRun);
            //    FlowDocConsola.Blocks.Add(ext);
            //    richText.Document = FlowDocConsola;
            //}
        }

        public void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
        
        private void descargar12h()
        {
            PPA ppa = new PPA();
            ppa.descargar();

            CPIeu cpieu = new CPIeu();
            cpieu.descargar();

            CPIus cpius = new CPIus();
            cpius.descargar();

            Yield10eu yield10eu = new Yield10eu();
            yield10eu.descargar();

            Yield10us yield10us = new Yield10us();
            yield10us.descargar();

            EURUSD eurusd = new EURUSD();
            eurusd.descargar();
        }

        private void btnGraficoPPA_Click(object sender, RoutedEventArgs e)
        {
            visorGraficos.Content = new graficoPPA();
        }

        private void GenerarPPAdaily_click(object sender, RoutedEventArgs e)
        {
            rellenarPppsEu_Us_Daily relle = new rellenarPppsEu_Us_Daily();
            relle.rellenaTabla();
        }

        private void DescargarCME_click(object sender, RoutedEventArgs e)
        {
            CMEeurusd des = new CMEeurusd();
            des.descargar();
        }

        private void btnCalcularPikitos_Click(object sender, RoutedEventArgs e)
        {
            Pikitos des = new Pikitos();
            des.descargar();
        }
    }
}
