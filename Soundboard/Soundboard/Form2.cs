//our second form, allows for loading
//todo: create new class to mute sounds

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Threading;

namespace Soundboard
{
    public partial class Form2 : Form
    {
        public int buttonindex = 0; //which main buttons are pressed.
        public int settingindex = 1; //which button of the setting (series) is pressed, e.g "Q"

        Button SubBtn; //substitute button
        public bool saved = false;

        //TEST
        SoundPlayer S = new SoundPlayer();

        //TEST

        //will be assigned to the loadData class variables when the user presses save.
        private string[] Q_Location = new string[9]; // e.g. Q[0] is "1" button on the q series
        private string[] A_Location = new string[9];
        private string[] Z_Location = new string[9];
        private string[] W_Location = new string[9];
        private string[] S_Location = new string[9];

        public Form2()
        {
            InitializeComponent();
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //default or first series up will be "Q"
            setSounds();
            initiateLoad();
            settingindex = 1;
            pressedLoadSetting(button12, 1); //simulate this
        }

        public void initiateLoad()
        {
            if (!isEmpty())
            {
                
                DialogResult dialogResult = MessageBox.Show("Keep existing sounds?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.None);
                if (dialogResult == DialogResult.Yes)
                {
                    setSounds();
                    //do something
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                    continueInit(); //double check
                }
            }

        }

        public bool isEmpty() //checks if the locations contain any sounds
        {
            bool empty = true;
            for (int i = 0; i < Q_Location.Length; i++)
            {
                if (Q_Location[i] != null)
                {
                    empty = false;
                    break;
                    
                }
                else if (A_Location[i] != null)
                {
                    empty = false;
                    break;

                }
                else if (Z_Location[i] != null)
                {
                    empty = false;
                    break;

                }
                else if (W_Location[i] != null)
                {
                    empty = false;
                    break;

                }
                else if (S_Location[i] != null)
                {
                    empty = false;
                    break;

                }

            }

            if (!empty)
            {
                return false;
            }
            else
            {
                return true;
            }
            

        }

        void setSounds() //transfers the sounds from form1
        {
            Form1.CopySoundsF1(Q_Location, A_Location, Z_Location, W_Location, S_Location);
        }

        void continueInit()
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure? (this will remove all existing sounds)", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.None);
            if (dialogResult == DialogResult.Yes)
            {
                removesounds();
                //do something
            }
            else if (dialogResult == DialogResult.No)
            {
                initiateLoad();
            }
        }

        void removesounds() //remove all sounds saved
        {
            Q_Location = new string[9];
            A_Location = new string[9];
            Z_Location = new string[9];
            W_Location = new string[9];
            S_Location = new string[9];
        }

        // SAVE =========================================================
        private void button2_Click(object sender, EventArgs e)
        {
            
            LoadData Dat = new LoadData(Q_Location, A_Location, Z_Location, W_Location, S_Location);

            //LoadData Dat = new LoadData();
            //Dat.Q = Q_Location;
            //Dat.A = A_Location;
            //Dat.Z = Z_Location;
            //Dat.W = W_Location;
            //Dat.S = S_Location;

            saved = true;
            LoadData.write();
            this.Close(); //works.



        }
        // ==============================================================

        // CANCEL =======================================================
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // ==============================================================



        // key functions ================================================

        public void pressedLoadBtn(Button b, int BtnIndex) //when press, begin load
        {

            OpenFileDialog open = new OpenFileDialog();
            
            
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = Path.GetExtension(open.FileName);

                if (ext == ".wav" || ext == ".mp3")
                {
                    b.Text = "FULL";
                    if (settingindex == 1) { Q_Location[BtnIndex] = open.FileName; } //Q
                    else if (settingindex == 2) { A_Location[BtnIndex] = open.FileName; } //A
                    else if (settingindex == 3) { Z_Location[BtnIndex] = open.FileName; } //Z
                    else if (settingindex == 4) { W_Location[BtnIndex] = open.FileName; } //W
                    else if (settingindex == 5) { S_Location[BtnIndex] = open.FileName; } //S
                }
                else
                {
                    MessageBox.Show("ERROR: file must be of .mp3 or .wav format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
                }



            }
                 
        }

