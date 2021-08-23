using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileWatcher.Logik.Models.FileWatcher
{
    public class FileSystemWatcherEvents
    {
        public FileSystemEventArgs FileSystemEventObject { get; set; }
        public RenamedEventArgs RenameEventObject { get; set; }
    }
}
