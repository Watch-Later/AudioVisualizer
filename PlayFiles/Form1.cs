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

        // songsDirectory is the directory all music files are expected to be in
        string songsDirectory = "C:\\Users\\hakuchan\\Desktop\\Music for Visualizer";
        DirectoryInfo searchDirectory;

        // You need double values for graphing on the plot
        readonly double[] AudioValues;

        // mp3FileReader uses a byte buffer
        byte[] buffer;

        // define some constants to init the mp3fileReader with
        int SampleRate = 44100;
        // Size of values can range from that of a signed 16 bit integer, which is 2 bytes
        int ByteDepth = 2;
        // this is in ms, so in reality 20E-3 s
        int BufferMilliseconds = 20;
        public Form1()
        {
            InitializeComponent();
            AudioValues = new double[SampleRate * BufferMilliseconds / 1000];

            // 
            buffer = new byte[(SampleRate * BufferMilliseconds / 1000)];

            formsPlot1.Plot.AddSignal(AudioValues, SampleRate / 1000);
            formsPlot1.Plot.YLabel("Level");
            formsPlot1.Plot.XLabel("Time (milliseconds)");
            formsPlot1.Refresh();
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
            int bytesRead;
            song = filenameText.Text;
            string songPath;
            FileInfo[] filesInDir = searchDirectory.GetFiles("*" + song + "*.*");
            if (filesInDir.Length > 0)
            {
                songPath = filesInDir[0].FullName;
                output = new WaveOut();
                reader = new Mp3FileReader(songPath);


                output.Init(reader);
                output.Play();
                pauseButton.Text = "Pause";
                fileLabel.Text = "Currently playing " + songPath;
                //do
                //{
                //    bytesRead = reader.Read(buffer, 0, buffer.Length);
                //    for (int i = 0; i < buffer.Length / 2; i++)
                //        AudioValues[i] = BitConverter.ToInt16(buffer, i);
                //} while (bytesRead > 0);

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

        // request a redraw when the timer ticks as well as resize the axis. 
        // also convert the data from the buffer to a double 
        public void TimerTick(object sender, EventArgs e)
        {
            for (int i = 0; i < buffer.Length / 2; i++)
                AudioValues[i] = BitConverter.ToInt16(buffer, i * 2);
            int level = (int)AudioValues.Max();
            var currentLimits = formsPlot1.Plot.GetAxisLimits();
            formsPlot1.Plot.SetAxisLimits(
                yMin: Math.Min(currentLimits.YMin, -level),
                yMax: Math.Max(currentLimits.YMax, level));
            formsPlot1.RefreshRequest();
        }
    }

}
