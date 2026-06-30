using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileUploadApp.Services
{
    public class FileUploadService
    {
        private const long MaxFileSizeBytes = 500L * 1024 * 1024; // 500 MB

        public void ValidateFile(string filePath)
        {
            var info = new FileInfo(filePath);
            if (!info.Exists)
                throw new FileNotFoundException("Selected file does not exist.", filePath);

            if (info.Length > MaxFileSizeBytes)
                throw new InvalidOperationException("File exceeds the 500 MB limit.");
        }

        public async Task UploadFileAsync(string sourcePath, IProgress<int> progress, CancellationToken ct)
        {
            ValidateFile(sourcePath);

            var uploadsDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FileUploadApp", "uploads");

            Directory.CreateDirectory(uploadsDir);

            var destPath = Path.Combine(uploadsDir, Path.GetFileName(sourcePath));

            // Simulated progress (no real backend transfer involved)
            for (int i = 0; i <= 100; i += 5)
            {
                ct.ThrowIfCancellationRequested();
                progress.Report(i);
                await Task.Delay(80, ct);
            }

            File.Copy(sourcePath, destPath, overwrite: true);
        }
    }
}