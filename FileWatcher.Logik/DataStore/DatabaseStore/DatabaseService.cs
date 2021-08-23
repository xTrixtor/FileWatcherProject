using Dapper;
using FileWatcher.Logik.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace FileWatcher.Logik.DataStore
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }
        private readonly string CreateFileSql = "[FileWatcher].[CreateFile] @HashString, @FileName, @FilePath, @FileType";
        private readonly string RenameFileSql = "[FileWatcher].[RenameFile] @HashString, @FileName, @FilePath";
        private readonly string DeleteFileSql = "[FileWatcher].[DeleteFile] @HashString";


        public async Task CreateFileAsync(FileModel fileModel)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.ExecuteAsync(CreateFileSql, new FileModel {
                    HashString = fileModel.HashString,
                    FileName = fileModel.FileName,
                    FilePath = fileModel.FilePath,
                    FileType = fileModel.FileType
                });
            }
        }

        public async Task RenameFileAsync(FileModel fileModel)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.ExecuteAsync(RenameFileSql, new {
                    HashString = fileModel.HashString,
                    FileName = fileModel.FileName, 
                    FilePath = fileModel.FilePath 
                });
            }
        }

        public async Task DelteFile(FileModel fileModel)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.ExecuteAsync(DeleteFileSql, new
                {
                    HashString = fileModel.HashString
                });
            }
        }
    }
}
