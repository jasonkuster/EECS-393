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
          
        /*
        //test case to see if i can even open/close the first resource description
        public void Expand(object sender, System.Windows.Input.GestureEventArgs e)
        {
			if (sender.Visibility.Equals("Visible")) //true = "Visibile"
			{
                T1.Visibility = System.Windows.Visibility.Collapsed;
			}
			else //if (T1.Visibility.Equals("Collapsed")) //false = "Collapsed"
			{
                T1.Visibility = System.Windows.Visibility.Visible;
			}
        }
         */


        //stuff added to try and make this event work
        //public delegate void MyEventHandle(object send);
        //public event MyEventHandle Tapper;
        //public event EventHandler Tap;



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
            int pos = 0;
            int WhereAreWe = 0; //marks "CONCERNS" (Name, Phone, Location, and Info) with 0, 1, 2, and 3 respecfully.
            //int Slasher = 0; //indicates if the last char read was a '\' so we know when a new line was made in the text file so we can update WhereAreWe

            for (int i = 0; i < thefile.Length; i++)
            {
                char Un = thechar[i];
                if (Un.Equals('\r') || i == thefile.Length) //if we are at the end of the line or end of file
                {
                    i = i + 1;// we know the next line is a \n which we can skip over
                    if (WhereAreWe == 0)//the line we just finished reading was the Name
                    {
                        temp.name = Adder;
                        //Adder = "";//reset adder to an empty string
                        WhereAreWe = WhereAreWe + 1; //indicate that what we're reading is a new "CONCERN"
                    }
                    else if (WhereAreWe == 1)//the line we just finished reading was the Phone#
                    {
                        temp.phone = Adder;
                        //Adder = "";//reset adder to an empty string
                        WhereAreWe = WhereAreWe + 1; //indicate that what we're reading is a new "CONCERN"
                    }
                    else if (WhereAreWe == 2)//the line we just finished reading was the Address
                    {
                        temp.location = Adder;
                       // Adder = "";//reset adder to an empty string
                        WhereAreWe = WhereAreWe + 1; //indicate that what we're reading is a new "CONCERN"
                    }
                     else if (WhereAreWe == 3)//the line we just finished reading was the Short Description, the last data feild for a resource
                    {
                        temp.info = Adder;
                        //Adder = "";//reset adder to an empty string
                        ListResource.Add(temp);
                        //listBox.i.Add(temp.name, temp.name);
                        //listBox.FontSize = 40;
                        //using (listBox.FontSize = 30)
                        TextBlock NM = new TextBlock();
                         NM.FontSize = 35;
                         NM.Text = temp.name;

                         //trying to add the event for expansion, I one of these is close

                        // NM.AddHandler(Name_Tap, new TouchFrameEventHandler(Name_Tap), true);

                         NM.Tap += Name_Tap;

                         //NM.IsReadOnly = true;
                         listBox.Items.Add(NM);
                        //pos++;
                        //listBox.Resources.Add(temp.phone + "\n" + temp.location + "\n" + temp.info, temp.phone + "\n" + temp.location + "\n" + temp.info);
                         TextBlock PN = new TextBlock();
                         PN.Text = temp.phone;
                         PN.Visibility = System.Windows.Visibility.Collapsed;
                         listBox.Items.Add(PN);
                         TextBlock LC = new TextBlock();
                         LC.Text = temp.location;
                         LC.Visibility = System.Windows.Visibility.Collapsed;
                         listBox.Items.Add(LC);
                         TextBlock IN = new TextBlock();
                         IN.Text = temp.info;
                         IN.Visibility = System.Windows.Visibility.Collapsed;
                         listBox.Items.Add(IN);
                        
                         WhereAreWe = 0;
                    }
                    Adder = "";

                }
                else
                {
                    Adder = Adder + Un.ToString();
                }
            }
            //EVERYTHING IS READ AND SET UP :D
            
        }
        int pos = -1; //nothing is expanded yet


        


        public void Name_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int IndxSender = listBox.Items.IndexOf(sender);
            TextBlock Node = new TextBlock();
            Node = (TextBlock) listBox.Items.ElementAt(IndxSender + 1); //node now points to the textbox below what we selected
            if (Node.Visibility == System.Windows.Visibility.Collapsed) //if this resource is collapsed
            {
                //Make the 3 fields (Phone, Location, Info) below the name visible
                Node.Visibility = System.Windows.Visibility.Visible;
                Node = (TextBlock)listBox.Items.ElementAt(IndxSender + 2);
                Node.Visibility = System.Windows.Visibility.Visible;
                Node = (TextBlock)listBox.Items.ElementAt(IndxSender + 3);
                Node.Visibility = System.Windows.Visibility.Visible;
                if (pos == -1) //if nothing else is open
                {
                    pos = IndxSender; //let it be known that this is the last opened resource
                }
                else
                {
                    //close the last opened resource
                    Node = (TextBlock)listBox.Items.ElementAt(pos + 1);
                    Node.Visibility = System.Windows.Visibility.Collapsed;
                    Node = (TextBlock)listBox.Items.ElementAt(pos + 2);
                    Node.Visibility = System.Windows.Visibility.Collapsed;
                    Node = (TextBlock)listBox.Items.ElementAt(pos + 3);
                    Node.Visibility = System.Windows.Visibility.Collapsed;
                    pos = IndxSender; //let it be known that this is the last opened resource
                }
            }
            else//if the Node below the sender is visible then this resource is opened already and we need to close it
            {
                Node.Visibility = System.Windows.Visibility.Collapsed;
                Node = (TextBlock)listBox.Items.ElementAt(IndxSender + 2);
                Node.Visibility = System.Windows.Visibility.Collapsed;
                Node = (TextBlock)listBox.Items.ElementAt(IndxSender + 3);
                Node.Visibility = System.Windows.Visibility.Collapsed;
                pos = -1;
            }
            /*
            //#########################
            int Mario = listBox.Items.IndexOf(sender);
            TextBlock Lugie = new TextBlock();
            Lugie = (TextBlock) listBox.Items.ElementAt(Mario + 1);
            if (Lugie.Visibility == System.Windows.Visibility.Collapsed) //if the name was selected and not yet expanded
            {
                Lugie.Visibility = System.Windows.Visibility.Visible;
                Mario++;
                Lugie = (TextBlock) listBox.Items.ElementAt(Mario);
                Lugie.Visibility = System.Windows.Visibility.Visible;
                Mario++;
                Lugie = (TextBlock) listBox.Items.ElementAt(Mario);
                Lugie.Visibility = System.Windows.Visibility.Visible;
                if (pos != -1)
                {
                    pos = Mario - 3; //keeps track of this, the index of the expanded name
                }
                else //if there is another resource expanded, collapse it all
                {
                    Lugie = (TextBlock)listBox.Items.ElementAt(pos + 1);
                    Lugie.Visibility = System.Windows.Visibility.Collapsed;
                    pos++;
                    Lugie.Visibility = System.Windows.Visibility.Collapsed;
                    pos++;
                    Lugie.Visibility = System.Windows.Visibility.Collapsed;
                    pos = Mario - 3;//keeps track of this, the index of the expanded name
                }
            }
            else //if this name is already expanded, close it
            {
                //Lugie = (TextBlock)listBox.Items.ElementAt(Mario + 1);
                Lugie.Visibility = System.Windows.Visibility.Collapsed;
                Mario++;
                Lugie.Visibility = System.Windows.Visibility.Collapsed;
                Mario++;
                Lugie.Visibility = System.Windows.Visibility.Collapsed;
                pos = -1;
            }*/
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