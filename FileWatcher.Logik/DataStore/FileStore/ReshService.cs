using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;

namespace FileWatcher.Logik.DataStore
{
    public class ReshService
    {
        private SHA256 _sha256 = SHA256.Create();
        public async Task<string> RunReshAsync(string filePath)
        {
            var fileBytes = await ReadFileAsync(filePath);
            var hashBytes = await GetHashBytesAsync(fileBytes);
            var hashString =  await GetHashStringAsync(hashBytes);
            return hashString;
        }
        public async Task<byte[]> ReadFileAsync(string filePath)
        {
            return await File.ReadAllBytesAsync(filePath);
        }
        public async Task<byte[]> GetHashBytesAsync(byte[] fileBytes)
        {
            return await Task.Run(() => _sha256.ComputeHash(fileBytes));
        }
        public async Task<string> GetHashStringAsync(byte[] hashBytes)
        {
            return await Task.Run(() => Encoding.Default.GetString(hashBytes));
        }
    }        
}
