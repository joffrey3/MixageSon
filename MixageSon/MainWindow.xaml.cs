using Microsoft.Win32;
using MixageSon.Rendering;
using MixageSon.Sound;
using NAudio.Extras;
using NAudio.Wave;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Threading;
using Equalizer = MixageSon.Sound.Equalizer;

namespace MixageSon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Visualizer _visualizer;
        private Equalizer? _equalizer;

        private WaveOutEvent? _outputDevice;
        private AudioFileReader? _audioFile;

        private DispatcherTimer? _playbackTimer;

        private bool _playing = false;

        public MainWindow()
        {
            InitializeComponent();
            _visualizer = new(VisualizerCanvas);
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new()
            {
                Multiselect = false,
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Fichiers son (*.wav,*.mp3,*.ogg)|*.wav;*.mp3;*.ogg"
            };

            bool? accepted = fileDialog.ShowDialog();
            if (accepted == null || accepted == false) { return; }

            string path = fileDialog.FileName;

            Hyperlink link = new();
            link.Inlines.Add(path);

            link.RequestNavigate += (object? sender, RequestNavigateEventArgs e) =>
            {
                Process.Start("explorer.exe", Path.GetDirectoryName(path)!);
            };

            PathLabel.Content = link;

            LoadFromFile(path);
        }

        private void VolumeSlider_ValueChanged<T>(object sender, RoutedPropertyChangedEventArgs<T> e)
        {
            VolumeLabel.Content = $"Volume: {Math.Round(VolumeSlider.Value, 0)}%";

            if (_outputDevice == null) { return; }
            _outputDevice.Volume = (float)(VolumeSlider.Value / 100.0);
        }

        private void LoadFromFile(string path)
        {
            _outputDevice?.Stop();
            _outputDevice?.Dispose();

            _outputDevice = new()
            {
                Volume = (float)VolumeSlider.Value / 100,
                DesiredLatency = 100
            };

            _audioFile = new(path);
            _equalizer = new(_audioFile);

            CustomAggregator aggregator = new(_equalizer, 2048);
            FFTCanvas _ = new(aggregator, FFTCanvas, 2048);

            _outputDevice.Init(aggregator);
            _outputDevice.Play();

            _playbackTimer?.Stop();
            _playbackTimer = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };


            _playbackTimer.Tick += (object? sender, EventArgs e) =>
            {
                UpdatePlaybackLabel();
                UpdatePlaybackSlider();
            };

            UpdatePlaybackLabel();
            _playbackTimer.Start();

            Play();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_playing) { Pause(); } else { Play(); }
        }

        private void Play()
        {
            _playing = true;
            PlayButton.Content = "Pause";
            _outputDevice?.Play();
        }

        private void Pause()
        {
            _playing = false;
            PlayButton.Content = "Play";
            _outputDevice?.Pause();
        }

        private void UpdatePlaybackLabel()
        {
            if (_audioFile == null) return;

            PlaybackLabel.Content = $"{_audioFile.CurrentTime:mm\\:ss} / {_audioFile.TotalTime:mm\\:ss}";
        }

        private void UpdatePlaybackSlider()
        {
            if (_audioFile == null) { return; }

            PlaybackSlider.Value = _audioFile.CurrentTime / _audioFile.TotalTime;
        }

        private void BassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Equalizer.Bass = (float)(BassSlider.Value);
        }

        private void TrebbleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Equalizer.Trebble = (float)(TrebbleSlider.Value);
        }

        private void MidrangeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Equalizer.Midrange = (float)(MidrangeSlider.Value);
        }
    }
}