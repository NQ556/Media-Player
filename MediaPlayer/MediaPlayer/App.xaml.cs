using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static string databaseName = "Songs.db";
        static string curDir = System.Environment.CurrentDirectory;
        public static string databasePath = System.IO.Path.Combine(curDir, databaseName);
    }
}
