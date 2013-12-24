using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.IO;
using System.Data;
using System.Web;
using System.Data.Odbc;
using System.Text.RegularExpressions;
using System.Collections;

namespace KernelClass
{
    public class CSVHelper
    {

        /// <summary>
        /// The class CSVHelper read the data from CSV file and store it in a DataTable class,
        /// and allows to return a column with index or column name.
        /// </summary>
        private static readonly char[] FormatSplit = new char[] { ',' };
        private const string replaceDoubleQuotes = "$replaceDoubleQuotes$";
        private const string tableName = "csvTable";
        private const string lastEmptyColumnName = "$lastEmptyColumnName$";

        public static DataTable ReadCSV(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (fi == null || !fi.Exists) return null;

            StreamReader reader = new StreamReader(filePath);

            string line = string.Empty;
            int lineNumber = 0;

            DataTable dt = new DataTable();

            while ((line = reader.ReadLine()) != null)
            {
                if (lineNumber == 0)
                {
                    dt = CreateDataTable(line);
                    if (dt.Columns.Count == 0) return null;
                }
                else
                {
                    bool isSuccess = CreateDataRow(ref dt, line);
                    if (!isSuccess)
                    {
                        throw new FileLoadException("There are some data unconsistent in your file.");
                    }
                }
                lineNumber++;
            }
                reader.Close();

			//如果最后一列是空的话就删除
            int indexLastestColumn = dt.Columns.Count -1;
            if (dt.Columns[indexLastestColumn].ColumnName == lastEmptyColumnName)
            {
                bool HasValueForLastestColumn = false;
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i][indexLastestColumn].ToString().Trim()))
                    {
                        HasValueForLastestColumn = true;
                        break;
                    }
                }
                if (!HasValueForLastestColumn)
                {
                    dt.Columns.RemoveAt(indexLastestColumn);
                }
            }
            return dt;
        }

        public static bool WriteCSV(DataTable _dataSourc, string filePath)
        {
            return WriteCSV(_dataSourc, filePath, false, System.Text.Encoding.UTF8);
        }

        public static bool WriteCSV(DataTable _dataSourc, string filePath, bool append, System.Text.Encoding encoding)
        {
            string data = ExportCSV(_dataSourc);
            return PhysicalFile.SaveFile(data, filePath, append, encoding);
        }

        public static string ExportCSV(DataTable _dataSource)
        {
            StringBuilder strbData = new StringBuilder();
            foreach (DataColumn column in _dataSource.Columns)
            {
                strbData.Append(column.ColumnName + ",");
            }
            strbData.Append("\n");
            foreach (DataRow dr in _dataSource.Rows)
            {
                for (int i = 0; i < _dataSource.Columns.Count; i++)
                {
                    string rowValue = dr[i].ToString().Replace("\"", "\"\"");
                    if(rowValue.Contains(','))
                        strbData.Append("\"" + dr[i].ToString().Replace("\"", "\"\"") + "\",");
                    else
                        strbData.Append(dr[i].ToString() + ",");
                }
                strbData.Append("\n");
            }
            return strbData.ToString();
        }

        public static List<double> GetColumnWithName(string columnName, DataTable dt)
        {
            List<double> list = new List<double>();

            var index = dt.Columns.IndexOf(columnName);
            list = GetColumnWithIndex(index, dt);

            return list;
        }

        public static List<double> GetColumnWithIndex(int index, DataTable dt)
        {
            List<double> list = new List<double>();

            foreach (DataRow dr in dt.Rows)
            {
                var s = dr[index].ToString();
                if (string.Compare(s, "") == 0)
                {
                    break;
                }
                double value = Convert.ToDouble(s);
                list.Add(value);
            }

            return list;
        }

        private static DataTable CreateDataTable(string line)
        {
            DataTable dt = new DataTable();
            string[] fields = line.Split(FormatSplit, StringSplitOptions.None);
            for(var i=0; i<fields.Length; i++)
            {
                if (string.IsNullOrEmpty(fields[i]) && i == fields.Length - 1)
                    dt.Columns.Add(lastEmptyColumnName);
                else
                    dt.Columns.Add(fields[i]);
            }
            return dt;
        }

        private static bool CreateDataRow(ref DataTable dt, string line)
        {
            DataRow dr = dt.NewRow();
            string src = string.Empty;
            Hashtable fields = new Hashtable();

            if (!string.IsNullOrEmpty(line))
            {
                src = line.Replace("\"\"", replaceDoubleQuotes);
				//正则表达式找出用双引号包括的字符串， 下面循环是为了防止字符串中含有分隔符 ,
                MatchCollection col = Regex.Matches(src, "\"([^\"]+)\"", RegexOptions.ExplicitCapture);
                IEnumerator ie = col.GetEnumerator();

                while (ie.MoveNext())
                {
                    string patn = ie.Current.ToString();
                    int key = src.Substring(0, src.IndexOf(patn)).Split(',').Length-1;

                    if (!fields.ContainsKey(key))
                    {
                        fields.Add(key, patn.Trim(new char[] { ',', '"' }));
                        src = src.Replace(patn, "");
                    }
                }

                string[] arr = src.Split(',');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (!fields.ContainsKey(i))
                        fields.Add(i, arr[i]);
                }
            }

            if (fields.Count == 0 || fields.Count > dt.Columns.Count)
            {
                return false;
            }

            for (int i = 0; i < fields.Count; i++)
            {
                dr[i] = fields[i].ToString().Replace(replaceDoubleQuotes, "\"");
            }

            dt.Rows.Add(dr);
            return true;
        }
    }

}
