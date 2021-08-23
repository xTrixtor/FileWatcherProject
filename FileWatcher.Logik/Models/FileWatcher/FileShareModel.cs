using System;
using System.Collections.Generic;
using System.Text;

namespace FileWatcher.Logik.Models.FileWatcher
{
    public class FileShareModel
    {
        public Guid ID { get; set; }
        public string FileShareName { get; set; }
        public string FileSharePath { get; set; }
    }
}
