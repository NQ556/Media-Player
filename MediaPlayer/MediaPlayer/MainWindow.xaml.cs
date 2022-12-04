using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Numerics;
using System.Windows.Threading;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Song> _songs = new ObservableCollection<Song>();
        private System.Windows.Media.MediaPlayer mediaPlayer;

        TimeSpan _position;
        DispatcherTimer _timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            myMusicButton.IsChecked = true;
            myMusicPanel.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _songs = new ObservableCollection<Song>();
            songsListview.ItemsSource = _songs;

            mediaPlayer = new System.Windows.Media.MediaPlayer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(tickTock);
            _timer.Start(); 
        }

        private void tickTock(object? sender, EventArgs e)
        {
            musicSlider.Value = mediaPlayer.Position.TotalSeconds;
        }

        private void addMusicButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "MP3 (.mp3)|*.mp3|ALL Files (*.*)|*.*";

            string songName = "";
            string songArtist = "";
            string songAlbum = "";
            string songLength = "";

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    TagLib.File file = TagLib.File.Create(filePath);

                    if (file.Tag.Title == null)
                    {
                        songName = Path.GetFileNameWithoutExtension(filePath);
                    }

                    else
                    {
                        songName = file.Tag.Title;
                    }

                    if (file.Tag.FirstPerformer == null)
                    {
                        songArtist = "Unknown Artist";
                    }

                    else
                    {
                        songArtist = file.Tag.FirstPerformer;
                    }
                    
                    if (file.Tag.Album == null)
                    {
                        songAlbum = "Unknown Album";
                    }
                    
                    else
                    {
                        songAlbum = file.Tag.Album;
                    }

                    int time = (int)file.Properties.Duration.TotalSeconds;
                    TimeSpan t = TimeSpan.FromSeconds(time);
                    songLength = t.ToString();

                    Song newSong = new Song()
                    {
                        name = songName,
                        artist = songArtist,
                        album = songAlbum,
                        length = songLength,
                        path = filePath
                    };
                    _songs.Add(newSong);
                }
            }
        }

        private bool isPlaying = false;
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int i = songsListview.SelectedIndex;
            string curPath = _songs[i].path;
            var uri = new System.Uri(curPath);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (isPlaying)
                {
                    mediaPlayer.Stop();
                    isPlaying = false;
                }
                
                mediaPlayer.Open(uri);
                mediaPlayer.Play();
                isPlaying = true;

                var playPauseImg = this.playPauseButton.Template.FindName("playPauseImg", playPauseButton) as Image;
                playPauseImg.Source = new BitmapImage(new Uri("Images/pause.png", UriKind.Relative));

                _position = TimeSpan.Parse(_songs[i].length);
                musicSlider.Minimum = 0;
                musicSlider.Maximum = _position.TotalSeconds;
            } 
        }

        private void myMusicButton_Click(object sender, RoutedEventArgs e)
        {
            myMusicPanel.Visibility = Visibility.Visible;
        }

        private void likedNameButton_Click(object sender, RoutedEventArgs e)
        {
            myMusicPanel.Visibility = Visibility.Hidden;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int i = songsListview.SelectedIndex;
            _songs.RemoveAt(i);
        }

        private void musicSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int pos = Convert.ToInt32(musicSlider.Value);
            mediaPlayer.Position = new TimeSpan(0, 0, 0, pos, 0);
        }

        private void playPauseButton_Click(object sender, RoutedEventArgs e)
        {
            var playPauseImg = this.playPauseButton.Template.FindName("playPauseImg", playPauseButton) as Image;

            if (isPlaying)
            {
                mediaPlayer.Pause();
                playPauseImg.Source = new BitmapImage(new Uri("Images/play.png", UriKind.Relative));
                isPlaying = false;
            }

            else
            {
                mediaPlayer.Play();
                playPauseImg.Source = new BitmapImage(new Uri("Images/pause.png", UriKind.Relative));
                isPlaying = true;
            }
        }
    }
}
