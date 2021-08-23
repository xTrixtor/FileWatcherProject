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
        private readonly string RenameFileSql = "[FileWatcher].[RenameFile] @FileId, @FileName, @FilePath";


        public async Task CreateFileLogAsync(FileModel fileModel)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.ExecuteAsync(CreateFileSql, new FileModel {
                    HashString = fileModel.HashString,
                    FileName = fileModel.FileName,
                    FilePath = fileModel.FilePath.Replace(@"\",@"\\"),
                    FileType = fileModel.FileType
                });
            }
        }

        public async Task RenameFileLogAsync()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.ExecuteAsync(RenameFileSql);
            }
        }
    }
}
