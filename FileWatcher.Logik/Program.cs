using FileWatcher.Logik.DataStore;
using FileWatcher.Logik.DataStore.FileStore;
using FileWatcher.Logik.Models;
using FileWatcher.Logik.Models.FileWatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileWatcher.Logik
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }


        private static async Task MainAsync()
        {
            var connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=FileWatcher;Integrated Security=True";

            //Services
            ReshService reshService = new ReshService();
            FileService fileService = new FileService();
            EventLoggerService eventLogger = new EventLoggerService(connectionString,fileService);
            FileSyncService fileSync = new FileSyncService(connectionString);
            FileShareSyncService fileShareSyncService = new FileShareSyncService(reshService);
            DatabaseService databaseService = new DatabaseService(connectionString);

            //FileService
            var fileSharePaths = Enumerable.Empty<string>();
            fileSharePaths = await fileSync.GetAllFileSharePaths();
            var combineListOfFiles = new List<IEnumerable<FileModel>>();

            foreach (var sharePath in fileSharePaths)
                combineListOfFiles.Add(await fileShareSyncService.GetAllFilesFromFileShareAsync(sharePath));


            foreach (var fileModelList in combineListOfFiles)
            {
                foreach (var fileModel in fileModelList)
                    await databaseService.CreateFileAsync(fileModel);
            }

            //FileWatcher 
            var filePath = @"C:\Users\Nico\Desktop\TestFileShare\SuperWichtigesProjekt\testHashFile.txt";
            var fileSharePath = @"C:\Users\Nico\Desktop\TestFileShare";

            FileWatcherService fileWatcher = new FileWatcherService(fileSharePath, eventLogger);

            //Test HashString
            Console.WriteLine(await reshService.RunReshAsync(filePath));

            //Start of FileWatcher
            fileWatcher.RunFileWatcher();

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        public static async Task<IEnumerable<FileModel>> GetAllFilesFromFileShareAsync(string fileSharePath)
        {
            return await Task.Run(() =>  GetAllFilesFromFileShare(fileSharePath));
        }

        public static IEnumerable<FileModel> GetAllFilesFromFileShare(string fileSharePath)
        {
            var myFileList = new List<FileModel>();

            var directories = Directory.GetDirectories(fileSharePath, "*.*", SearchOption.AllDirectories);

            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory);
                var currentDirectory = directory;
                foreach (var file in files)
                {
                    var fileDirectoryParts = file.Split("\\");
                    var fileName = fileDirectoryParts[fileDirectoryParts.Length - 1];
                    myFileList.Add(new FileModel { FileName = fileName, FilePath = $"{currentDirectory}" });
                }
            }

            return myFileList;
        }
    }
}
