using System.Text.RegularExpressions;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace ERPSystem.Common.Infrastructure;

public static class FileHelpers
{
    public static MemoryStream ConvertToStream(IFormFile file)
    {
        var ms = new MemoryStream();
        file.CopyTo(ms);
        return ms;
    }
    
    public static string FixBase64(this string base64)
    {
        if (string.IsNullOrEmpty(base64)) return "";
        try
        {
            var tempRegex = @"data:image/(.*?);base64,";
            var lsRegex = Regex.Matches(base64, tempRegex, RegexOptions.Singleline);
            if (lsRegex.Count > 0)
                base64 = base64.Substring(lsRegex[0].Value.Length);

            return base64.Trim();
        }
        catch
        {
            return base64;
        }
    }
    
    public static bool IsTextBase64(this string text)
    {
        if (string.IsNullOrEmpty(text)) return false;

        try
        {
            text = text.FixBase64();
            byte[] fromBase64String = Convert.FromBase64String(text);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public static bool SaveFileImage(string base64, string path, int maxSize = 0, int rotate = 0)
    {
        string[] folders = path.Split("/");
        string countFolder = folders[0];
        if (!Directory.Exists(countFolder))
        {
            Directory.CreateDirectory(countFolder);
        }
        for (int i = 1; i < folders.Length - 1; i++)
        {
            countFolder += $"/{folders[i]}";
            if (!Directory.Exists(countFolder))
            {
                Directory.CreateDirectory(countFolder);
            }
        }
        byte[] data = Convert.FromBase64String(base64.FixBase64());
        MagickImage image = new MagickImage(new MemoryStream(data));
        using (MemoryStream ms = new MemoryStream(data))
        {
            try
            {
                if (rotate != 0)
                {
                    image.Rotate(rotate);
                }

                if (maxSize != 0 && data.Length > maxSize)
                {
                    // ReSharper disable once PossibleLossOfFraction
                    var resize = Math.Sqrt(data.Length / maxSize);
                    if (resize >= 1)
                    {
                        int newWidth = (int)(image.Width / resize);
                        int newHeight = (int)(image.Height / resize);
                        image.Resize(newWidth, newHeight);
                    }
                }

                var output = image.ToByteArray();
                File.WriteAllBytes(path, output);
                ms.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[Exception Save Image]: " + ex.Message + ex.StackTrace);
                return false;
            }
        }

        return true;
    }
    
    public static bool DeleteFileFromLink(string link)
    {
        try
        {
            if (File.Exists(link))
            {
                File.Delete(link);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
    
    public static string GetExtensionOfFile(string pathFile)
    {
        try
        {
            return pathFile.Split('.').Last();
        }
        catch
        {
            return "";
        }
    }
    
    public static bool SaveFileByIFormFile(IFormFile file, string path)
    {
        try
        {
            string[] folders = path.Split("/");
            string countFolder = folders[0];
            if (!Directory.Exists(countFolder))
            {
                Directory.CreateDirectory(countFolder);
            }
            for (int i = 1; i < folders.Length - 1; i++)
            {
                countFolder += $"/{folders[i]}";
                if (!Directory.Exists(countFolder))
                {
                    Directory.CreateDirectory(countFolder);
                }
            }

            using (Stream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
    
    public static string GetContentTypeByFileName(string fileName)
    {
        string defaultContentType = "application/octet-stream";
        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(fileName, out string contentType))
        {
            contentType = defaultContentType;
        }

        return contentType;
    }

    public static bool CreateFolder(string path)
    {
        try
        {
            string[] folders = path.Split("/");
            string countFolder = folders[0];
            if (!Directory.Exists(countFolder))
            {
                Directory.CreateDirectory(countFolder);
            }

            for (int i = 1; i <= folders.Length - 1; i++)
            {
                countFolder += $"/{folders[i]}";
                if (!Directory.Exists(countFolder))
                {
                    Directory.CreateDirectory(countFolder);
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public static bool DeleteFolder(string path)
    {
        try
        {
            Directory.Delete(path, true);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public static bool EditNameOfFolder(string folderPath, string newFolderName)
    {
        try
        {
            // Check if the folder exists
            if (Directory.Exists(folderPath))
            {
                // Get the directory name from the folder path
                string directoryName = Path.GetDirectoryName(folderPath);

                // Form the new path with the new folder name
                string newFolderPath = Path.Combine(directoryName, newFolderName);

                // Rename the folder
                Directory.Move(folderPath, newFolderPath);

                Console.WriteLine("Folder renamed successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("The folder does not exist.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    public static bool EditNameOfFile(string oldPath, string newPath)
    {
        try
        {
            File.Move(oldPath, newPath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}