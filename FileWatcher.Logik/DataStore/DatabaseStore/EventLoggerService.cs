using Dapper;
using FileWatcher.Logik.Models;
using FileWatcher.Logik.Models.FileWatcher;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher.Logik.DataStore
{
    public class EventLoggerService
    {
        private readonly string _connectionString;

        public EventLoggerService(string connectionString)
        {
            this._connectionString = connectionString;
        }


        private readonly string GetFileIdSql = "Select * From [FileWatcher].[File] Where FileName = @FileName And FilePath = @FilePath";

        private readonly string CreateCreateEventLogSql = "Exec [CreateCreateEventLog] @FileId, @EventType, @EventValue";
        private readonly string CreateDeleteEventLogSql = "Exec [FileWatcher].[CreateDeleteEventLog] @FileId, @EventType, @EventValue";
        private readonly string CreateRenameEventLogSql = "Exec [FileWatcher].[CreateRenameEventLog] @FileId, @EventType, @EventValue";
        private readonly string CreateChangedEventLogSql = "Exec [FileWatcher].[CreateChangedEventLog] @FileId, @EventType, @EventValue";

        private readonly string CreateErrorLogSql = "[FileWatcher].[CreateErrorLog] @ErrorMessage, @InnerExceptionMessage, @TargetSite";


        public async Task<FileModel> GetFileIdAsync(FileSystemWatcherEvents fileSystemWatcherEvents)
        {
            if (fileSystemWatcherEvents.FileSystemEventObject != null)
            {
                var fileModel = GetParameterReadyForSqlStatement(fileSystemWatcherEvents);
                
                using(var con = new SqlConnection(_connectionString))
                    return await con.QueryFirstOrDefaultAsync<FileModel>(GetFileIdSql, new FileModel{ FileName = fileModel.FileName, FilePath = fileModel.FilePath });
            }
            else 
            {
                var fileModel = GetParameterReadyForSqlStatement(fileSystemWatcherEvents);
                using (var con = new SqlConnection(_connectionString))
                    return await con.QueryFirstOrDefaultAsync<FileModel>(GetFileIdSql, new { FileName = fileModel.FileName, FilePath = fileModel.FilePath });
            }
            
        }

        public async Task CreateEventLogAsyc(WatcherChangeTypes eventTypeEnumn, FileSystemWatcherEvents fileWatcherEvents, string eventMessage )
        {
            var sqlStatement = String.Empty;
            sqlStatement = GetEventSqlStatment(eventTypeEnumn);

            var fileModel = await GetFileIdAsync(fileWatcherEvents);
            var eventString = await GetEventTypeStringValueAsync(eventTypeEnumn);

            using (var con = new SqlConnection(_connectionString))
            {
                 await con.ExecuteAsync(sqlStatement, new { FileId = fileModel.ID, EventType = eventString, EventValue = eventMessage });
            }
        }
       
        public async Task CreateErrorMessage(Exception ex)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.ExecuteAsync(CreateErrorLogSql, new ErrorLogModel { ErrorMessage = ex.Message, InnerExceptionMessage = ex.InnerException.Message, TargetSite = ex.TargetSite.Name  });
            }
        }

        public string GetEventSqlStatment(WatcherChangeTypes eventTypeEnumn)
        {
            switch (eventTypeEnumn)
            {
                case WatcherChangeTypes.Created:
                    return CreateCreateEventLogSql;
                case WatcherChangeTypes.Deleted:
                    return CreateDeleteEventLogSql;
                case WatcherChangeTypes.Changed:
                    return CreateChangedEventLogSql;
                case WatcherChangeTypes.Renamed:
                    return CreateRenameEventLogSql;
                default:
                    throw new Exception("Es wurde kein Event übergeben!");
            }
        }

        public FileModel GetParameterReadyForSqlStatement(FileSystemWatcherEvents fileWatcherEvents)
        {
            if (fileWatcherEvents.FileSystemEventObject != null)
            {
                var fileName = String.Empty;
                var fileNameParts = fileWatcherEvents.FileSystemEventObject.Name.Split('\\');
                fileName = fileNameParts[fileNameParts.Length - 1];
                var filePath = fileWatcherEvents.FileSystemEventObject.FullPath;
                return new FileModel { FileName = fileName, FilePath = filePath };
            }
            else
            {
                var fileName = String.Empty;
                var fileNameParts = fileWatcherEvents.RenameEventObject.Name.Split('\\');
                fileName = fileNameParts[fileNameParts.Length - 1];

                var filePath = fileWatcherEvents.RenameEventObject.FullPath;
                filePath = Path.Combine(filePath, "");

                return new FileModel { FileName = fileName, FilePath = filePath };
            }
        }

        private async Task<string> GetEventTypeStringValueAsync(WatcherChangeTypes eventTypeEnumn)
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