        public void pressedLoadSetting(Button b, int setIndex) //when we press the setting buttons [q, a, z, w...]
        {
            //change the colour
            if (setIndex != settingindex && SubBtn != null) { SubBtn.BackColor = SystemColors.ButtonFace; } //if the current index is not equal to the previous one
            b.BackColor = Color.Red;
            SubBtn = b;
            settingindex = setIndex;

            //a new array which stores the urls for all the buttons in the corresponding button
            string[] currentSetting = new string[9];

            //check for the setting, set the array to the urls belonging to that setting
            if (settingindex == 1) { currentSetting = Q_Location; } //Q
            else if (settingindex == 2) { currentSetting = A_Location; } //A
            else if (settingindex == 3) { currentSetting = Z_Location; } //Z
            else if (settingindex == 4) { currentSetting = W_Location; } //W
            else if (settingindex == 5) { currentSetting = S_Location; } //S

            for (int i = 0; i < Q_Location.Length; i++) //loop through all the urls
            {
                if (currentSetting[i] == null || currentSetting[i] == "") //if the looped url is empty
                {
                    //find the button/place rendered empty and denote that on the button
                    switch(i){
                        case 0:
                            button8.Text = "EMPTY"; 
                            break;
                        case 1:
                            button10.Text = "EMPTY";
                            break;
                        case 2:
                            button11.Text = "EMPTY";
                            break;
                        case 3:
                            button7.Text = "EMPTY";
                            break;
                        case 4:
                            button6.Text = "EMPTY";
                            break;
                        case 5:
                            button5.Text = "EMPTY";
                            break;
                        case 6:
                            button4.Text = "EMPTY";
                            break;
                        case 7:
                            button3.Text = "EMPTY";
                            break;
                        case 8:
                            button9.Text = "EMPTY";
                            break;
                    }
                }
                else if (currentSetting[i] != null || currentSetting[i] != "") // or if it is full
                {
                    switch (i) //denote it
                    {
                        case 0:
                            button8.Text = "FULL";
                            break;
                        case 1:
                            button10.Text = "FULL";
                            break;
                        case 2:
                            button11.Text = "FULL";
                            break;
                        case 3:
                            button7.Text = "FULL";
                            break;
                        case 4:
                            button6.Text = "FULL";
                            break;
                        case 5:
                            button5.Text = "FULL";
                            break;
                        case 6:
                            button4.Text = "FULL";
                            break;
                        case 7:
                            button3.Text = "FULL";
                            break;
                        case 8:
                            button9.Text = "FULL";
                            break;
                    }
                }

            }
        }

        // ==============================================================

        //lanchpad Keys =================================================
   
        private void button8_Click(object sender, EventArgs e) { pressedLoadBtn(button8, 0); } //1

        private void button10_Click(object sender, EventArgs e) { pressedLoadBtn(button10, 1); } //2

        private void button11_Click(object sender, EventArgs e) { pressedLoadBtn(button11, 2); } //3

        private void button7_Click(object sender, EventArgs e) { pressedLoadBtn(button7, 3); } //4

        private void button6_Click(object sender, EventArgs e) { pressedLoadBtn(button6, 4); } //5

        private void button5_Click(object sender, EventArgs e) { pressedLoadBtn(button5, 5); } //6

        private void button4_Click(object sender, EventArgs e) { pressedLoadBtn(button4, 6); } //7

        private void button3_Click(object sender, EventArgs e) { pressedLoadBtn(button3, 7); } //8

        private void button9_Click(object sender, EventArgs e) { pressedLoadBtn(button9, 8); } //9

        // ===========================================================

        // setting keys ==============================================
        private void button12_Click(object sender, EventArgs e) //Q
        {
            pressedLoadSetting(button12, 1);
        }

        private void button14_Click(object sender, EventArgs e) //A
        {
            pressedLoadSetting(button14, 2);
        }

        private void button13_Click(object sender, EventArgs e) //Z
        {
            pressedLoadSetting(button13, 3);
        }

        private void button16_Click(object sender, EventArgs e) //W
        {
            pressedLoadSetting(button16, 4);
        }

