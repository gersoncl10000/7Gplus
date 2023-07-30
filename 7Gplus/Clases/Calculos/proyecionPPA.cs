using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7Gplus.Clases.Economicas;
using System.Windows;

namespace _7Gplus.Clases.Calculos
{
    public class proyecionPPA
    {
        
        private DateTime dateUltmPPA { get; set; }
        private Double?   ultimoTcPpa { get; set; } // Ultimo tipo de Camabio PPA
        //DateTime datePrimCPI { get; set; }// Ultimo Primer CPI Registrado coinsidente con la Fecha final de PPA 
        private DateTime dateUltmCPI { get; set; }// Ultimo CPI Registrado 
        private double CPIus_1 { get; set; } //Primer IPC de Estados Unidos
        private double CPIus_2 { get; set; } //Ultimo IPC de Estados Unidos
        private double CPIue_1 { get; set; } //Primer IPC de Europa
        private double CPIue_2 { get; set; } //Ultimo IPC de Europa
        private double? ultimTc { get; set; } //Ultimo tipo de Camabio Ajustado a IPC de Europa
        private double? ultimYieldDiarioUS { get; set; } //Ultimo Yiel Diario US
        private double? ultimYieldDiarioUE { get; set; } //Ultimo Yiel Diario Europa
        double diasAnno { get; set; } // Calculas los dias del Año

        private DateTime dateUltmYield { get; set; }

        public double? tcEstimado { get; set; }




        public double? getTipoCambioPPAestimado(DateTime dateA , DateTime dateE )
        {

            GEntities _7G = new GEntities();

            var date = (from c in _7G.tPppsEuroUsds
                        where c.Date < dateA
                        select c).ToList();
            dateUltmPPA = DateTime.Parse(date.Max(s => s.Date).ToString());




            var uPpa = (from c in _7G.tPppsEuroUsds
                        where c.Date == dateUltmPPA
                        select c).SingleOrDefault();
            ultimoTcPpa = uPpa.Value;

            List<CPIus_eu> qCPI = _7G.Database.SqlQuery<CPIus_eu>(@"
            select t3.date,t3.cpiUS,t3.cpiUE 
            from(
            select EOMONTH(t1.date) as 'date', t1.value as 'cpiUS', t2.value as 'cpiUE' 
            from tCPIus t1 
            right join 
            (
            select * from
            (
            select * , RANK() OVER( PARTITION BY date order by id desc   ) as ranking

            from tCPIeu  
            ) t
            where t.ranking = 1

            ) t2 on t1.date = t2.date ) t3
            order by t3.date desc

            ").ToList();

            dateUltmCPI = qCPI.Where(s => s.date < dateA).Max(s => s.date).Value;

            CPIus_1 = qCPI.Where(s => s.date == dateUltmPPA).Select(s => s.cpiUS).SingleOrDefault().Value;
            CPIus_2 = qCPI.Where(s => s.date == dateUltmCPI).Select(s => s.cpiUS).SingleOrDefault().Value;
            CPIue_1 = qCPI.Where(s => s.date == dateUltmPPA).Select(s => s.cpiUE).SingleOrDefault().Value;
            CPIue_2 = qCPI.Where(s => s.date == dateUltmCPI).Select(s => s.cpiUE).SingleOrDefault().Value;

            ultimTc = ultimoTcPpa * ((CPIus_2 / CPIue_2)/(CPIus_1/ CPIue_1));

            List<yield10Y> qYield = _7G.Database.SqlQuery<yield10Y>("select t1.date, t1.value as 'yieldUS', t2.value as 'yieldUE'  from tbondUS10Y t1 inner join tbondEU10Y t2 on t1.date = t2.date").OrderBy(s => s.date).ToList();

            qYield = qYield.Where(s => s.date > dateUltmCPI && s.date < dateA).OrderBy(s => s.date).ToList();

            if (qYield.Count != 0)
            {
                diasAnno = (new DateTime(2017, 12, 31).ToOADate()) - (new DateTime(2016, 12, 31).ToOADate());
                dateUltmYield = dateUltmCPI; //Iniciamos la Ultima Fecha de Yiel a la par de la Ultima Fecha CPI

                foreach (var y in qYield)
                {
                    ultimYieldDiarioUS = Math.Exp((Math.Log(((double)((1 + (y.yieldUS / 100)) / 1))) / diasAnno)) - 1;
                    ultimYieldDiarioUE = Math.Exp((Math.Log((double)((1 + (y.yieldUE / 100)) / 1)) / diasAnno)) - 1;
                    var VarUS = (Math.Pow(1 + ((double)ultimYieldDiarioUS), (DateTime.Parse(y.date.ToString()).ToOADate() - DateTime.Parse(dateUltmYield.ToString()).ToOADate())));
                    var VarEU = (Math.Pow(1 + ((double)ultimYieldDiarioUE), (DateTime.Parse(y.date.ToString()).ToOADate() - DateTime.Parse(dateUltmYield.ToString()).ToOADate())));
                    ultimTc = ultimTc * (VarUS/VarEU);
                    dateUltmYield = (DateTime)y.date;
                }
                

            }
            else
            {
                dateUltmYield = dateUltmCPI;
                List<yield10Y> qeYield = _7G.Database.SqlQuery<yield10Y>("select t1.date, t1.value as 'yieldUS', t2.value as 'yieldUE'  from tbondUS10Y t1 inner join tbondEU10Y t2 on t1.date = t2.date").OrderBy(s => s.date).ToList();
                yield10Y y = qeYield.Where(s => s.date < dateA).OrderBy(s => s.date).FirstOrDefault();

                if (y == null)
                {
                    ultimYieldDiarioUS = 0;
                    ultimYieldDiarioUE = 0;
                }
                else
                {
                    ultimYieldDiarioUS = Math.Exp((Math.Log(((double)((1 + (y.yieldUS / 100)) / 1))) / diasAnno)) - 1;
                    ultimYieldDiarioUE = Math.Exp((Math.Log((double)((1 + (y.yieldUE / 100)) / 1)) / diasAnno)) - 1;
                }
                

            }
            var varUSest = (Math.Pow(1 + ((double)ultimYieldDiarioUS), (DateTime.Parse(dateE.ToString()).ToOADate() - DateTime.Parse(dateUltmYield.ToString()).ToOADate())));
            var varEUest = (Math.Pow(1 + ((double)ultimYieldDiarioUE), (DateTime.Parse(dateE.ToString()).ToOADate() - DateTime.Parse(dateUltmYield.ToString()).ToOADate())));

            tcEstimado = ultimTc * (varUSest / varEUest);
                                    

            if( double.IsNaN((double)tcEstimado))
            {
                tcEstimado = 0;
                return tcEstimado;
            }
            else
            {
                return tcEstimado;
            }


        }





    }
}
