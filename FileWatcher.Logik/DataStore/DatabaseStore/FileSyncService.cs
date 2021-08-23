using Dapper;
using FileWatcher.Logik.Models.FileWatcher;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher.Logik.DataStore
{
    class FileSyncService
    {
        private readonly string _connectionString;

        public FileSyncService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        private readonly string GetAllFileSharePahtsSql = "Select fs.FileSharePath From [FileWatcher].[FileShare] as fs";
        private readonly string CreateFileSharePathSql = "[FileWatcher].[CreateFileSharePath] @FileShareName, @FileShare";
        private readonly string UpdateFileSharePathSql = "[FileWatcher].[UpdateFileSharePath] @FileShareId, @FileShareName, @FileSharePath";

        public async Task<IEnumerable<string>> GetAllFileSharePaths()
        {
            using (var con = new SqlConnection(_connectionString))
                return await con.QueryAsync<string>(GetAllFileSharePahtsSql);
        }

        public async Task<IEnumerable<string>> CreateFileSharePath(FileShareModel fileShare)
        {
            using (var con = new SqlConnection())
                return await con.QueryAsync<string>(CreateFileSharePathSql, new FileShareModel 
                {FileShareName = fileShare.FileShareName, FileSharePath = fileShare.FileSharePath });
        }

        public async Task<IEnumerable<string>> UpdateFileSharePath(FileShareModel fileShare)
        {
            using (var con = new SqlConnection(_connectionString))
                return await con.QueryAsync<string>(UpdateFileSharePathSql, new FileShareModel
                { ID = fileShare.ID, FileShareName = fileShare.FileShareName, FileSharePath = fileShare.FileSharePath });
        }

    }
}
