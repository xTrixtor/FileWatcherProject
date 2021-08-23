using Dapper;
using FileWatcher.Logik.DataStore.FileStore;
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
        private FileService _fileService;

        public EventLoggerService(string connectionString, FileService fileService)
        {
            this._connectionString = connectionString;
            this._fileService = fileService;
        }

        private readonly string GetFileIdSql = "Select * From [FileWatcher].[File] Where FileName = @FileName And FilePath = @FilePath";

        private readonly string CreateCreateEventLogSql = "Exec [CreateCreateEventLog] @FileId, @EventType, @EventValue";
        private readonly string CreateDeleteEventLogSql = "Exec [FileWatcher].[CreateDeleteEventLog] @FileId, @EventType, @EventValue";
        private readonly string CreateRenameEventLogSql = "Exec [FileWatcher].[CreateRenameEventLog] @FileId, @EventType, @EventValue";
        private readonly string CreateChangedEventLogSql = "Exec [FileWatcher].[CreateChangedEventLog] @FileId, @EventType, @EventValue";

        private readonly string CreateErrorLogSql = "[FileWatcher].[CreateErrorLog] @ErrorMessage, @InnerExceptionMessage, @TargetSite";

        public async Task CreateEventLogAsyc(WatcherChangeTypes eventTypeEnumn, FileSystemWatcherEvents fileWatcherEvents, string eventMessage )
        {
            var sqlStatement = String.Empty;
            sqlStatement = GetEventSqlStatment(eventTypeEnumn);
            var fileModel = await GetFileAsync(fileWatcherEvents);

            var eventString =  await _fileService.GetEventTypeStringValueAsync(eventTypeEnumn);

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

        public async Task<FileModel> GetFileAsync(FileSystemWatcherEvents fileSystemWatcherEvents)
        {
            if (fileSystemWatcherEvents.FileSystemEventObject != null)
            {
                var filePath = await _fileService.GetFilePathOfEventPathAsync(fileSystemWatcherEvents);
                var fileName = await _fileService.GetFileNameOfEventAsync(fileSystemWatcherEvents);

                using (var con = new SqlConnection(_connectionString))
                    return await con.QueryFirstOrDefaultAsync<FileModel>(GetFileIdSql, new FileModel { FileName = fileName, FilePath = fileSystemWatcherEvents.FileSystemEventObject.FullPath });
            }
            else
            {
                var filePath = await _fileService.GetFilePathOfEventPathAsync(fileSystemWatcherEvents);
                var fileName = await _fileService.GetFileNameOfEventAsync(fileSystemWatcherEvents);

                using (var con = new SqlConnection(_connectionString))
                    return await con.QueryFirstOrDefaultAsync<FileModel>(GetFileIdSql, new { FileName = fileName, FilePath = filePath });
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
    }
}
