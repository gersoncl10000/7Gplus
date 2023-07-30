using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Gplus.Modelo
{
    class CMEDataGrid
    {
        private DateTime _date;
        private DateTime _endContractDate;
        private double _settle;
        private double _settle1;
        private double _varSettler;
        private double _varSettler1;
        private double _last;
        private double _last1;
        private double _varlast;
        private double _varlast1;
        public DateTime date
        {
            get { return _date; }
            set
            {
                _date = value;
                NotifyPropertyChanged("date");
            }
        }
        public DateTime endContractDate
        {
            get { return _endContractDate; }
            set
            {
                _endContractDate = value;
                NotifyPropertyChanged("endContractDate");
            }
        }
        public double settle
        {
            get { return _settle; }
            set
            {
                _settle = value;
                NotifyPropertyChanged("settle");
            }
        }
        public double settle1
        {
            get { return _settle1; }
            set
            {
                _settle1 = value;
                NotifyPropertyChanged("settle1");
            }
        }
        public double varSettler
        {
            get { return _varSettler; }
            set
            {
                _varSettler = value;
                NotifyPropertyChanged("varSettler");
            }
        }
        public double varSettler1
        {
            get { return _varSettler1; }
            set
            {
                _varSettler1 = value;
                NotifyPropertyChanged("varSettler1");
            }
        }
        public double last
        {
            get { return _last; }
            set
            {
                _last = value;
                NotifyPropertyChanged("last");
            }
        }
        public double last1
        {
            get { return _last1; }
            set
            {
                _last1 = value;
                NotifyPropertyChanged("last1");
            }
        }
        public double varlast
        {
            get { return _varlast; }
            set
            {
                _varlast = value;
                NotifyPropertyChanged("varlast");
            }
        }
        public double varlast1
        {
            get { return _varlast1; }
            set
            {
                _varlast1 = value;
                NotifyPropertyChanged("varlast1");
            }
        }






        // Property Change Logic  
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
