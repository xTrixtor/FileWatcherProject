using FileWatcher.Logik.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileWatcher.Logik.DataStore
{
    class FileShareSyncService
    {
        private readonly ReshService _reshService;

        public FileShareSyncService(ReshService reshService)
        {
            this._reshService = reshService;
        }
        
        public async Task<IEnumerable<FileModel>> GetAllFilesFromFileShareAsync(string fileSharePath)
        {
            return await Task.Run(() => GetAllFilesFromFileShare(fileSharePath));
        }

        private async Task<IEnumerable<FileModel>>GetAllFilesFromFileShare(string fileSharePath)
        {
            var myFileList = new List<FileModel>();

            var directories = Directory.GetDirectories(fileSharePath, "*.*", SearchOption.AllDirectories);

            foreach (var directory in directories)
            {
                var filePaths = Directory.GetFiles(directory);
                var currentDirectory = directory;
                foreach (var filePath in filePaths)
                {
                    var hastString = await _reshService.RunReshAsync(filePath);
                    var fileType = await GetTypeOfFileAsync(filePath);

                    var fileDirectoryParts = filePath.Split("\\");
                    var fileName = fileDirectoryParts[fileDirectoryParts.Length - 1];
                    myFileList.Add(new FileModel
                    {
                        FileName = fileName,
                        FilePath = Path.Combine(currentDirectory, fileName),
                        HashString = hastString,
                        FileType = fileType
                    }); 
                }
            }

            return myFileList;
        }

        public async Task<string> GetTypeOfFileAsync(string filePath)
        {
            return await Task.Run(() => Path.GetExtension(filePath));
        }
    }
}
