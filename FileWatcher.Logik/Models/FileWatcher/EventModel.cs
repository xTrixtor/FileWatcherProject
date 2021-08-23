using System;
using System.Collections.Generic;
using System.Text;

namespace FileWatcher.Logik.Models.FileWatcher
{
    public class EventModel
    {
        public Guid ID { get; set; }
        public string EventType { get; set; }
        public string EventValue { get; set; }
    }
}
