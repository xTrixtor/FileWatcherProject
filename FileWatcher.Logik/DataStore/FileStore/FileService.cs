using FileWatcher.Logik.Models;
using FileWatcher.Logik.Models.FileWatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher.Logik.DataStore.FileStore
{
    public class FileService
    {
        public async Task<string> GetFilePathOfEventPathAsync(FileSystemWatcherEvents events)
        {
            return await Task.Run(() => GetFilePathOfEventPath(events));
        }

        private string GetFilePathOfEventPath(FileSystemWatcherEvents events)
        {
            if (events.FileSystemEventObject != null)
            {
                var filePathParts = events.FileSystemEventObject.FullPath.Split("\\");
                var filePath = string.Empty;
                int count = 0;
                foreach (var part in filePathParts)
                {
                    if (count != filePathParts.Length -1)
                        filePath += part + @"\";
                    else
                        filePath += part;
                    count++;
                }
                return filePath;
            }
            else
            {
                var filePathParts = events.RenameEventObject.FullPath.Split("\\");
                var filePath = string.Empty;
                int count = 0;
                foreach (var part in filePathParts)
                {
                    if (count != filePathParts.Length)
                        filePath += part + @"\";
                    else
                        filePath += part;
                    count++;
                }
                return filePath;
            }
        }

        public async Task<string> GetTypeOfFileAsync(string filePath)
        {
            return await Task.Run(() => Path.GetExtension(filePath));
        }

        public async Task<string> GetFileNameOfEventAsync(FileSystemWatcherEvents fileWatcherEvents)
        {
            return await Task.Run(() => GetFileNameOfEvent(fileWatcherEvents));
        }

        private string GetFileNameOfEvent(FileSystemWatcherEvents fileWatcherEvents)
        {
            if (fileWatcherEvents.FileSystemEventObject != null)
            {
                var fileName = String.Empty;
                var fileNameParts = fileWatcherEvents.FileSystemEventObject.Name.Split('\\');
                return fileNameParts[fileNameParts.Length - 1];
            }
            else
            {
                var fileName = String.Empty;
                var fileNameParts = fileWatcherEvents.RenameEventObject.Name.Split('\\');
                return fileNameParts[fileNameParts.Length - 1];
            }
        }

        public async Task<string> GetEventTypeStringValueAsync(WatcherChangeTypes eventTypeEnumn)
        {
            return await Task.Run(() => GetEventTypeStringValue(eventTypeEnumn));
        }

        private string GetEventTypeStringValue(WatcherChangeTypes eventTypeEnumn)
        {
            switch (eventTypeEnumn)
            {
                case WatcherChangeTypes.Created:
                    return "Created";
                case WatcherChangeTypes.Deleted:
                    return "Deleted";
                case WatcherChangeTypes.Changed:
                    return "Changed";
                case WatcherChangeTypes.Renamed:
                    return "Renamed";
                default:
                    throw new Exception("Es wurde kein Event übergeben!");
            }
        }
    }
}
