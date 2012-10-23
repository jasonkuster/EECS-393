using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CWRUtility.ViewModels
{
    public class FavoriteViewModel : INotifyPropertyChanged
    {
        private string _stop;

        public string Stop
        {
            get
            {
                return _stop;
            }
            set
            {
                if (value != _stop)
                {
                    _stop = value;
                    NotifyPropertyChanged("Stop");
                }
            }
        }

        private string _timeOne;
        public string TimeOne
        {
            get
            {
                return _timeOne;
            }
            set
            {
                if (value != _timeOne)
                {
                    _timeOne = value;
                    NotifyPropertyChanged("TimeOne");
                }
            }
        }

        private string _timeTwo;
        public string TimeTwo
        {
            get
            {
                return _timeTwo;
            }
            set
            {
                if (value != _timeTwo)
                {
                    _timeTwo = value;
                    NotifyPropertyChanged("TimeTwo");
                }
            }
        }

        private string _timeThree;
        public string TimeThree
        {
            get
            {
                return _timeThree;
            }
            set
            {
                if (value != _timeThree)
                {
                    _timeThree = value;
                    NotifyPropertyChanged("TimeThree");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
