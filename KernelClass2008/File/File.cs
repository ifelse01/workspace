using System.IO;
using System;

namespace KernelClass
{
    public class File
    {
        public static readonly char separatorChar = Path.DirectorySeparatorChar;
        public static readonly string separator = "" + Path.DirectorySeparatorChar;

        private string _path;

        public File(string path)
        {
            _path = path;
        }

        public File(string dir, string file)
        {
            if (dir == null)
            {
                _path = file;
            }
            else
            {
                _path = Path.Combine(dir, file);
            }
        }

        public virtual bool Delete()
        {
            if (Exists())
            {
                System.IO.File.Delete(_path);
                return !Exists();
            }
            return false;
        }

        public bool Exists()
        {
            return System.IO.File.Exists(_path) || Directory.Exists(_path);
        }

        public string GetCanonicalPath()
        {
            return Path.GetFullPath(_path);
        }

        public string GetAbsolutePath()
        {
            return Path.GetFullPath(_path);
        }

        public string GetName()
        {
            int index = _path.LastIndexOf(separator);
            return _path.Substring(index + 1);
        }

        public string GetPath()
        {
            return _path;
        }

        public bool IsDirectory()
        {
#if CF_1_0 || CF_2_0
			return System.IO.Directory.Exists(_path);
#else
            return (System.IO.File.GetAttributes(_path) & FileAttributes.Directory) != 0;
#endif
        }

        public long Length()
        {
            return new FileInfo(_path).Length;
        }

        /// <summary>
        /// 得到文件夹下的所有文件名（含路径）
        /// </summary>
        /// <returns></returns>
        public string[] List()
        {
            return Directory.GetFiles(_path);
        }

        public string GetFolderPath()
        {
            return _path.Substring(0, _path.LastIndexOf("\\") + 1);
        }
        public bool MkFile()
        {
            string folderPath = GetFolderPath();
            File fo = new File(folderPath);
            fo.Mkdirs();

            if (Exists())
            {
                return false;
            }
            new FileInfo(_path).Create();
            return Exists();
        }

        public string ReadFile()
        {
            try
            {
                string str = "";
                using (StreamReader objStreamReader = new StreamReader(_path))
                {
                    str = objStreamReader.ReadToEnd();
                }
                return str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SaveFile(string content)
        {
            try
            {
                string str = "";
                using (StreamWriter streamWriter = new StreamWriter(_path))
                {
                    streamWriter.Write(content);
                    streamWriter.Flush();
                }
                return str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Mkdir()
        {
            if (Exists())
            {
                return false;
            }
            Directory.CreateDirectory(_path);
            return Exists();
        }

        public bool Mkdirs()
        {
            if (Exists())
            {
                return false;
            }
            int pos = _path.LastIndexOf(separator);
            if (pos > 0)
            {
                new File(_path.Substring(0, pos)).Mkdirs();
            }
            return Mkdir();
        }

        public void RenameTo(File file)
        {
            new FileInfo(_path).MoveTo(file.GetPath());
        }
    }
}