using System;
using System.Windows;
using NAudio.Wave;
using System.Diagnostics;
using WaveFileManipulator;
using System.IO;
using System.Media;

namespace AudioApplication
{
    public partial class MainWindow : Window
    {
        private const string StartTimeDisplay = "00:00:00";
        private Stopwatch Watch;
        private System.Timers.Timer AppTimer;
        private string FileName;
        private string OutputFileName;

        private AudioEdit Edit = new AudioEdit();
        private WaveIn Wave;
        private WaveFileWriter WaveWriter;
        public MainWindow()
        {
            InitializeComponent();
            FileName = Edit.CreateFileName();

            LabelForTimer.Content = StartTimeDisplay;

            Watch = new Stopwatch();
            AppTimer = new System.Timers.Timer(1000);

            AppTimer.Elapsed += TimerElapsed;
            if (OutputFileName == null)
            {
                StartButton.IsEnabled = false;
                AudioPlay.IsEnabled = false;
            }
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => LabelForTimer.Content = Watch.Elapsed.ToString(@"hh\:mm\:ss"));
        }

        private void WaveInDataAvailable(object sender, WaveInEventArgs e)
        {
            if (WaveWriter == null) return;

            WaveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            WaveWriter.Flush();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            StartButton.IsEnabled = false;
            Watch.Start();
            AppTimer.Start();

            Wave = new WaveIn();
            Wave.DeviceNumber = 0;
            Wave.WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(Wave.DeviceNumber).Channels);

            Wave.DataAvailable += new EventHandler<WaveInEventArgs>(WaveInDataAvailable);
            WaveWriter = new WaveFileWriter(OutputFileName, Wave.WaveFormat);
            Wave.StartRecording();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            if (Wave != null)
            {
                Wave.StopRecording();
                Wave.Dispose();
                Wave = null;
            }
            if (WaveWriter != null)
            {
                WaveWriter.Dispose();
                WaveWriter = null;
            }
            Watch.Stop();
            AppTimer.Stop();
            Watch.Reset();
            LabelForTimer.Content = StartTimeDisplay;

            var manipulator = new Manipulator(OutputFileName);
            var reversedByteArray = manipulator.Reverse();

            using (FileStream reversedFileStream = new FileStream($@"{ReadPath()}\{FileName}Reversed.wav", FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                reversedFileStream.Write(reversedByteArray, 0, reversedByteArray.Length);
            }
        }
        private string ReadPath()
        {
            return PathToCatalog.Text;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OutputFileName = $@"{ReadPath()}\{FileName}.wav";
            StartButton.IsEnabled = true;
            AudioPlay.IsEnabled = true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SoundPlayer audioPlayer = new SoundPlayer($@"{ReadPath()}\{FileName}Reversed.wav");
            audioPlayer.Play();
        }
    }
}
