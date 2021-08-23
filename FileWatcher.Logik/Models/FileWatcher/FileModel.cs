

using System;

namespace FileWatcher.Logik.Models
{
    public class FileModel
    {
        public Guid ID { get; set; }
        public string HashString { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
