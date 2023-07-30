using _7Gplus.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Gplus.Clases.Procesos
{
    class pData
    {
        public List<Data> lData { get; set;}


        public void cargarDatos() //Descarga los Valalores del Mercado de Futuros del CME 
        {

            //await Task.Factory.StartNew(() =>
            //{
                using (GEntities _7G = new GEntities())
                {

                    lData = _7G.Database.SqlQuery<Data>(@"
                        select t1.Date,t1.Value,t1.ValueNext,t1.ValuePrev,t1.Value/t1.ValuePrev-1 as 'varPrev', t1.ValueNext/t1.Value-1 as 'varNext',t2.Value as 'PPA'
                        from(
                        select t.Date,t.Value,FIRST_VALUE(t.Date) OVER (ORDER BY t.Date) as 'First', LAG(t.Value) OVER (ORDER BY t.Date) as 'ValuePrev',
                        LEAD (t.Value) OVER (ORDER BY t.Date) as 'ValueNext'    
                        from tEURUSDfed t
                        ) t1 
                        inner join tPppsEuroUsdDaily t2 on t1.Date = t2.Date
                        where t1.Date != t1.First  
                        order by t1.Date
                        ").ToList();

                }



            //});

        }





        }
}
