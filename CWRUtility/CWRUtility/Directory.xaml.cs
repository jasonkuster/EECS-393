using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;

namespace CWRUtility
{

    public partial class Directory : PhoneApplicationPage
    {
        public Directory()
        {
            InitializeComponent();

            readFromFile();
        }
        
        //test case to see if i can even open/close the first resource description
        public void Expand(object sender, System.Windows.Input.GestureEventArgs e)
        {
			if (T1.Visibility.Equals("Visible")) //true = "Visibile"
			{
                T1.Visibility = System.Windows.Visibility.Collapsed;
			}
			else //if (T1.Visibility.Equals("Collapsed")) //false = "Collapsed"
			{
                T1.Visibility = System.Windows.Visibility.Visible;
			}
        }

        public void readFromFile()
        {

            string thefile = "";
            //ProjectName;component/data/filename.txt
            StreamReader sr = new StreamReader(":/Resources/CWRUtility;CampusResources.txt"); //the problem is this line, whatever comes after it is hit with a MethodAccessException
            //GETTING 1 REM cycle then takling it.
            //thefile = sr.ReadToEnd();
            //sr.Close();


            T1.Text = thefile;
        }


    }
    public class Resource
    {
        public Resource(string name, string phone, string info)
        {
            this.name = name;
            this.phone = phone;
            this.info = info;
        }
        public string name { get; set; }
        public string phone { get; set; }
        public string info { get; set; }
    }
}