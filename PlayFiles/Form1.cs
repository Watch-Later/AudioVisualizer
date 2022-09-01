using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayFiles
{
    public partial class Form1 : Form
    {
        string song;
        WaveOut output;
        Mp3FileReader reader;
        string songsDirectory = "C:\\Users\\hakuchan\\Desktop\\Music for Visualizer";
        DirectoryInfo searchDirectory;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            filenameText.Text = "Enter file";
            filenameText.GotFocus += RemoveText;
            filenameText.LostFocus += AddText;
            startButton.Click += PlaySong;
            pauseButton.Click += PauseSong;
            searchDirectory = new DirectoryInfo(songsDirectory);


        }

        public void RemoveText(object sender, EventArgs e)
        {
            if (filenameText.Text == "Enter file")
            {
                filenameText.Text = "";
            }
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(filenameText.Text))
                filenameText.Text = "Enter text here...";
        }

        public void PlaySong(object sender, EventArgs e)
        {

            song = filenameText.Text;
            string songPath;
            FileInfo[] filesInDir = searchDirectory.GetFiles("*" + song + "*.*");
            if(filesInDir.Length > 0)
            {
                songPath = filesInDir[0].FullName;
                output = new WaveOut();
                reader = new Mp3FileReader(songPath);
                output.Init(reader);
                output.Play();
                pauseButton.Text = "Pause";
                fileLabel.Text = "Currently playing " + songPath;
            }
            else
            {
                fileLabel.Text = "This is not a valid song. Try Again.";
            }
            
        }
        public void PauseSong(object sender, EventArgs e)
        {
            if (output == null)
            {
                return;
            }
            if (output.PlaybackState == PlaybackState.Playing)
            {
                output.Pause();
                pauseButton.Text = "Resume";
            }
            else
            {
                output.Play();
                pauseButton.Text = "Start";
            }
        }

        private void fileLabel_Click(object sender, EventArgs e)
        {

        }

        private void pauseButton_Click(object sender, EventArgs e)
        {

        }

        private void filenameText_TextChanged(object sender, EventArgs e)
        {

        }

        private void fileLabel_Click_1(object sender, EventArgs e)
        {

        }

        private void pauseButton_Click_1(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void filenameText_TextChanged_1(object sender, EventArgs e)
        {

        }
    }

}
