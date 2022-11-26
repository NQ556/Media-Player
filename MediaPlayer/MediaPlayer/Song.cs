using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer
{
    public class Song : INotifyPropertyChanged
    {
        string name { get; set; }
        string artist { get; set; }
        string album { get; set; }
        string time { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
