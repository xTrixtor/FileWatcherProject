

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
        public string OldPath { get; set; }
        public string OldName { get; set; }
        public string OldFilePath { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsCreated { get; set; } = false;
        public bool ISRenamed { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public DateTime LastChangeDate { get; set; }

    }
}
