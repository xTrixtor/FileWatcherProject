using FileWatcher.Logik.Models.FileWatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher.Logik.DataStore
{
    public class FileWatcherService
    {
        public FileWatcherService(string path, EventLoggerService eventLogger)
        {
            _fileWatcher.Path = path;
            _eventLogger = eventLogger;
        }

        public static FileSystemWatcher _fileWatcher = new FileSystemWatcher();
        public static EventLoggerService _eventLogger;

        public void RunFileWatcher()
        {
            RegisterEventMethods();
            RegisterChangeEvents();
        }
        public void RegisterEventMethods()
        {
            _fileWatcher.Changed += OnChangeFile;
            _fileWatcher.Created += OnCreateFile;
            _fileWatcher.Deleted += OnDeleteFile;
            _fileWatcher.Renamed += OnRenameFile;
            _fileWatcher.Error += OnError;
        }

        public void RegisterChangeEvents()
        {

            _fileWatcher.NotifyFilter = NotifyFilters.Attributes
                                        | NotifyFilters.CreationTime
                                        | NotifyFilters.LastAccess
                                        | NotifyFilters.LastWrite
                                        | NotifyFilters.Security
                                        | NotifyFilters.Size;

            _fileWatcher.Filter = "*.*";
            _fileWatcher.IncludeSubdirectories = true;
            _fileWatcher.EnableRaisingEvents = true;
        }
        private async static void OnChangeFile(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            Console.WriteLine($"Die Datei |{e.Name}| wurde geändert" );

            if (e.Name.Contains("."))
            {
                var eventMessage = String.Empty;
                eventMessage = $"Die Datei |{e.Name}| wurde geändert";
                await _eventLogger.CreateEventLogAsyc(
                    WatcherChangeTypes.Changed,
                    new FileSystemWatcherEvents { FileSystemEventObject = e },
                    eventMessage);
            }
            else
            { }
            
        }
        private async static void OnCreateFile(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType!= WatcherChangeTypes.Created)
                return;
            Console.WriteLine($"Die Datei: |{e.Name}| wurde auf folgenden Pfad hinterlegt: {e.FullPath}");

            var eventMessage = String.Empty;
            eventMessage = $"Die Datei: |{e.Name}| wurde auf folgenden Pfad hinterlegt: {e.FullPath}";
            await _eventLogger.CreateEventLogAsyc(
                WatcherChangeTypes.Created,
                new FileSystemWatcherEvents { FileSystemEventObject = e },
                eventMessage);
        }
        private async static void OnDeleteFile(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType!= WatcherChangeTypes.Deleted)
                return;
            Console.WriteLine($"Die Datei: {e.Name} wurde gelöscht");

            var eventMessage = String.Empty;
            eventMessage = $"Die Datei: {e.Name} wurde gelöscht";

            await _eventLogger.CreateEventLogAsyc(
                WatcherChangeTypes.Deleted,
                new FileSystemWatcherEvents { FileSystemEventObject = e },
                eventMessage);
        }
        private async static void OnRenameFile(object sender, RenamedEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Renamed)
                return;

            Console.WriteLine($"Der Name der Datei: |{e.OldName}| wurde in |{e.Name}| geändert");

            var eventMessage = String.Empty;
            eventMessage = $"Die Datei: {e.Name} wurde gelöscht";
            await _eventLogger.CreateEventLogAsyc(
                WatcherChangeTypes.Renamed,
                new FileSystemWatcherEvents { FileSystemEventObject = e },
                eventMessage);
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private async static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.TargetSite);
                PrintException(ex.InnerException);
            }

            await _eventLogger.CreateErrorMessage(ex);
        }
    }
}
