using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Gplus.Modelo
{
    public class mt4fxEURUSD
    {
        public DateTime? date { get; set; }
        public string hours { get; set; }
        public double? open { get; set; }
        public double? high { get; set; }
        public double? low { get; set; }
        public double? close { get; set; }
        public double? volume { get; set; }

        public mt4fxEURUSD(DateTime? Date, string Hours, double? Open, double? High, double? Low, double? Close, double? Volume)
        {
            date = Date;
            hours = Hours;
            open = Open;
            high = High;
            low = Low;
            close = Close;
            volume = Volume;
           
        }


        public IEnumerable<mt4fxEURUSD> ReadMt4csv(string fileName )
        {
            string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(fileName, ".csv"));
            return lines.Select(line =>
            {
                string[] data = line.Split(',');
                // We return a person with the data in order.
                return new mt4fxEURUSD(
                    new DateTime(int.Parse(data[0].Substring(0,4)), int.Parse(data[0].Substring(5, 2)), int.Parse(data[0].Substring(8, 2))),
                    data[1], 
                    double.Parse(data[2], CultureInfo.InvariantCulture),
                    double.Parse(data[3], CultureInfo.InvariantCulture),
                    double.Parse(data[4], CultureInfo.InvariantCulture),
                    double.Parse(data[5], CultureInfo.InvariantCulture),
                    double.Parse(data[6], CultureInfo.InvariantCulture));
            });

        }

    }
}
