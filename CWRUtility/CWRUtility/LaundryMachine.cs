using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CWRUtility
{
    public class LaundryMachine
    {
        public LaundryMachine(string number, string type, string availability, string timeRemaining, string color)
            {
                this.number = number;
                this.type = type;
                this.availability = availability;
                this.timeRemaining = timeRemaining;
                this.color = color;
            }

            public string number { get; set; }
            public string type { get; set; }
            public string availability { get; set; }
            public string timeRemaining { get; set; }
            public string color { get; set; }
    }
}
