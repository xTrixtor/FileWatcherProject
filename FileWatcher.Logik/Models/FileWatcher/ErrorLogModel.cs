using System;
using System.Collections.Generic;
using System.Text;

namespace FileWatcher.Logik.Models.FileWatcher
{
    public class ErrorLogModel
    {
        public Guid ID { get; set; }
        public string TargetSite { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ProcessedDate { get; set; }
    }
}
