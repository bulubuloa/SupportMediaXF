using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportMediaXF.iOS.SupportMediaExtended
{
    public static class FileHelper
    {
        /// <summary>
        /// Copy file with progress
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destFilePath"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task CopyFileAsync(string sourceFilePath, string destFilePath, IProgress<float> progress, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buffer = new byte[1024 * 1024]; // 1MB buffer
            using (var source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var fileLength = source.Length * 1.0f;
                using (var dest = new FileStream(destFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                {
                    var totalBytes = 0f;
                    var currentBlockSize = 0;

                    while ((currentBlockSize = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        totalBytes += currentBlockSize;
                        var persentage = totalBytes / fileLength;
                        await dest.WriteAsync(buffer, 0, currentBlockSize, cancellationToken).ConfigureAwait(false);
                        progress?.Report(persentage);
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
            }
        }

        /// <summary>
        /// Copy file
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destFilePath"></param>
        public static void CopyFile(string sourceFilePath, string destFilePath)
        {
            try
            {
                var buffer = new byte[2 * 1024 * 1024]; // 2 MB
                using (var source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var dest = new FileStream(destFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                    {
                        var currentBlockSize = 0;
                        while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            dest.Write(buffer, 0, currentBlockSize);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                File.Copy(sourceFilePath, destFilePath, true);
            }
        }

        /// <summary>
		/// Gets the file type format.
		/// </summary>
		/// <returns>The file type format.</returns>
		/// <param name="fileName">File name.</param>
		public static string GetFileTypeFormat(string fileName)
        {
            try
            {
                var fileType = fileName.Replace(Path.GetFileNameWithoutExtension(fileName), "").Replace(".", "");

                if (fileType.ToLower().Contains("pdf"))
                    return "pdf";
                if (fileType.ToLower().Contains("doc"))
                    return "word";
                if (fileType.ToLower().Contains("xls"))
                    return "excel";
                if (IsImageFile(fileName))
                    return "image";

                return string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Check: file is image?
        /// </summary>
        /// <param name="filePath">C:\abc.jpg</param>
        /// <returns>.pdf</returns>
        public static bool IsImageFile(string filePath)
        {
            try
            {
                var ext = GetExtension(filePath);
                if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == "raw")
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Get extension of file
        /// </summary>
        /// <param name="filePath">C:\abc.pdf</param>
        /// <returns>.pdf</returns>
        public static string GetExtension(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return string.Empty;

                var result = Path.GetExtension(filePath).ToLower();
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Get file name without extension
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>abc.pdf</returns>
        public static string GetFileNameWithoutExtension(string filePath)
        {
            try
            {
                var result = Path.GetFileNameWithoutExtension(filePath);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Get file name with extenstion
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>abc.pdf</returns>
        public static string GetFileName(string filePath)
        {
            try
            {
                return string.IsNullOrEmpty(filePath) ? null : Path.GetFileName(filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Create a file with file path
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="isOverride">delete the file if it's existing</param>
        /// <returns></returns>
        public static bool CreateFile(string filePath, bool isOverride = true)
        {
            try
            {
                if (FileExists(filePath) && !isOverride)
                    return true;

                File.Delete(filePath);
                var stream = File.Create(filePath);
                stream.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Create a folder with folder path
        /// </summary>
        /// <param name="folderPath">folder path</param>
        /// <param name="isOverride">delete the folder if it's existing</param>
        /// <returns></returns>
        public static bool CreateFolder(string folderPath, bool isOverride = false)
        {
            try
            {
                var existing = FolderExists(folderPath);
                if (existing && !isOverride)
                    return true;

                if (existing)
                    Directory.Delete(folderPath);

                Directory.CreateDirectory(folderPath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Create write file stream
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileStream CreateWriteFileStream(string filePath)
        {
            try
            {
                if (FileExists(filePath))
                    DeleteFile(filePath);

                var stream = File.Create(filePath);
                return stream;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Check file is existing
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>True/False</returns>
        public static bool FileExists(string filePath)
        {
            try
            {
                return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Check folde is existing
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns>True/False</returns>
        public static bool FolderExists(string folderPath)
        {
            try
            {
                return !string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Check file is exist and has data
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ValidateFile(string filePath)
        {
            try
            {
                // Check existing
                var existing = FileExists(filePath);
                if (!existing)
                    return false;

                // Check file size
                var file = new FileInfo(filePath);
                return file.Length > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Get a read stream from file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileStream GetReadStream(string filePath)
        {
            try
            {
                var existing = FileExists(filePath);
                if (!existing)
                    return null;

                var stream = File.OpenRead(filePath);
                return stream;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Get a write stream file from file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileStream GetWriteFileStream(string filePath)
        {
            try
            {
                var existing = FileExists(filePath);
                if (!existing)
                    return null;

                var stream = File.OpenWrite(filePath);
                return stream;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Get a memory stream from file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static MemoryStream CreateMemoryStream(string filePath)
        {
            try
            {
                var existing = FileExists(filePath);
                if (!existing)
                    return null;

                // Get bytes
                var bytes = File.ReadAllBytes(filePath);
                if (!bytes.Any())
                    return null;

                // New memory stream
                var memoryStream = new MemoryStream(bytes);
                return memoryStream;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Get all files in folder
        /// </summary>
        /// <param name="folderPath">folder path</param>
        /// <returns>list of file path</returns>
        public static List<string> GetFileInFolder(string folderPath)
        {
            try
            {
                if (!FolderExists(folderPath))
                    return null;

                var files = Directory.GetFiles(folderPath).ToList();
                return files;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (!FileExists(filePath))
                    return false;

                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Move file
        /// </summary>
        /// <param name="souceFilePath">C:\a1.jpg</param>
        /// <param name="descFilePath">C:\Folder\a2.jpg</param>
        /// <returns></returns>
        public static bool MoveFile(string souceFilePath, string descFilePath)
        {
            try
            {
                if (!File.Exists(souceFilePath))
                    Debug.WriteLine("souceFilePath is not exits");
                if (File.Exists(descFilePath))
                    Debug.WriteLine("descFilePath is alredy exits");

                // https://forums.xamarin.com/discussion/42145/android-sharing-violation-on-file-rename
                File.Copy(souceFilePath, descFilePath);
                File.Delete(souceFilePath);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Delete folder
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <returns></returns>
        public static bool DeleteFolder(string folderPath)
        {
            try
            {
                if (!FolderExists(folderPath))
                    return false;

                // Delete files in folder
                var files = GetFileInFolder(folderPath) ?? new List<string>();
                foreach (var file in files)
                {
                    DeleteFile(file);
                }

                // Delete folders in folder
                var folders = Directory.GetDirectories(folderPath);
                foreach (var folder in folders)
                {
                    DeleteFolder(folder);
                }

                Directory.Delete(folderPath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Delete files in folder
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <returns></returns>
        public static bool DeleteFiles(string folderPath)
        {
            try
            {
                if (!FolderExists(folderPath))
                    return false;

                // Delete files in folder
                var files = GetFileInFolder(folderPath) ?? new List<string>();
                foreach (var file in files)
                {
                    DeleteFile(file);
                }

                // Delete folders in folder
                var folders = Directory.GetDirectories(folderPath);
                foreach (var folder in folders)
                {
                    DeleteFolder(folder);
                }

                // Result
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Copy a file to a folder
        /// </summary>
        /// <param name="folderPath">C:\Folder\</param>
        /// <param name="sourceFilePath">C:\a2.jpg</param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        public static bool CopyFileToFoler(string folderPath, string sourceFilePath, string newFileName = "")
        {
            try
            {
                if (!FolderExists(folderPath) || !FileExists(sourceFilePath))
                    return false;

                var fileName = !string.IsNullOrEmpty(newFileName) ? newFileName : GetFileName(sourceFilePath);

                // Create new file path
                var newFilePath = Path.Combine(folderPath, fileName);
                if (FileExists(newFilePath))
                    DeleteFile(newFilePath);

                File.Copy(sourceFilePath, newFilePath, true);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Copy folder
        /// </summary>
        /// <param name="sourceFolderPath"></param>
        /// <param name="destFolderPath"></param>
        /// <returns></returns>
        public static bool CopyFolderToFolder(string sourceFolderPath, string destFolderPath)
        {
            try
            {
                if (!FolderExists(sourceFolderPath))
                    return false;

                // Create destionate folder
                if (!FileExists(destFolderPath))
                    Directory.CreateDirectory(destFolderPath);

                var listFiles = GetFileInFolder(sourceFolderPath);
                if (!listFiles.Any())
                    return false;

                foreach (var filePath in listFiles)
                    File.Copy(filePath, Path.Combine(destFolderPath, Path.GetFileName(filePath)), true);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Write a stream data to file
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool? WriteBytesToFile(byte[] bytes, string filePath)
        {
            try
            {
                if (bytes == null || string.IsNullOrEmpty(filePath))
                    return false;

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Get bytes from file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    return null;

                var result = File.ReadAllBytes(filePath);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Set last write time
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool SetLastWriteTime(string filePath, DateTime? dateTime)
        {
            try
            {
                File.SetLastWriteTime(filePath, dateTime ?? DateTime.Now);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}
