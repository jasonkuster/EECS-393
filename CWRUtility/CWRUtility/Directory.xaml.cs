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
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;

namespace CWRUtility
{

    public partial class Directory : PhoneApplicationPage
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        
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
            List<Resource> ListResource = new List<Resource>();
            string thefile = "";
            var resource = System.Windows.Application.GetResourceStream(new Uri("Resources/CampusResources.txt", UriKind.RelativeOrAbsolute));
            //StreamReader read = new StreamReader("C:/Users/VVilliam/Documents/GitHub/EECS-393/CWRUtility/CWRUtility/ResourcesCampusResources.txt");
            //read.Close();
            /*
            //ProjectName;component/data/filename.txt
            System.IO.StreamReader sr = new StreamReader("CWRUtility;Resources/CampusResources.txt"); //the problem is this line, whatever comes after it is hit with a MethodAccessException
            //GETTING 1 REM cycle then takling it.
            //thefile = sr.ReadToEnd();
            //sr.Close();
            

           / T1.Text = thefile;*/
            StreamReader SR = new StreamReader(resource.Stream);
            thefile = SR.ReadToEnd();
            char [] thechar = thefile.ToCharArray();
            Resource temp = new Resource ("", "", "", "");
            String Adder = "";
            

            int WhereAreWe = 0; //marks "CONCERNS" (Name, Phone, Location, and Info) with 0, 1, 2, and 3 respecfully.
            int Slasher = 0; //indicates if the last char read was a '\' so we know when a new line was made in the text file so we can update WhereAreWe

            for (int i = 0; i < thefile.Length; i++)
            {
                char Un = thechar[i];
                if (Un.Equals('\\'))
                {
                    Slasher = 1;
                }
                else
                {
                    if (Un.Equals('r') && Slasher == 1) //new lines are \r\n, so this runs if we are at the beginning of a new line statement
                    {
                        if (WhereAreWe == 0)//the line we just finished reading was the Name
                        {
                            temp.name = Adder;
                            Adder = "";//reset adder to an empty string
                            WhereAreWe = WhereAreWe + 1; //indicate that what we're reading is a new "CONCERN"
                        }
                        if (WhereAreWe == 1)//the line we just finished reading was the Phone#
                        {
                            temp.phone = Adder;
                            Adder = "";//reset adder to an empty string
                            WhereAreWe = WhereAreWe + 1; //indicate that what we're reading is a new "CONCERN"
                        }
                        if (WhereAreWe == 2)//the line we just finished reading was the Address
                        {
                            temp.location = Adder;
                            Adder = "";//reset adder to an empty string
                            WhereAreWe = WhereAreWe + 1; //indicate that what we're reading is a new "CONCERN"
                        }
                        if (WhereAreWe == 3)//the line we just finished reading was the Short Description, the last data feild for a resource
                        {
                            temp.info = Adder;
                            Adder = "";//reset adder to an empty string
                            ListResource.Add(temp);
                            WhereAreWe = 0;
                        }
                        i = i + 2; //we know that '\n' comes after '\r' so we can move i forward 2 to read whatever comes after '\r'
                        Slasher = 0; //reset our '\' 
                    }
                    else
                    {
                        if (Slasher == 1)
                        {
                            Adder.Insert(Adder.Length, "\\"); //insert the '\' we read last time but did nothing with
                        }
                        Adder.Insert(Adder.Length, Un.ToString()); //insert the character we are evaluating.
                    }

                }
              
            }
            //at this point ListResource should be populated
            //time to make the ListResource present itself with The name in a large font in it's own Text Box and with the other feild below in smaller fonts and text boxes
            int pos = 0;
            for (int x = 0; x < ListResource.Count; x++)
            {
                TextBlock Temp = new TextBlock();
                Temp.DataContext = temp.name;
                listBox.Resources.Add(pos, Temp);
                pos++;
                Temp.DataContext = temp.phone + "\n" + temp.location + "\n" + temp.info;
                Temp.Visibility = System.Windows.Visibility.Collapsed;
                listBox.Resources.Add(pos, Temp);
            }
        }



    }
    public class Resource
    {
        public Resource(string name, string phone, string location, string info)
        {
            this.name = name;
            this.phone = phone;
            this.info = info;
            this.location = location;
        }
        public string name { get; set; }
        public string phone { get; set; }
        public string info { get; set; }
        public string location { get; set; }
    }
}