        private void button15_Click(object sender, EventArgs e) //S
        {
            pressedLoadSetting(button15, 5);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //quick load:
            //load all audio from just a folder, the folder must contain subfolders each named Q, A, Z, W, S ... (or at least one. does not require all)
            //each subfolder must contain files named (in order 1,2,3,4,5,6... to corresponding KEY)
            // MAKE SURE TO ASK USER FOR OVERRIDE CHECK

            var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                /*
                string[] files = Directory.GetFiles(fbd.SelectedPath);

                System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                */
                string[] directories = Directory.GetDirectories(fbd.SelectedPath);

                if (directories.Length == 0)
                {
                    //empty folder
                    MessageBox.Show("No sub folders found! (make sure there are sub-folders containing your tracks)", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                else
                {
                    bool foundany = false;
                    for (int i = 0; i < directories.Length; i++)
                    {
                        //MessageBox.Show(Path.GetFileName(directories[i])); works
                        switch (Path.GetFileName(directories[i])) 
                        {
                            case "q":
                                foundany = true;
                                loadQuickWiz(1, directories[i]);
                                break;
                            case "a":
                                foundany = true;
                                loadQuickWiz(2, directories[i]);
                                break;
                            case "z":
                                foundany = true;
                                loadQuickWiz(3, directories[i]);
                                break;
                            case "w":
                                foundany = true;
                                loadQuickWiz(4, directories[i]);
                                break;
                            case "s":
                                foundany = true;
                                loadQuickWiz(5, directories[i]);
                                break;

                        }
                    }

                    if (foundany == false)
                    {
                        MessageBox.Show("Folders not named properly, name them 'q', 'a', 'z'... so that all sounds will be imported in properly", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                    else
                    {
                        MessageBox.Show("All done!", "Done", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                }
            }
        }

        public void loadQuickWiz(int settingi, string url)
        {
            

            switch (settingi) //refresh stuff
            {
                case 1:
                    pressedLoadSetting(button12, settingi);
                    break;
                case 2:
                    pressedLoadSetting(button14, settingi);
                    break;
                case 3:
                    pressedLoadSetting(button13, settingi);
                    break;
                case 4:
                    pressedLoadSetting(button16, settingi);
                    break;
                case 5:
                    pressedLoadSetting(button15, settingi);
                    break;
            }
            //settingindex = settingi;

            string[] fileEntries = Directory.GetFiles(url);
            string[] fileURL = new string[9];
            for (int i = 0; i < fileEntries.Length; i++) //we will loop through all the audio files.
            {
                string ext = Path.GetExtension(fileEntries[i]);

                if (ext == ".wav" || ext == ".mp3") //must be audio extension
                {
                    //MessageBox.Show(Path.GetFileNameWithoutExtension(fileEntries[i])); //works
                    switch (Path.GetFileNameWithoutExtension(fileEntries[i]))
                    {
                        case "1":
                            fileURL[0] = fileEntries[i];
                            button8.Text = "FULL";
                            break;
                        case "2":
                            fileURL[1] = fileEntries[i];
                            button10.Text = "FULL";
                            break;
                        case "3":
                            fileURL[2] = fileEntries[i];
                            button11.Text = "FULL";
                            break;
                        case "4":
                            fileURL[3] = fileEntries[i];
                            button7.Text = "FULL";
                            break;
                        case "5":
                            fileURL[4] = fileEntries[i];
                            button6.Text = "FULL";
                            break;
                        case "6":
                            fileURL[5] = fileEntries[i];
                            button5.Text = "FULL";
                            break;
                        case "7":
                            fileURL[6] = fileEntries[i];
                            button4.Text = "FULL";
                            break;
                        case "8":
                            fileURL[7] = fileEntries[i];
                            button3.Text = "FULL";
                            break;
                        case "9":
                            fileURL[8] = fileEntries[i];
                            button9.Text = "FULL";
                            break;

                    }
                }

                
            }



            if (settingindex == 1) { Array.Copy(fileURL, Q_Location, 9); } //Q
            else if (settingindex == 2) { Array.Copy(fileURL, A_Location, 9); } //A
            else if (settingindex == 3) { Array.Copy(fileURL, Z_Location, 9); } //Z
            else if (settingindex == 4) { Array.Copy(fileURL, W_Location, 9); } //W
            else if (settingindex == 5) { Array.Copy(fileURL, S_Location, 9); } //S



        }

        private void button18_Click(object sender, EventArgs e)
        {
            HELP h = new HELP();

            h.ShowDialog();
        }



        // ===========================================================


    }
}
