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
using SQLite;
using System.Security.Policy;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLiteConnection connection = new SQLiteConnection(App.databasePath);

        ObservableCollection<Song> _songs = new ObservableCollection<Song>();
        private System.Windows.Media.MediaPlayer mediaPlayer;

        TimeSpan _position;
        DispatcherTimer _timer = new DispatcherTimer();

        private int curSongPos = 0;

        private bool isPlaying = false;
        private bool isShuffle = false;
        private bool isRepeated = false;

        public MainWindow()
        {
            InitializeComponent();
            myMusicButton.IsChecked = true;
            myMusicPanel.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            createTable();

            _songs = new ObservableCollection<Song>();
            songsListview.ItemsSource = _songs;
            loadSongs();

            mediaPlayer = new System.Windows.Media.MediaPlayer();
            createTimer();

            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        }

        private void createTimer()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(tickTock);
            _timer.Start();
        }

        private void createTable()
        {
            connection.CreateTable<Song>();
        }

        private void loadSongs()
        {
            var query = connection.Table<Song>();

            foreach (Song song in query)
            {
                _songs.Add(song);
            }
        }

        private void tickTock(object? sender, EventArgs e)
        {
            musicSlider.Value = mediaPlayer.Position.TotalSeconds;
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (isRepeated)
            {
                repeatSong();
            }

            else
            {
                changeToNextSong();
            }
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
                    connection.Insert(newSong);
                }
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            curSongPos = songsListview.SelectedIndex;
            string curPath = _songs[curSongPos].path;
            var uri = new System.Uri(curPath);

            if (e.ChangedButton == MouseButton.Left)
            {
                startPlaying(uri);
            } 
        }

        private void startPlaying(Uri uri)
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

            _position = TimeSpan.Parse(_songs[curSongPos].length);
            musicSlider.Minimum = 0;
            musicSlider.Maximum = _position.TotalSeconds;
        }

        private void changeToNextSong()
        {
            if (isShuffle)
            {
                Random random = new Random();

                int nextSongPos = 0;

                do
                {
                    nextSongPos = random.Next(0, _songs.Count);
                } while (nextSongPos == curSongPos || nextSongPos == (_songs.Count - 1));

                curSongPos = nextSongPos;
                songsListview.SelectedIndex = curSongPos;
                string curPath = _songs[curSongPos].path;
                var uri = new System.Uri(curPath);

                startPlaying(uri);
            }

            else
            {
                curSongPos += 1;

                if (curSongPos >= _songs.Count)
                {
                    curSongPos = 0;
                }

                songsListview.SelectedIndex = curSongPos;
                string curPath = _songs[curSongPos].path;
                var uri = new System.Uri(curPath);

                startPlaying(uri);
            }
        }

        private void changeToPreviousSong()
        {
            curSongPos -= 1;

            if (curSongPos < 0)
            {
                curSongPos = _songs.Count - 1;
            }

            songsListview.SelectedIndex = curSongPos;
            string curPath = _songs[curSongPos].path;
            var uri = new System.Uri(curPath);

            startPlaying(uri);
        }

        private void repeatSong()
        {
            string curPath = _songs[curSongPos].path;
            var uri = new System.Uri(curPath);
            startPlaying(uri);
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
            connection.Delete(_songs[i]);
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

        private void MySlider_DragCompleted(object sender, MouseButtonEventArgs e)
        {
            int pos = Convert.ToInt32(musicSlider.Value);
            mediaPlayer.Position = new TimeSpan(0, 0, 0, pos, 0);
        }

        private void playModeButton_Click(object sender, RoutedEventArgs e)
        {
            var modeImg = this.playModeButton.Template.FindName("modeImg", playModeButton) as Image;

            if (!isShuffle)
            {
                isShuffle = true;
                modeImg.Source = new BitmapImage(new Uri("Images/shuffle.png", UriKind.Relative));
            }

            else
            {
                isShuffle = false;
                modeImg.Source = new BitmapImage(new Uri("Images/next_song.png", UriKind.Relative));
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            changeToNextSong();
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            changeToPreviousSong();
        }

        private void repeatButton_Click(object sender, RoutedEventArgs e)
        {
            var repeatImg = this.repeatButton.Template.FindName("repeatImg", repeatButton) as Image;

            if (isRepeated)
            {
                isRepeated = false;
                repeatImg.Source = new BitmapImage(new Uri("Images/repeat.png", UriKind.Relative));
            }

            else
            {
                isRepeated = true;
                repeatImg.Source = new BitmapImage(new Uri("Images/repeat_1.png", UriKind.Relative));
            }
        }
    }
}
