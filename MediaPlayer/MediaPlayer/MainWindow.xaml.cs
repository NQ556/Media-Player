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

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Song> _songs = new ObservableCollection<Song>();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _songs = new ObservableCollection<Song>();
            songsListview.ItemsSource = _songs;
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
    }
}
