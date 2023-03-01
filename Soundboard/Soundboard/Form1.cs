/* Soundboard 1.0 by clarence yang 31/3/19
 * this project includes currently (as of 6/4/19) 3 classes including this one.
 * 
 * CODE CLEANUP AND FUNCTIONAL UPGRADE 22/06/22
 *  - fix audio playback - change from .wav with mp3 compatibility 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Input;
using System.Media;
using System.IO;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Diagnostics;
using AudioSwitcher.AudioApi.CoreAudio;
using ScottPlot;
using NAudio.CoreAudioApi;
using System.Numerics;


namespace Soundboard
{
    public partial class Form1 : Form
    {
        //essential variables
        static int currrentSetting = 1; //setting (series) keys: series 1 = q key
        int buttonindex; //which button is selected
        bool emptyLocation = false;

        public Button subBtn;

        public int currentChannel = 1;

        bool pressRec = false;
        WasapiLoopbackCapture CaptureInstance = new WasapiLoopbackCapture();

        //the file destination/urls for all the sounds
        //correction: only index 8 because nine buttons 012345678
        private static string[] Q_Loc = new string[9]; // e.g. Q[0] is "1" button on the q series
        private static string[] A_Loc = new string[9];
        private static string[] Z_Loc = new string[9];
        private static string[] W_Loc = new string[9];
        private static string[] S_Loc = new string[9];

        private static ToolTip t;
        private static Button b1;
        private static Button b2;
        private static Button b3;
        private static Button b4;
        private static Button b5;
        private static Button b6;
        private static Button b7;
        private static Button b8;
        private static Button b9;

        public List<System.Windows.Media.MediaPlayer> channel1 = new List<System.Windows.Media.MediaPlayer>();
        public List<System.Windows.Media.MediaPlayer> channel2 = new List<System.Windows.Media.MediaPlayer>();

        //all the mediaplayers, one for each button: should make array instead
        System.Windows.Media.MediaPlayer Sp1 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer Sp2 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer Sp3 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer Sp4 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer Sp5 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer Sp6 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer Sp7 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer Sp8 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer Sp9 = new System.Windows.Media.MediaPlayer();


        //media players for the recorded slots - will loop
        System.Windows.Media.MediaPlayer S1 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer S2 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer S3 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer S4 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer S5 = new System.Windows.Media.MediaPlayer();
        System.Windows.Media.MediaPlayer S6 = new System.Windows.Media.MediaPlayer();
        public string path = @"C:\SoundboardRec\"; // let user change this location 
        public bool canSave = false;
        public bool enableset = false;
        public string[] url = new string[6];
        public int ind = 0;
        public bool[] occupied = new bool[6]; //


        bool[] ArrayBegin = new bool[9];
        bool[] isPlayingArray = new bool[9];


        public float channel1vol = 1f;
        public float channel2vol = 0f;
        public float channel1volsmol = 1f;
        public float channel2volsmol = 1f;

        public bool channel1Playing = true;
        public bool channel2Playing = true;

        // prepare class objects
        public BufferedWaveProvider bwp;

        //datetime
        DateTime setTimeAud;

        public int deviceInd = 0; //for spectrum output

        public bool inititated = false;

        public int consoleMode = 0;
        public string consoledat = "";

        public Form1()
        {

            InitializeComponent();

            //spectrum output
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            comboBox1.Items.AddRange(devices.ToArray());
            comboBox1.SelectedIndex = 1;
        }


        private void Form1_Load(object sender, EventArgs e) //what to do on load
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            comboBox2.SelectedIndex = 0;

            //this.Activate();
            Output("loading soundboard");
            currrentSetting = 1; //the load out, e.g. Q, A, Z, etc.
            pressSettingBtn(button10, 1); //default load is q


            button17.BackColor = Color.Orange;
            button28.Image = Properties.Resources.video_pause_button__1_;
            button28.BackColor = Color.Orange;
            label3.Text = "NOT RECORDING"; //reset text

            assign_controls(toolTip1, button7, button5, button3, button4, button8, button1, button9, button6, button2);

        }

        public static void assign_controls(ToolTip T, Button B7, Button B5, Button B3, Button B4, Button B8, Button B1, Button B9, Button B6, Button B2)
        {
            t = T;
            b1 = B1;
            b2 = B2;
            b3 = B3;
            b4 = B4;
            b5 = B5;
            b6 = B6;
            b7 = B7;
            b8 = B8;
            b9 = B9;
        }

        async void Form1_Shown(object sender, EventArgs e)
        {
            Button[] buttons = { button9, button6, button2, button1, button4, button8, button7, button5, button3 };
            //set the array's content 
            for (int i = 0; i < isPlayingArray.Length; i++) //NOTE: the arrays are of constant length: 8 {0, 1, 2, 3, 4, 5, 6, 7, 8} totalling with nine values (one for each button), this means they can share one length in for loop.
            {
                
                await Task.Run(() =>
                {
                    buttons[i].BackColor = Color.Red;
                    ArrayBegin[i] = false; //just set this
                    isPlayingArray[i] = false; //not playing sound
   
                    
                });
                await Task.Delay(100);
                //Thread.Sleep(500);
                Output("Setting content " + (i+1).ToString() + "/" + isPlayingArray.Length.ToString());
                buttons[i].BackColor = SystemColors.ControlLightLight;

            }



        }

        //sets the contents of our output
        public void Output(string logText)
        {
            consoledat += "[PROGRAM] - " + logText + "\n";
            if (consoleMode == 0)
            {
                
                OutputBox.Text = consoledat;
            }


        }

        


        // keyboard input ==============================================================
        private void Form1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) //keyboard
        {

            if (e.KeyCode == Keys.NumPad1) { unclickbutton(button7); }
            else if (e.KeyCode == Keys.NumPad2) { unclickbutton(button5); }
            else if (e.KeyCode == Keys.NumPad3) { unclickbutton(button3); }
            else if (e.KeyCode == Keys.NumPad4) { unclickbutton(button8); }
            else if (e.KeyCode == Keys.NumPad5) { unclickbutton(button4); }
            else if (e.KeyCode == Keys.NumPad6) { unclickbutton(button1); }
            else if (e.KeyCode == Keys.NumPad7) { unclickbutton(button9); }
            else if (e.KeyCode == Keys.NumPad8) { unclickbutton(button6); }
            else if (e.KeyCode == Keys.NumPad9) { unclickbutton(button2); }

        }

        private void Form1_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e) //keyboard
        {
            //keypad buttons
            if (e.KeyCode == Keys.NumPad1) { pressedbutton(button7, Sp1); }
            else if (e.KeyCode == Keys.NumPad2) { pressedbutton(button5, Sp2); }
            else if (e.KeyCode == Keys.NumPad3) { pressedbutton(button3, Sp3); }
            else if (e.KeyCode == Keys.NumPad4) { pressedbutton(button8, Sp4); }
            else if (e.KeyCode == Keys.NumPad5) { pressedbutton(button4, Sp5); }
            else if (e.KeyCode == Keys.NumPad6) { pressedbutton(button1, Sp6); }
            else if (e.KeyCode == Keys.NumPad7) { pressedbutton(button9, Sp7); }
            else if (e.KeyCode == Keys.NumPad8) { pressedbutton(button6, Sp8); }
            else if (e.KeyCode == Keys.NumPad9) { pressedbutton(button2, Sp9); }

            //setting buttons (series)
            
            if (e.KeyCode == Keys.Q) { pressSettingBtn(button10, 1); } //q
            else if (e.KeyCode == Keys.A) { pressSettingBtn(button11, 2); } //a
            else if (e.KeyCode == Keys.Z) { pressSettingBtn(button12, 3); } //z
            else if (e.KeyCode == Keys.W) { pressSettingBtn(button14, 4); } //w
            else if (e.KeyCode == Keys.S) { pressSettingBtn(button15, 5); } //s

            //cancel save
            else if (e.KeyCode == Keys.E) { cancelSave(); } //s
        }
        // ================================================================================

        // transfers the new loaded sounds to the main form
        public static void ReadDATA(string[] Qloc, string[] Aloc, string[] Zloc, string[] Wloc, string[] Sloc)
        {

            Array.Copy(Qloc, Q_Loc, 9);
            Array.Copy(Aloc, A_Loc, 9);
            Array.Copy(Zloc, Z_Loc, 9);
            Array.Copy(Wloc, W_Loc, 9);
            Array.Copy(Sloc, S_Loc, 9);

            //works!
            //data succesfully transfers from class Load Data to form 1
            //MessageBox.Show("locations is: " + Q_Loc[6]); //<-- this works
            refresh_tooltips(t, b7, b5, b3, b4, b8, b1, b9, b6, b2);


        }

        //transfers the original existing audio files to form2 where they may be saved
        public static void CopySoundsF1(string[] Qloc, string[] Aloc, string[] Zloc, string[] Wloc, string[] Sloc)
        {
            Array.Copy(Q_Loc, Qloc, 9);
            Array.Copy(A_Loc, Aloc, 9);
            Array.Copy(Z_Loc, Zloc, 9);
            Array.Copy(W_Loc, Wloc, 9);
            Array.Copy(S_Loc, Sloc, 9);
        }



        //button functions and audio
        public void pressedbutton(Button b, System.Windows.Media.MediaPlayer S) //change color of button when key or button is pressed
        {
            //confirm();


            /*
            if (LoadData.saved)
            {
                LoadData.saved = false;
                LoadData.write();
            }
            */
            
            if (S == null)
                MessageBox.Show("error, soundplayer not defined", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);

            //MessageBox.Show(Q_Loc[6]); //works!

            //if (Q_Loc[6] == null)
            //{
                //MessageBox.Show("Q location doesnt exist");
            //}
            //else
            //{
                //MessageBox.Show(Q_Loc[6]); //<-- but this doesnt
            //}
     
            b.BackColor = Color.Red;


            //which button is pressed
            if (b == button7) { buttonindex = 0; }
            else if (b == button5) { buttonindex = 1; }
            else if (b == button3) { buttonindex = 2; }
            else if (b == button8) { buttonindex = 3; }
            else if (b == button4) { buttonindex = 4; }
            else if (b == button1) { buttonindex = 5; }
            else if (b == button9) { buttonindex = 6; }
            else if (b == button6) { buttonindex = 7; }
            else if (b == button2) { buttonindex = 8; }

            //MessageBox.Show(currrentSetting.ToString() + " " + buttonindex.ToString());
            //MessageBox.Show(buttonindex.ToString());
            //MessageBox.Show(combine(Q_Loc[buttonindex]));
            string location = "";

            

            //MessageBox.Show(buttonindex.ToString());
            if (currrentSetting == 1 && (Q_Loc[buttonindex] != null)) { location = @Q_Loc[buttonindex]; } //q
            else if (currrentSetting == 2 && A_Loc[buttonindex] != null) { location = @A_Loc[buttonindex]; } //a
            else if (currrentSetting == 3 && Z_Loc[buttonindex] != null) { location = @Z_Loc[buttonindex]; } //z
            else if (currrentSetting == 4 && W_Loc[buttonindex] != null) { location = @W_Loc[buttonindex]; } //w
            else if (currrentSetting == 5 && S_Loc[buttonindex] != null) { location = @S_Loc[buttonindex]; } //s
            else 
            {
                
                emptyLocation = true;
            }
            //play


            if (emptyLocation == false)
            {
                try
                {
                    //S.Load();
                    //S.Play();
                    if (currentChannel == 1)
                    {
                        channel1.Add(S);
                        S.Volume = channel1vol * channel1volsmol;
                    }
                    else
                    {
                        channel2.Add(S);
                        S.Volume = channel2vol * channel2volsmol;
                    }
                    
                    S.Open(new System.Uri(location));
                    S.Play();

                    //if (fadeInEnabled)
                        //fadeIN(buttonindex);
                    //ArrayLastSounds[buttonindex] = location;
                    

                    //lastSound = location; //to fade out

                    if (!ArrayBegin[buttonindex]) //a check, is the sound not playing currently?
                    {
                        //ArrayStopwatches[buttonindex].Start(); //make array
                        //myTimer.Start();
                        ArrayBegin[buttonindex] = true;
                        //timerArray[buttonindex].Start();
                        isPlayingArray[buttonindex] = true; //the sound is playing
                        
                    }
                    else //the sound is already playing
                    {
                        //if the sound originally was playing, reset the timers and stopwatch
                        //timerArray[buttonindex].Start();
                        //timerArray[buttonindex].Stop();

                        //reset stopwatch
                        //ArrayStopwatches[buttonindex].Stop();
                        //ArrayStopwatches[buttonindex].Reset();
                        //ArrayStopwatches[buttonindex].Start();

                        isPlayingArray[buttonindex] = true; //pressing the button again while sound is still playing makes no difference; it is still playing.
                        ArrayBegin[buttonindex] = false;
                    }

                    //get seconds in clip
                    //TimeSpan span = GetWavFileDuration(location);
                    //ArraySpanSeconds[buttonindex] = span.TotalSeconds.ToString();
                    //string[] spanSecondsArray = ArraySpanSeconds[buttonindex].Split('.');
                    //ArraySpanSeconds[buttonindex] = spanSecondsArray[0].ToString(); //only get seconds


                }
                catch (Win32Exception e)
                {
                    //MessageBox.Show(e.Message);
                    Output(e.Message);
                }

            }
            else
            {
                //no sounds loaded or it failed
                //MessageBox.Show("failed");
                emptyLocation = false;
            }

        }

        public string combine(string url)
        {
            string newUrl = Path.Combine("@", url);
            return newUrl;
        }


        public void unclickbutton(Button b) //unclick (work more on this)
        {
            b.BackColor = SystemColors.ControlLightLight;
        } 

        public static void refresh_tooltips(ToolTip t, Button b7, Button b5, Button b3, Button b4, Button b8, Button b1, Button b9, Button b6, Button b2)
        {
            string[] urls = new string[9];
            switch (currrentSetting)
            {
                case 1:
                    Array.Copy(Q_Loc, urls, 9);
                    break;
                case 2:
                    Array.Copy(A_Loc, urls, 9);
                    break;
                case 3:
                    Array.Copy(Z_Loc, urls, 9);
                    break;
                case 4:
                    Array.Copy(W_Loc, urls, 9);
                    break;
                case 5:
                    Array.Copy(S_Loc, urls, 9);
                    break;
            }

            for (int i = 0; i < urls.Length; i++)
            {
                if (urls[i] == "" || urls[i] == null)
                {
                    urls[i] = "Empty";
                }
            }

            t.SetToolTip(b7, urls[0]);
            t.SetToolTip(b5, urls[1]);
            t.SetToolTip(b3, urls[2]);
            t.SetToolTip(b8, urls[3]);
            t.SetToolTip(b4, urls[4]);
            t.SetToolTip(b1, urls[5]);
            t.SetToolTip(b9, urls[6]);
            t.SetToolTip(b6, urls[7]);
            t.SetToolTip(b2, urls[8]);
        }


        public void pressSettingBtn(Button b, int currentSet)
        {
            if (currentSet != currrentSetting && subBtn != null) { subBtn.BackColor = SystemColors.ControlLightLight; }
            b.BackColor = Color.Red;
            subBtn = b;
            currrentSetting = currentSet;

            refresh_tooltips(toolTip1, button7, button5, button3, button4, button8, button1, button9, button6, button2);

        }

        //public void clickbutton (Button b) //click button programmatically 
        //{
        // b.PerformClick();
        //}

        void stopAll()
        {
            
            Sp1.Stop();
            Sp2.Stop();
            Sp3.Stop();
            Sp4.Stop();
            Sp5.Stop();
            Sp6.Stop();
            Sp7.Stop();
            Sp8.Stop();
            Sp9.Stop();

            channel1.Clear();
            channel2.Clear();
          
            Output("All sounds stopped.");
        }

        //buttons
        void pauseAll()
        {
            if (currentChannel == 1)
            {
                for (int i = 0; i < channel1.Count; i++)
                {
                    channel1[i].Pause();
                }
            }
            else
            {
                for (int i = 0; i < channel2.Count; i++)
                {
                    channel2[i].Pause();
                }
            }
                
        }

        void playAll()
        {
            if (currentChannel == 1)
            {
                for (int i = 0; i < channel1.Count; i++)
                {
                    channel1[i].Play();
                }
            }
            else
            {
                for (int i = 0; i < channel2.Count; i++)
                {
                    channel2[i].Play();
                }
            }
        }


        //lauchpad keys (buttons) =========================================================

        //1
        private void button7_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { pressedbutton(button7, Sp1); }
        private void button7_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { unclickbutton(button7); }

        //2
        private void button5_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { pressedbutton(button5, Sp2); }
        private void button5_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { unclickbutton(button5); }

        //3
        private void button3_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { pressedbutton(button3, Sp3); }
        private void button3_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { unclickbutton(button3); }

        //4
        private void button8_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { pressedbutton(button8, Sp4); }
        private void button8_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { unclickbutton(button8); }

        //5
        private void button4_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { pressedbutton(button4, Sp5); }
        private void button4_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { unclickbutton(button4); }

        //6
        private void button1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { pressedbutton(button1, Sp6); }
        private void button1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { unclickbutton(button1); }

        //7
        private void button9_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { pressedbutton(button9, Sp7); } //
        private void button9_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { unclickbutton(button9); }

        //8
        private void button6_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { pressedbutton(button6, Sp8); }
        private void button6_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { unclickbutton(button6); }

        //9
        private void button2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { pressedbutton(button2, Sp9); }
        private void button2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { unclickbutton(button2); }

        // ================================================================================

        //setting keys

        private void button10_Click(object sender, EventArgs e) { pressSettingBtn(button10, 1); } //Q

        private void button11_Click(object sender, EventArgs e) { pressSettingBtn(button11, 2); } //A

        private void button12_Click(object sender, EventArgs e) { pressSettingBtn(button12, 3); } //Z

        private void button14_Click(object sender, EventArgs e) { pressSettingBtn(button14, 4); } //W

        private void button15_Click(object sender, EventArgs e) { pressSettingBtn(button15, 5); } //S


        //sound options
        private void loadsoundBtn_Click(object sender, EventArgs e)
        {
            //
            Output("Loading audio - pick a file");
            Form2 f2 = new Form2();
            f2.ShowDialog();




        }

        private void button13_Click(object sender, EventArgs e)
        {

                
            if (pressRec)
            {
          
                stopRecord();
                pressRec = false;
            }
            else
            {
                pressRec = true;
                Record();
            }
            
            
        }


        private void Record()
        {
            // Define the output wav file of the recorded audio
            Output("RECORD: choose a file");
            string outputFilePath = "";

            SaveFileDialog fbd = new SaveFileDialog();
            fbd.Filter = "Wave file (*.wav)|*.wav";
            fbd.DefaultExt = "wav";
            fbd.AddExtension = true;
            DialogResult result = fbd.ShowDialog();


            if (result == DialogResult.OK)
            {

                outputFilePath = @Path.GetFullPath(fbd.FileName);

                label3.Text = "RECORDING";
                Output("RECORDING");
                setTimeAud = DateTime.Now;
            }
            else
            {
                Output("Recording cancelled.");
                return;
            }


            // Redefine the capturer instance with a new instance of the LoopbackCapture class
            CaptureInstance = new WasapiLoopbackCapture();

            // Redefine the audio writer instance with the given configuration
            WaveFileWriter RecordedAudioWriter = new WaveFileWriter(outputFilePath, CaptureInstance.WaveFormat);

            // When the capturer receives audio, start writing the buffer into the mentioned file
            CaptureInstance.DataAvailable += (s, a) =>
            {
                // Write buffer into the file of the writer instance
                RecordedAudioWriter.Write(a.Buffer, 0, a.BytesRecorded);
            };

            // When the Capturer Stops, dispose instances of the capturer and writer
            CaptureInstance.RecordingStopped += (s, a) =>
            {
                RecordedAudioWriter.Dispose();
                RecordedAudioWriter = null;
                CaptureInstance.Dispose();
            };

            // Start audio recording !
            CaptureInstance.StartRecording();

            
        }

        private void stopRecord()
        {
            CaptureInstance.StopRecording();
            label3.Text = "NOT RECORDING";
            Output("Stopped Recording Audio");
            label21.Text = "00:00:00";
        }

 
        //stop button
        private void button16_Click(object sender, EventArgs e)
        {
            stopAll();
        }


        
        //help
        private void button17_Click(object sender, EventArgs e)
        {

           
            
        }





        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }



        private void button28_Click(object sender, EventArgs e)
        {
            

            if (currentChannel == 1)
            {
                if (channel1Playing)
                {
                    channel1Playing = false;
                    pauseAll(); //will check for which channel to pause
                    button28.Image = Properties.Resources.play_button__1_;
                }
                else
                {
                    channel1Playing = true;
                    playAll();
                    button28.Image = Properties.Resources.video_pause_button__1_;
                }
            }
            else
            {
                if (channel2Playing)
                {
                    channel2Playing = false;
                    pauseAll();
                    button28.Image = Properties.Resources.play_button__1_;
                }
                else
                {
                    channel2Playing = true;
                    playAll();
                    button28.Image = Properties.Resources.video_pause_button__1_;
                }
            }
            
        }


        private void button18_Click(object sender, EventArgs e) //will record and repeat
        {
            bool vacant = false;
            if (!canSave)
            {
                
                canSave = true;
                

                for (int i = 0; i < url.Length; i++)
                {
                    if (url[i] == "" || url[i] == null)
                    {
                        vacant = true;
                       
                        break;

                    }
                }

                if (vacant)
                {
                    Output("Now recording! Press once more to finish recording");

                }
                else
                {
                    Output("Recording, NOTE: all slots are filled, prepare to override");
                }


                // Define the output wav file of the recorded audio


                try
                {
                    // Determine whether the directory exists.
                    if (!Directory.Exists(path))
                    {
                        // Try to create the directory.
                        DirectoryInfo di = Directory.CreateDirectory(path);
                    }



                }
                catch
                {
                    Output("error recording!");
                    return;
                }
                string outputFilePath = path + ind + ".wav";
                url[ind] = outputFilePath;
                ind++;

                // Redefine the capturer instance with a new instance of the LoopbackCapture class
                CaptureInstance = new WasapiLoopbackCapture();
                WaveFileWriter RecordedAudioWriter = new WaveFileWriter(outputFilePath, CaptureInstance.WaveFormat);
                // Redefine the audio writer instance with the given configuration              

                // When the capturer receives audio, start writing the buffer into the mentioned file
                CaptureInstance.DataAvailable += (s, a) =>
                {
                    // Write buffer into the file of the writer instance
                    RecordedAudioWriter.Write(a.Buffer, 0, a.BytesRecorded);
                };

                // When the Capturer Stops, dispose instances of the capturer and writer
                CaptureInstance.RecordingStopped += (s, a) =>
                {
                    RecordedAudioWriter.Dispose();
                    RecordedAudioWriter = null;
                    CaptureInstance.Dispose();
                };

                // Start audio recording !
                CaptureInstance.StartRecording();
            }
            else
            {
                
                CaptureInstance.StopRecording();
                //label3.Text = "NOT RECORDING";
                if (vacant)
                {
                    Output("Stopped Recording Audio, click on any slot to play and loop or press 'e' to cancel");

                }
                else
                {
                    Output("Stopped Recording Audio, all slots are occupied click on any slot to override or press 'e' to cancel");
                }
                for (int i = 0; i < occupied.Length; i++)
                {
                    if (!occupied[i])
                    {
                        switch (i)
                        {
                            case 0:
                                button22.BackColor = Color.FromArgb(112, 251, 240);
                                break;
                            case 1:
                                button23.BackColor = Color.FromArgb(112, 251, 240);
                                break;
                            case 2:
                                button24.BackColor = Color.FromArgb(112, 251, 240);
                                break;
                            case 3:
                                button25.BackColor = Color.FromArgb(112, 251, 240);
                                break;
                            case 4:
                                button26.BackColor = Color.FromArgb(112, 251, 240);
                                break;
                            case 5:
                                button29.BackColor = Color.FromArgb(112, 251, 240);
                                break;
                        }
                    }
                    

                }

            }
            

        }

        public void cancelSave()
        {
            if (canSave)
            {
                canSave = false;
                Output("Cancelled recording! File will be deleted");
                ind--;
                if (File.Exists(url[ind]))
                {
                    File.Delete(url[ind]);
                }

                //change this
                button22.BackColor = SystemColors.ControlLightLight;
                button23.BackColor = SystemColors.ControlLightLight;
                button24.BackColor = SystemColors.ControlLightLight;
                button25.BackColor = SystemColors.ControlLightLight;
                button26.BackColor = SystemColors.ControlLightLight;
                button29.BackColor = SystemColors.ControlLightLight;
            }
        }

        //sound slots
        private void button22_Click(object sender, EventArgs e) 
        {
            pressPlayRecBTN(button22, S1);
        } //slot1

        private void button23_Click(object sender, EventArgs e)
        {
            pressPlayRecBTN(button23, S2);
        } //slot 2

        private void button24_Click(object sender, EventArgs e)
        {
            pressPlayRecBTN(button24, S3);
        } //slot 3

        private void button25_Click(object sender, EventArgs e)
        {
            pressPlayRecBTN(button25, S4);
        } //slot 4

        private void button26_Click(object sender, EventArgs e)
        {
            pressPlayRecBTN(button26, S5);
        } //slot 5
        private void button29_Click(object sender, EventArgs e)
        {
            pressPlayRecBTN(button29, S5);
        }//slot 6

        public void pressPlayRecBTN(Button b, System.Windows.Media.MediaPlayer s)
        {
            if (canSave)
            {

            }
        }



        //tool strips
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) //about
        {
            AboutBox1 s = new AboutBox1();
            s.ShowDialog();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpMain h = new HelpMain();
            h.ShowDialog();
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void loadSoundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Output("Loading audio - pick a file");
            Form2 f2 = new Form2();
            f2.ShowDialog();
        }

        
        

        private void timer1_Tick(object sender, EventArgs e) //timer
        {
            
            if (consoleMode == 1)
            {
                OutputBox.Text = "";


            }

            if (pressRec)
            {
                TimeSpan elapesd = DateTime.Now - setTimeAud;
                //change the stuff here
                label21.Text = string.Format("{0:00}:{1:00}:{2:00}", elapesd.Hours, elapesd.Minutes, elapesd.Seconds);
            }

            if (comboBox1.SelectedItem != null)
            {
                var device = (MMDevice)comboBox1.SelectedItem;
                progressBar1.Value = (int)(Math.Round(device.AudioMeterInformation.PeakValues[0] * 100)); // L Channel.
                progressBar2.Value = (int)(Math.Round(device.AudioMeterInformation.PeakValues[1] * 100)); // R Channel.
            }

            if (channel1.Count != 0)
            {
                if (consoleMode == 1)
                {

            
                    OutputBox.SelectionColor = Color.Orange;
                    OutputBox.AppendText("Channel 1");
                    OutputBox.AppendText(Environment.NewLine);
                }
                for (int i = 0; i < channel1.Count; i++)
                {

                    //Output(channel1[i].Source.ToString());
                    NAudio.Wave.WaveFileReader reader = new NAudio.Wave.WaveFileReader(channel1[i].Source.ToString().Replace("file:///", ""));

                    if (consoleMode == 1)
                    {


                        OutputBox.SelectionColor = Color.Black;
                        OutputBox.AppendText(Path.GetFileName(channel1[i].Source.LocalPath));
                        
                        OutputBox.AppendText(" "+((int)channel1[i].Position.TotalSeconds).ToString() + "/" + ((int)reader.TotalTime.TotalSeconds).ToString());
                        OutputBox.AppendText(Environment.NewLine);
                    }

                    //Output(((int)channel1[i].Position.TotalSeconds).ToString() + "/" + ((int)reader.TotalTime.TotalSeconds).ToString());
                    if ((int)channel1[i].Position.TotalSeconds >= (int)reader.TotalTime.TotalSeconds)
                    {
                        Output("media done");
                        channel1.RemoveAt(i);
                    }

                    
                }
            }
            else
            {
                if (consoleMode == 1)
                {
                    OutputBox.SelectionColor = Color.Orange;
                    OutputBox.AppendText("Channel 1");
                    OutputBox.SelectionColor = Color.Black;
                    OutputBox.AppendText(Environment.NewLine);
                    OutputBox.AppendText("No Tracks playing");

                }
            }

            if (channel2.Count != 0)
            {
                if (consoleMode == 1)
                {

                    OutputBox.SelectionColor = Color.Green;
                    OutputBox.AppendText("Channel 2");
                    OutputBox.AppendText(Environment.NewLine);
                }
                for (int i = 0; i < channel2.Count; i++)
                {
                    //Output(channel1[i].Source.ToString());
                    NAudio.Wave.WaveFileReader reader = new NAudio.Wave.WaveFileReader(channel2[i].Source.ToString().Replace("file:///", ""));
                    if (consoleMode == 1)
                    {
                        OutputBox.SelectionColor = Color.Black;
                        OutputBox.AppendText(Path.GetFileName(channel2[i].Source.LocalPath));
                        
                        OutputBox.AppendText(" " + ((int)channel2[i].Position.TotalSeconds).ToString() + "/" + ((int)reader.TotalTime.TotalSeconds).ToString());
                        OutputBox.AppendText(Environment.NewLine);
                    }
                    //Output(((int)channel2[i].Position.TotalSeconds).ToString() + "/" + ((int)reader.TotalTime.TotalSeconds).ToString());
                    if ((int)channel2[i].Position.TotalSeconds >= (int)reader.TotalTime.TotalSeconds)
                    {
                        Output("media done");
                        channel2.RemoveAt(i);
                    }


                }
            }
            else 
            {
                if (consoleMode == 1)
                {
                    OutputBox.AppendText(Environment.NewLine);
                    OutputBox.SelectionColor = Color.Green;
                    OutputBox.AppendText("Channel 2");
                    OutputBox.SelectionColor = Color.Black;
                    OutputBox.AppendText(Environment.NewLine);
                    OutputBox.AppendText("No Tracks playing");
                }

            }

            GC.Collect(); 
        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            if (currentChannel == 1)
            {
                currentChannel = 2;
                button17.Text = "Channel 2";
                button17.BackColor = Color.Green;
                button28.BackColor = Color.Green;
                if (channel2Playing)
                {
                    button28.Image = Properties.Resources.video_pause_button__1_;
                }
                else
                {
                    button28.Image = Properties.Resources.play_button__1_;
                }
            }
            else
            {
                currentChannel = 1;
                button17.Text = "Channel 1";
                button17.BackColor = Color.Orange;
                button28.BackColor = Color.Orange;
                if (channel1Playing)
                {
                    button28.Image = Properties.Resources.video_pause_button__1_;
                }
                else
                {
                    button28.Image = Properties.Resources.play_button__1_;
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int val = trackBar1.Value;
            //Output(val.ToString());
            channel2vol = (float) val / 10;
            channel1vol = 1 - channel2vol;

            float channel2v = channel2vol * channel2volsmol;
            float channel1v = channel1vol * channel1volsmol;

            //Output(channel1v.ToString());

            for (int i = 0; i < channel1.Count; i++)
            {
                channel1[i].Volume = channel1v;
            }
            for (int i = 0; i < channel2.Count; i++)
            {
                channel2[i].Volume = channel2v;
            }

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            channel1volsmol = (float) trackBar3.Value / 10;
            float channel1v = channel1vol * channel1volsmol;
            for (int i = 0; i < channel1.Count; i++)
            {
                channel1[i].Volume = channel1v;
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            channel2volsmol = (float)trackBar2.Value / 10;
            float channel2v = channel2vol * channel2volsmol;
            for (int i = 0; i < channel2.Count; i++)
            {
                channel2[i].Volume = channel2v;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            consoleMode = comboBox2.SelectedIndex; // 0 = standard, 1 = songs
            if (consoleMode == 0)
            {
                OutputBox.Text = consoledat;

            }
            else
            {
                OutputBox.Text = "";
                // refresh in timer 
            }
        }
    }
}
