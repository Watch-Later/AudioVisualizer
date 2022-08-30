using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        public Form1()
        {
            InitializeComponent();


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            urlText.Text = "Enter url";
            urlText.GotFocus += RemoveText;
            urlText.LostFocus += AddText;
            startButton.Click += PlaySong;
            pauseButton.Click += PauseSong;

        }

        public void RemoveText(object sender, EventArgs e)
        {
            if (urlText.Text == "Enter url")
            {
                urlText.Text = "";
            }
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(urlText.Text))
                urlText.Text = "Enter text here...";
        }

        public void PlaySong(object sender, EventArgs e)
        {
            if (song == urlText.Text)
            {
                return;
            }
            song = urlText.Text;
            output = new WaveOut();
            reader = new Mp3FileReader(song);
            output.Init(reader);
            output.Play();
            pauseButton.Text = "Pause";
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
    }

}
