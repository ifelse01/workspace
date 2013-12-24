using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace KernelClass
{
    ///   <summary> 
    ///   通过ADO方式的Excel文件操作类 
    ///   </summary> 
    public class ExcelDatabase
    {
        #region  常量定义
        const string _DataSourceStr1 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{0}';Extended Properties='Excel 8.0;Imex=2;HDR=YES;'";
        const string _DataSourceStr2 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties='Excel 8.0;HDR=YES;'";
        const int _TableName = 2;
        #endregion

        #region   构造函数

        ///   <summary> 
        ///   带参数的构造 
        ///   </summary> 
        ///   <param   name= "FileName "> Excel文件名 </param> 
        public ExcelDatabase(string FileName)
        {
            this.ExcelFileName = FileName;

            /////构造数据库对象 
            //this.mConn = new OleDbConnection(GetConnectionString(FileName));

            this.mCommSelect = new OleDbCommand();
            this.mCommUpdate = new OleDbCommand();

            this.mCommSelect.Connection = this.mConn;
            this.mCommUpdate.Connection = this.mConn;

            this.mDA = new OleDbDataAdapter();

            this.mDA.SelectCommand = this.mCommSelect;
            this.mDA.UpdateCommand = this.mCommUpdate;
        }

        /////   <summary> 
        /////   不带参数的构造 
        /////   </summary> 
        //public ExcelDatabase()
        //{
        //    ///构造数据库对象 
        //    this.mConn = new OleDbConnection();

        //    this.mCommSelect = new OleDbCommand();
        //    this.mCommUpdate = new OleDbCommand();

        //    this.mCommSelect.Connection = this.mConn;
        //    this.mCommUpdate.Connection = this.mConn;

        //    this.mDA = new OleDbDataAdapter();

        //    this.mDA.SelectCommand = this.mCommSelect;
        //    this.mDA.UpdateCommand = this.mCommUpdate;
        //}

        #endregion

        #region   对象定义
        ///   <summary> 
        ///   要操作的Excel文件名称 
        ///   </summary> 
        private string mExcelFileName;
        ///   <summary> 
        ///   链接对象 
        ///   </summary> 
        private System.Data.OleDb.OleDbConnection mConn = null;
        ///   <summary> 
        ///   提取数据命令对象 
        ///   </summary> 
        private System.Data.OleDb.OleDbCommand mCommSelect = null;
        ///   <summary> 
        ///   更新数据命令对象 
        ///   </summary> 
        private System.Data.OleDb.OleDbCommand mCommUpdate = null;
        ///   <summary> 
        ///   数据适配器对象 
        ///   </summary> 
        private System.Data.OleDb.OleDbDataAdapter mDA = null;
        #endregion

        #region   属性
        public string ExcelFileName
        {
            get { return this.mExcelFileName; }

            set
            {
                if (this.mConn != null)
                {
                    this.mConn.Close();
                    this.mConn.Dispose();
                    this.mConn = null;
                }

                this.mExcelFileName = value;
                this.mConn = new OleDbConnection(string.Format(_DataSourceStr1, mExcelFileName));

            }
        }
        #endregion

        #region  内部方法定义
        private void openConn() {
            try
            {
                this.mConn.Open();
            }
            catch
            {
                this.mConn = new OleDbConnection(string.Format(_DataSourceStr2, this.mExcelFileName));
                this.mConn.Open();
            }
        }

        /// <summary>
        /// 获取字段名集合串（以“,”分割）
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private string GetColumnsStr(DataColumnCollection columns)
        {
            string tempColumnStr = string.Empty;
            if (columns != null)
            {
                foreach (DataColumn tempDataColumn in columns)
                {
                    tempColumnStr += string.Format("[{0}],", tempDataColumn.ColumnName);
                }
                tempColumnStr = tempColumnStr.EndsWith(",") ? tempColumnStr.Substring(0, tempColumnStr.Length - 1) : tempColumnStr;
            }
            return tempColumnStr;
        }

        /// <summary>
        /// 获取数据行值集合串（以“,”分割）
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private string GetRowValuesStr(DataRow dr)
        {
            string tempValueStr = string.Empty;
            if (dr != null)
            {
                foreach (object tempItem in dr.ItemArray)
                {
                    tempValueStr += string.Format("'{0}',", tempItem.ToString());
                }
                tempValueStr = tempValueStr.EndsWith(",") ? tempValueStr.Substring(0, tempValueStr.Length - 1) : tempValueStr;
            }
            return tempValueStr;
        }


        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="SQL">SQL语句</param>
        /// <returns></returns>
        private int ExecuteNonQuery(string sqlText)
        {
            int ReYS = 0;
            try
            {
                openConn();//打开文件
                OleDbCommand cmd = new OleDbCommand(sqlText, this.mConn);
                ReYS = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                ReYS = 0;
            }
            finally
            {
                this.mConn.Close();
            }
            return ReYS;
        }

        /////   <summary> 
        /////   添加列名和逗号 
        /////   </summary> 
        /////   <param   name= "ColName "> 列名 </param> 
        /////   <returns> </returns> 
        //private static string AddColWithComma(string ColName)
        //{
        //    ColName += ", ";

        //    return ColName;

        //}


        /////   <summary> 
        /////   添加列名和逗号 
        /////   </summary> 
        /////   <param   name= "ColName "> 列名 </param> 
        /////   <param   name= "Value "> 值 </param> 
        /////   <returns> </returns> 
        //private static string AddColWithComma(string ColName, string Value)
        //{
        //    ColName = ColName + "   =   " + Value;
        //    ColName += ", ";

        //    return ColName;

        //}


        /////   <summary> 
        /////   添加列名和空格 
        /////   </summary> 
        /////   <param   name= "ColName "> 列名 </param> 
        /////   <returns> </returns> 
        //private static string AddColWithSpace(string ColName)
        //{
        //    ColName += "   ";

        //    return ColName;
        //}


        /////   <summary> 
        /////   添加列名和空格 
        /////   </summary> 
        /////   <param   name= "ColName "> 列名 </param> 
        /////   <param   name= "Value "> 值 </param> 
        /////   <returns> </returns> 
        //private static string AddColWithSpace(string ColName, string Value)
        //{
        //    ColName = ColName + "   =   " + Value;

        //    ColName += "   ";

        //    return ColName;
        //}
        #endregion

        #region   公有方法
        /// <summary>
        /// 判断Sheet是否存在
        /// </summary>
        /// <param name="SheetName">Sheet名称</param>
        /// <returns></returns>
        public bool IsExistSheets(string SheetName)
        {
            IList<string> tempList = this.GetExcelSheetNames();
            return tempList.Contains(SheetName);
        }

        ///   <summary> 
        ///   返回Excel表中的Sheets 
        ///   </summary> 
        ///   <returns> Sheets的名称 </returns> 
        public IList<string> GetExcelSheetNames()
        {
            System.Data.DataTable dt = null;
            string[] res = null;
            IList<string> resList = new List<string>();
            try
            {
                //打开数据库链接 
                openConn();

                //将Sheets获取到表中 
                dt = this.mConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (null != dt)
                {
                    res = new string[dt.Rows.Count];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        res[i] = dt.Rows[i][_TableName].ToString();
                        resList.Add(res[i].Substring(0, res[i].Length - 1));
                    }
                    this.mConn.Close();
                }
                else
                {
                    res = null;
                }
            }
            catch (System.Data.OleDb.OleDbException OleEx)
            {
                System.Diagnostics.Debug.WriteLine(OleEx.ToString());
                this.mConn.Close();
            }

            return resList;
        }

        ///   <summary> 
        ///   根据工作表名称获取工作表数据 
        ///   </summary> 
        ///   <param   name= "SheetName "> 工作表名称 </param> 
        ///   <returns> 数据表对象 </returns> 
        public System.Data.DataTable GetSheetData(string SheetName)
        {
            string sql = string.Format("SELECT * FROM [{0}$]", SheetName);
            System.Data.DataTable dt = null;
            try
            {
                openConn();
                dt = new DataTable(SheetName);
                this.mDA = new OleDbDataAdapter(sql, this.mConn);
                this.mDA.FillSchema(dt, System.Data.SchemaType.Source);
                this.mDA.Fill(dt);
            }
            catch (System.Data.OleDb.OleDbException OleEx)
            {
                System.Diagnostics.Debug.WriteLine(OleEx.ToString());
            }
            finally
            { this.mConn.Close(); }
            return dt;
        }

        /// <summary>
        /// 创建Excel表中的Sheets 
        /// </summary>
        /// <param name="SheetName"></param>
        /// <param name="dataColumn"></param>
        public bool DelExcelSheet(String SheetName)
        {
            try
            {
                if (this.IsExistSheets(SheetName))
                {
                    string tempSql = string.Format("Drop Table [{0}]", SheetName);
                    openConn();//打开文件
                    OleDbCommand tempCmd = new OleDbCommand(tempSql, this.mConn);
                    return tempCmd.ExecuteNonQuery() > 0;
                }
                else { return true; }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            finally
            {
                this.mConn.Close();
            }
            return false;
        }

        
        public bool AddSheet(System.Data.DataTable dt, string sheetName)
        {
            if (dt != null && CreateSheetFrame(sheetName, dt.Columns))
            {//表不为空 且 创建Sheets架构成功
                foreach (DataRow tempRow in dt.Rows)
                {
                    this.AddRow(sheetName, dt.Columns, tempRow);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 添加Excel表中的Sheets 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool AddSheet(System.Data.DataTable dt)
        {
            return AddSheet(dt, dt.TableName);
        }

        /// <summary>
        /// 添加Sheets中的行数据
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public bool AddRow(string tableName, DataColumnCollection columns, DataRow dr)
        {
            if (dr != null)
            {
                string tempColumnsStr = GetColumnsStr(columns);
                string tempRowValueStr = GetRowValuesStr(dr);
                string tempSql = string.Format("Insert Into [{0}$] ({1}) values ({2})", tableName, tempColumnsStr, tempRowValueStr);
                this.ExecuteNonQuery(tempSql);///执行语句
            }
            return false;
        }

        /// <summary>
        /// 创建Excel表中的Sheets架构
        /// </summary>
        /// <param name="SheetName"></param>
        /// <param name="columns"></param>
        public bool CreateSheetFrame(String SheetName, DataColumnCollection columns)
        {
            try
            {
                this.DelExcelSheet(SheetName);///先删除这个表

                string tempColumnStr = GetColumnsStr(columns);
                tempColumnStr = tempColumnStr.Replace(",", " varchar(254),") + " varchar(254)";

                string tempSql = string.Format("create table [{0}] ({1})", SheetName, tempColumnStr);
                this.ExecuteNonQuery(tempSql);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            finally
            {
                this.mConn.Close();
            }
            return false;
        }

        /////   <summary> 
        /////   更新Excel表中的数据 
        /////   </summary> 
        /////   <param   name= "SheetName "> 工作表名称 </param> 
        /////   <param   name= "dt "> 数据表 </param> 
        //public void UpdateSheetData(String SheetName, System.Data.DataTable dt)
        //{
        //    try
        //    {
        //        ///构建更新命令   
        //        this.mCommUpdate.CommandText = string.Format("UPDATE [{0}$] SET ", SheetName);
        //        string parameters = null;
        //        for (int i = 0; i < dt.Columns.Count - 1; i++)
        //        {
        //            parameters += AddColWithComma(dt.Columns[i].ToString(), "? ");

        //        }

        //        parameters += AddColWithSpace(dt.Columns[dt.Columns.Count - 1].ToString(), "? ");

        //        this.mCommUpdate.CommandText += parameters;

        //        this.mCommUpdate.CommandText = this.mCommUpdate.CommandText + "WHERE   " + AddColWithSpace(dt.Columns[0].ToString(), "? ");

        //        ///添加参数 
        //        System.Data.OleDb.OleDbParameter UpdatePara = null;

        //        for (int j = 0; j < dt.Columns.Count; j++)
        //        {
        //            UpdatePara = new OleDbParameter("? ", dt.Columns[j].DataType.ToString());
        //            UpdatePara.SourceColumn = dt.Columns[j].ColumnName;
        //            this.mCommUpdate.Parameters.Add(UpdatePara);
        //        }

        //        UpdatePara = new OleDbParameter("? ", dt.Columns[0].DataType.ToString());
        //        UpdatePara.SourceColumn = dt.Columns[0].ColumnName;
        //        this.mCommUpdate.Parameters.Add(UpdatePara);

        //        try
        //        {
        //            DataRowCollection tempRows = dt.Rows;
        //            this.mDA.Update(dt);

        //        }
        //        catch (System.Data.OleDb.OleDbException e)
        //        {
        //            //ErrorWnd Wnd = new ErrorWnd("更新数据出错 ", e.Message);
        //            //Wnd.ShowDialog();
        //        }
        //    }
        //    catch (Exception ex) 
        //    { }
        //}

        #endregion
    }
}
