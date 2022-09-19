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
    public partial class MainPanel : Form
    {
        string song;
        WaveOut output;

        WaveFileReader reader;
        WaveFileReader readerVisualizer;
        //Mp3FileReader reader;
        //Mp3FileReader readerVisualizer;

        // songsDirectory is the directory all music files are expected to be in
        //string songsDirectory = Properties.Settings.Default.songsDirectory;
        string songsDirectory = "C:\\Users\\hakuchan\\Desktop\\Music for Visualizer";
        DirectoryInfo searchDirectory;

        // You need double values for graphing on the plot
        readonly double[] AudioValues;
        readonly double[] FFTValues;

        // mp3FileReader uses a byte buffer
        byte[] buffer;

        // define some constants to init the mp3fileReader with
        int SampleRate = 44100;
        // Size of values can range from that of a signed 16 bit integer, which is 2 bytes
        int ByteDepth = 2;
        // this is in ms, so in reality 20E-3 s
        int BufferMilliseconds = 20;

        int bytesRead;

        double SmoothingValue = 0.1;
        public MainPanel()
        {
            InitializeComponent();
            AudioValues = new double[SampleRate * BufferMilliseconds / 1000];
            double[] paddedAudio = FftSharp.Pad.ZeroPad(AudioValues);
            double[] fftTransform = FftSharp.Transform.FFTmagnitude(paddedAudio);
            FFTValues = new double[fftTransform.Length];
            double fftPeriod = FftSharp.Transform.FFTfreqPeriod(SampleRate, fftTransform.Length);

            buffer = new byte[2 * (SampleRate * BufferMilliseconds / 1000)];

            formsPlot1.Plot.AddSignal(FFTValues, fftPeriod);
            formsPlot1.Plot.YLabel("Level");
            formsPlot1.Plot.XLabel("Frequency (Hz)");
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
            smoothingControl.Scroll += smoothing_Scroll;
            smoothingLabel.KeyUp += changeSmoothing;
            output = new WaveOut();

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

            if (song == filenameText.Text)
            {
                output.Resume();
                return;
            }
            song = filenameText.Text;
            string songPath;
            FileInfo[] filesInDir = searchDirectory.GetFiles("*" + song + "*.wav");
            if (filesInDir.Length > 0)
            {
                songPath = filesInDir[0].FullName;
                //reader = new Mp3FileReader(songPath);
                //readerVisualizer = new Mp3FileReader(songPath);
                reader = new WaveFileReader(songPath);
                readerVisualizer = new WaveFileReader(songPath);

                output.Init(reader);
                output.Play();
                timer.Tick += TimerTick;
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
            if (output.PlaybackState == PlaybackState.Playing)
            {
                output.Pause();
                pauseButton.Text = "Resume";
            }
            else if (output.PlaybackState == PlaybackState.Paused)
            {
                output.Play();
                pauseButton.Text = "Pause";
            }

        }

        // request a redraw when the timer ticks as well as resize the axis. 
        // also convert the data from the buffer to a double 
        public void TimerTick(object sender, EventArgs e)
        {

            if (output != null && output.PlaybackState == PlaybackState.Playing)
            {
                bytesRead = readerVisualizer.Read(buffer, 0, buffer.Length);
                for (int i = 0; i < buffer.Length / 2; i++)
                {
                    // single pole low pass filter (smoothing instantaneous values)
                    AudioValues[i] = AudioValues[i] * SmoothingValue + BitConverter.ToInt16(buffer, i * 2) * (1 - SmoothingValue);
                }
            }


            double[] paddedAudio = FftSharp.Pad.ZeroPad(AudioValues);
            var window = new FftSharp.Windows.Hanning();
            window.ApplyInPlace(paddedAudio);
            double[] fftTransform = FftSharp.Transform.FFTpower(paddedAudio);

            double max = fftTransform.Max();
            var currentLimits = formsPlot1.Plot.GetAxisLimits();
            formsPlot1.Plot.SetAxisLimits(
                xMin: 0,
                xMax: 6,
                yMin: 0,
                yMax: Math.Max(currentLimits.YMax, max));

            Array.Copy(fftTransform, FFTValues, fftTransform.Length);
            formsPlot1.RefreshRequest();
        }

        private void smoothing_Scroll(object sender, EventArgs e)
        {
            double val = (double)(smoothingControl.Value) / 20;
            smoothingLabel.Text = $"{val:0.##}";
            SmoothingValue = val;
        }

        private void changeSmoothing(object sender, KeyEventArgs e)
        {

            double val;
            if (e.KeyCode == Keys.Enter)
            {
                if (Double.TryParse(smoothingLabel.Text, out val))
                {
                    SmoothingValue = val;
                }
                else
                {
                    smoothingLabel.Text = "This is not a valid value. Try again.";
                }
            }
        }
    }

}
