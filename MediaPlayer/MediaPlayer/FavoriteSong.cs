using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer
{
    public class FavoriteSong : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string name { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string length { get; set; }
        public string path { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
