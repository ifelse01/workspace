using System.IO;
using System;

namespace KernelClass
{
    public static class PhysicalFile
    {
        public static readonly string separator = "" + Path.DirectorySeparatorChar;

        public static bool FolderExists(string folderPath)
        {
            return Directory.Exists(folderPath);
        }

        public static bool FileExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        public static bool CreateFolder(string folderPath)
        {
            if (FolderExists(folderPath))
            {
                return false;
            }
            Directory.CreateDirectory(folderPath);
            return FolderExists(folderPath);
        }

        public static string GetFolderPath(string filePath)
        {
            return filePath.Substring(0, filePath.LastIndexOf("\\") + 1);
        }

        public static bool CreateFile(string filePath)
        {
            string folderPath = PhysicalFile.GetFolderPath(filePath);
            PhysicalFile.CreateFolder(folderPath);

            if (PhysicalFile.FileExists(filePath))
            {
                return false;
            }
            new FileInfo(filePath).Create().Close();
            return PhysicalFile.FileExists(filePath);
        }

        public static string ReadFile(string filePath)
        {
            try
            {
                //string str = "";
                System.Text.StringBuilder str = new System.Text.StringBuilder();
                using (StreamReader objStreamReader = new StreamReader(filePath, System.Text.Encoding.UTF8))
                {
                    //str = objStreamReader.ReadToEnd();
                    while (objStreamReader.Peek() >= 0)
                    {
                        str.Append(objStreamReader.ReadLine()).AppendLine();
                    }

                    objStreamReader.Close();
                }
                //return str;
                return str.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SaveFile(string content, string filePath)
        { 
            return SaveFile(content, filePath, true);
        }


        public static bool SaveFile(string content, string filePath, bool append)
        {
            return SaveFile(content, filePath, append, System.Text.Encoding.UTF8);
        }

        public static bool SaveFile(string content, string filePath, bool append, System.Text.Encoding encoding)
        {
            if (!FileExists(filePath))
                CreateFile(filePath);
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath, append, encoding))
                {
                    streamWriter.Write(content);
                    streamWriter.Flush();
                }
            }
            catch
            {
                return false;
            }
            return true;
            
        }

        public static bool SaveFile(byte[] content, string filePath)
        {
            if (!FileExists(filePath))
                CreateFile(filePath);
            try
            {
                MemoryStream m = new MemoryStream(content);
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    m.WriteTo(fs);
                }
                m.Close();
            }
            catch
            {
                return false;
            }
            return true;

        }

        public static bool AddToFile(string content, string filePath)
        {
            if (!FileExists(filePath))
                CreateFile(filePath);

            FileStream fs = null;
            try
            {
                byte[] bData = System.Text.Encoding.UTF8.GetBytes(content.ToString());

                fs = new FileStream(filePath, FileMode.Append);//FileMode.CreateNew
                fs.Write(bData, 0, bData.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            return true;
        }

        public static bool DeleteFile(string filePath)
        {
            if (FileExists(filePath))
            {
                System.IO.File.Delete(filePath);
                return !FileExists(filePath);
            }
            return false;
        }

        public static bool DeleteFolder(string folderPath)
        {
            if (FolderExists(folderPath))
            {
                System.IO.Directory.Delete(folderPath);
                return !FolderExists(folderPath);
            }
            return false;
        }

        public static string GetName(string _path)
        {
            int index = _path.LastIndexOf(separator);
            return _path.Substring(index + 1);
        }

        public static bool RenameFile(string sourceFilePath, string destFilePath)
        {
            if (!FileExists(sourceFilePath) || FileExists(destFilePath))
                return false;
            new FileInfo(sourceFilePath).MoveTo(destFilePath);
            return !FileExists(sourceFilePath) && FileExists(destFilePath);
        }

        public static bool RenameFolder(string sourceFolderPath, string destFolderPath)
        {
            if (!FolderExists(sourceFolderPath) || FolderExists(destFolderPath))
                return false;
            Directory.Move(sourceFolderPath, destFolderPath);
            return !FolderExists(sourceFolderPath) && FolderExists(destFolderPath);
        }
        
        /// <summary>
        /// 得到文件夹下的所有文件名（含路径）
        /// </summary>
        /// <returns></returns>
        public static string[] List(string folderPath)
        {
            string[] str = new string[]{""};
            if (FolderExists(folderPath))
                str= Directory.GetFiles(folderPath);

            return str;
        }

        /// <summary>
        /// 获得文件的后缀
        /// 不带点，小写
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileExt(string filePath)
        {
            return filePath.Substring(filePath.LastIndexOf(".") + 1, filePath.Length - filePath.LastIndexOf(".") - 1).ToLower();
        }
    }
}