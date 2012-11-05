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
             
        public Directory()
        {
            InitializeComponent(); //Load the Page

            readFromFile(); //Populate the Page
        }
       

        public void readFromFile()
        {
            List<Resource> ListResource = new List<Resource>(); //Initialize an empty list of resources
            string thefile = ""; //make an empty string object
            var resource = System.Windows.Application.GetResourceStream(new Uri("Resources/CampusResources.txt", UriKind.RelativeOrAbsolute)); //Grab the text File
            StreamReader SR = new StreamReader(resource.Stream);//Read the text file
            thefile = SR.ReadToEnd();//make the text file into a string
            char [] thechar = thefile.ToCharArray();//make the string into a array of characters
            Resource temp = new Resource ("", "", "", "");//make a temporary resource we will fill again and again to populate the list
            String Adder = "";//make a temporary string that we'll build by parsing through the char array and ultimate load into the different data fields of Resource
            int WhereAreWe = 0; //marks "CONCERNS" (Name, Phone, Location, and Info) with 0, 1, 2, and 3 respecfully.
            for (int i = 0; i < thefile.Length; i++) //iterate through the char array
            {
                char Un = thechar[i]; //make a temporary char named 'Un' for 'Unhandled' at index i in the char array
                if (Un.Equals('\r') || i == thefile.Length) //if we are at the end of the line or end of file
                {
                    i = i + 1;// we know the next line is a \n which we can skip over
                    if (WhereAreWe == 0)//the line we just finished reading was the Name
                    {
                        temp.name = Adder; //Load the string "Adder" into the name datafield
                        WhereAreWe = WhereAreWe + 1; //indicate that the next full string read will be phone#
                    }
                    else if (WhereAreWe == 1)//the line we just finished reading was the Phone#
                    {
                        temp.phone = Adder;//Load the string "Adder" into the phone# datafield
                        WhereAreWe = WhereAreWe + 1; //indicate that the next full string read will be location
                    }
                    else if (WhereAreWe == 2)//the line we just finished reading was the Address
                    {
                        temp.location = Adder;//Load the string "Adder" into the location datafield
                        WhereAreWe = WhereAreWe + 1; //indicate that the next full string read will be info
                    }
                     else if (WhereAreWe == 3)//the line we just finished reading was the Short Description, the last data feild for a resource
                    {
                        temp.info = Adder;//Load the string "Adder" into the info datafield
                        ListResource.Add(temp); //the temporary Resource now has all 4 data fields entered, time to put it into the List
                        
                         /*#########################################################
                          * now de-structing the resources into TextBlocks NM, PN, LC, IN for name, phone number, location, and info 
                          * #######################################################*/
                         TextBlock NM = new TextBlock(); //make a textblock for the name
                         NM.FontSize = 35; //the name should be noticiably larger than the other 3 data feilds
                         NM.Text = temp.name;//set the text of the textblock to the name of the resource we are currently handeling
                         NM.Tap += Name_Tap;//set the event for the name being tapped to Name_Tap
                         listBox.Items.Add(NM);//with the Textblock for name complete it is possible to move onto the next textblock and datafield
                         TextBlock PN = new TextBlock();//make a textblock for the phone#
                         PN.Text = temp.phone;//set the text of the textblock to the phone# of the resource we are currently handeling
                         PN.Visibility = System.Windows.Visibility.Collapsed;//by default this will not be visibile. 
                         PN.Tap += Phone_Tap;//set the event for the phone# being tapped to Phone_Tap
                         listBox.Items.Add(PN);//with the Textblock for phone# complete it is possible to move onto the next textblock and datafield
                         TextBlock LC = new TextBlock();//make a textblock for the location
                         LC.Text = temp.location;//set the text of the textblock to the location of the resource we are currently handeling
                         LC.Visibility = System.Windows.Visibility.Collapsed;//by default this will not be visibile. 
                         listBox.Items.Add(LC);//with the Textblock for location complete it is possible to move onto the next textblock and datafield
                         TextBlock IN = new TextBlock();//make a textblock for the info
                         IN.Text = temp.info;//set the text of the textblock to the info of the resource we are currently handeling
                         IN.Visibility = System.Windows.Visibility.Collapsed;//by default this will not be visibile. 
                         listBox.Items.Add(IN);//with the Textblock for info complete we have no more textblocks to make for this resource
                         WhereAreWe = 0;//the next full string read in will be the name of the next resource
                    }
                    Adder = "";//reset the temporary string 'Adder'

                }
                else //if we are not at the end of the file or end of a line
                {
                    Adder = Adder + Un.ToString();//add the read character to the temporary string
                }
            }

        }
        /*############################################################
         * METHODS METHODS METHODS METHODS METHODS AND A CLASS
         * #########################################################*/

        int pos = -1; //nothing is expanded yet
        public void Phone_Tap(object sender, System.Windows.Input.GestureEventArgs e)//method called in the event that the phone# textblock is tapped
        {
            int IndxSender = listBox.Items.IndexOf(sender);//store the index of the textblock which calls this event
            PhoneCallTask PhNum = new PhoneCallTask();//make a object of the PhoneCallTask Class named PhNum for phonenumber
            PhNum.DisplayName = ((TextBlock) listBox.Items.ElementAt(IndxSender -1)).Text;//this sets the name that will be displayed when the# is tapped
            PhNum.PhoneNumber = ((TextBlock) sender).Text;//this sets the number that will be displayed when the number is tapped. this number may be dialed if the user confirms their choice.
            PhNum.Show();//this simple line of code places the call
        }


        public void Name_Tap(object sender, System.Windows.Input.GestureEventArgs e)//method called in the event that the resources name is tapped and therefor the field underneath need expanding
        {
            int IndxSender = listBox.Items.IndexOf(sender);//store the index of the textblock which calls this event
            TextBlock Node = new TextBlock();//make a temporary Textblock named Node which we will use to iterate through all relevant textblocks below the one calling the method
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
                else//if something was opened prior to the opening of this resource
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
           
        }
    }
    public class Resource//this class holds the name, phone#, location, and a short description of most campus resources
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