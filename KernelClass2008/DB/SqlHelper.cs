using System;
using System.Collections.Generic;

using System.Data;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;

namespace KernelClass
{
    //也支持其他数据库
    public class SqlHelper:IDisposable
    {
        //protected string DefaultConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
        protected string DefaultConnectionString = ConfigurationManager.AppSettings["LocalConnectionString"];
        //protected string DefaultProviderName = ConfigurationManager.AppSettings["DbHelperProvider"];
        private string _provideName = (null == ConfigurationManager.AppSettings["DbHelperProvider"] ? "System.Data.SqlClient" : ConfigurationManager.AppSettings["DbHelperProvider"]);
        private string _connectionString = string.Empty;
        private DbConnection _connection = null;
        private DbTransaction _transaction = null;
        private bool _disposed = false;
        public string ProvideName {
            get {
                return _provideName; 
            }
            set { _provideName = value; }
        }
        
        private static Dictionary<string, DbProviderFactory> _dataFactorys = new Dictionary<string, DbProviderFactory>();

        /// <summary>
        /// make default connect from config file the key LocalConnectionString
        /// default provideName from config file the key DbHelperProvider -- System.Data.SqlClient
        /// </summary>
        public SqlHelper() {
            _connectionString = DefaultConnectionString;
        }

        public SqlHelper(string connectionString){
            _connectionString = connectionString;
        }

        private DbProviderFactory GetFactory()
        {
            return DbProviderFactories.GetFactory(_provideName);
        }

        public void OpenConnection() {
            try
            {
                if (_connection == null)
                {
                    _connection = GetFactory().CreateConnection();
                    _connection.ConnectionString = _connectionString;
                    //_connection = new SqlConnection(_connectionString);
                }
                if (_connection.State == System.Data.ConnectionState.Closed)
                {
                    _connection.Open();
                }
            }
            catch { }
        }

        public void CloseConnection(){
            try
            {
                if (_connection != null)
                {
                    if (_connection.State != ConnectionState.Closed)
                    {
                        _connection.Close();
                    }
                }
            }
            catch { }
        }

        public void BeginTransaction() {
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Commit();
                    _transaction = null;
                }
            }
            catch { }
        }

        public void RollbackTransaction()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction = null;
                }
            }
            catch { }
        }

         private DbProviderFactory GetDataFactory()
        {
            if (!_dataFactorys.ContainsKey(_provideName))
            {
                DbProviderFactory dataFactory = DbProviderFactories.GetFactory(_provideName);
                _dataFactorys.Add(_provideName, dataFactory);
            }

            return _dataFactorys[_provideName];
        }
        
        public void Dispose() 
        {
            if (!this._disposed)
            {
                if (_connection != null)
                {
                    RollbackTransaction();
                    CloseConnection();
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ExecuteNonQuery(string sql)
        {
            using (DbCommand command = _connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }
                return command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public DataSet ExecuteQuery(string sql)
        {
            DataSet returnValue = null;

            using (DbCommand command = _connection.CreateCommand())
            {
                DbDataAdapter adapter = GetDataFactory().CreateDataAdapter();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }
                adapter.SelectCommand = command;
                returnValue = new DataSet();
                adapter.Fill(returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSet ExecuteQuery(string sql, string TableName)
        {
            DataSet returnValue = null;

            using (DbCommand command = _connection.CreateCommand())
            {
                DbDataAdapter adapter = GetDataFactory().CreateDataAdapter();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }
                adapter.SelectCommand = command;
                returnValue = new DataSet();
                adapter.Fill(returnValue, TableName);
            }

            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public object ExecuteScalar(string sql)
        {
            using (DbCommand command = _connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }
                return command.ExecuteScalar();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ExecuteStoredProcNonQuery(string storedProc, Dictionary<string, object> parameters)
        {
            using (DbCommand command = _connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = storedProc;
                SetCommandParameters(command, parameters);
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, _transaction, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;

        }
        /// <summary>
        /// 
        /// </summary>
        public SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            try
            {
                PrepareCommand(cmd, _transaction, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                throw;
            }
            finally
            {
                //conn.Close();
            }
        }

        /// <summary>
        /// ExecuteReader
        /// </summary>
        public SqlDataReader ExecuteReader(string sql)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            using (DbCommand command = _connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;

                return (SqlDataReader)command.ExecuteReader();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public DataSet ExecuteDataSet(CommandType cmdType, string cmdText, DataSet ds, string tableName, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = cmdType;
            cmd.CommandText = cmdText;
            PrepareCommand(cmd, _transaction, cmdType, cmdText, commandParameters);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            //DataSet ds = new DataSet();
            sda.Fill(ds, tableName);

            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public DataSet ExecuteDataSet(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = cmdType;
            cmd.CommandText = cmdText;
            PrepareCommand(cmd, _transaction, cmdType, cmdText, commandParameters);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;
        }
        public DataSet ExecuteDataSet(CommandType cmdType, string cmdText, string tableName, params SqlParameter[] commandParameters)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = cmdType;
            cmd.CommandText = cmdText;
            PrepareCommand(cmd, _transaction, cmdType, cmdText, commandParameters);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds, tableName);
            return ds;
        }
        /// <summary>
        /// 
        /// </summary>
        public int ExecuteStoredProcNonQuerys(CommandType comType, string storedProc, params SqlParameter[] parameters)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, _transaction, comType, storedProc, parameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;

        }
        /// <summary>
        /// 
        /// </summary>
        private void PrepareCommand(SqlCommand cmd, DbTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            cmd.Connection = (SqlConnection)_connection;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = (SqlTransaction)trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataSet ExecuteStoredProcQuery(string storedProc, Dictionary<string, object> parameters)
        {
            DataSet returnValue = null;

            using (DbCommand command = _connection.CreateCommand())
            {
                DbDataAdapter adapter = GetDataFactory().CreateDataAdapter();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = storedProc;
                SetCommandParameters(command, parameters);
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }
                adapter.SelectCommand = command;
                returnValue = new DataSet();
                adapter.Fill(returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ExecuteStoredProc(string storedProc, Dictionary<string, object> parameters)
        {
            int returnValue = 0;

            using (DbCommand command = _connection.CreateCommand())
            {
                DbDataAdapter adapter = GetDataFactory().CreateDataAdapter();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = storedProc;
                SetCommandParameters(command, parameters);
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }
                returnValue = command.ExecuteNonQuery();
            }

            return returnValue;
        }


        /// <summary>
        /// 
        /// </summary>
        public DataSet ExecuteStoredProcQuery(string storedProc, Dictionary<string, object> parameters, string TableName)
        {
            DataSet returnValue = null;

            using (DbCommand command = _connection.CreateCommand())
            {
                DbDataAdapter adapter = GetDataFactory().CreateDataAdapter();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = storedProc;
                SetCommandParameters(command, parameters);
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }
                adapter.SelectCommand = command;
                returnValue = new DataSet();
                adapter.Fill(returnValue, TableName);
            }

            return returnValue;
        }


        /// <summary>
        /// 
        /// </summary>
        public object ExecuteStoredProcScalar(string storedProc, Dictionary<string, object> parameters)
        {
            using (DbCommand command = _connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = storedProc;
                SetCommandParameters(command, parameters);
                if (_transaction != null)
                {
                    command.Transaction = _transaction;
                }
                return command.ExecuteScalar();
            }
        }

        public object ExecuteScalar(CommandType cmdType, string cmdText, Dictionary<string, object> parameters)
        {
            using (DbCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandType = cmdType;
                cmd.CommandText = cmdText;
                SetCommandParameters(cmd, parameters);
                if (_transaction != null)
                {
                    cmd.Transaction = _transaction;
                }
                object oResult = cmd.ExecuteScalar();

                cmd.Parameters.Clear();
                return oResult;
            }
        }

        public string[] ExecuteScalarWithOutPut(string cmdText, ref SqlParameter[] parameters, string[] OutPutParameters)
        {
            using (DbCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = cmdText;
                if (_transaction != null)
                {
                    cmd.Transaction = _transaction;
                }
                //SetCommandParameters(cmd, parameters);
                foreach (SqlParameter sp in parameters)
                {
                    cmd.Parameters.Add(sp);
                }
                foreach (String sOutPut in OutPutParameters)
                {
                    cmd.Parameters[sOutPut].Direction = ParameterDirection.Output;
                }

                object oResult = cmd.ExecuteScalar();
                if (oResult != null)
                {
                    int index = 0;
                    foreach (String sOutPut in OutPutParameters)
                    {
                        OutPutParameters[index] = cmd.Parameters[sOutPut].Value.ToString();
                        index++;
                    }
                    cmd.Parameters.Clear();
                    return OutPutParameters;
                }
                else
                {
                    cmd.Parameters.Clear();
                    return null;
                }
            }
        }

        public void ExecuteStoredProcWithOutput(string StoredProcedureName, SqlParameter[] parameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, _transaction, CommandType.StoredProcedure, StoredProcedureName, parameters);
                cmd.ExecuteNonQuery();
            }
        }

        public DataSet ExecuteStoredProcQueryWithOutput(string StoredProcedureName, SqlParameter[] parameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                PrepareCommand(cmd, _transaction, CommandType.StoredProcedure, StoredProcedureName, parameters);
                DbDataAdapter adapter = GetDataFactory().CreateDataAdapter();
                adapter.SelectCommand = cmd;
                DataSet returnValue = new DataSet();
                adapter.Fill(returnValue);
                return returnValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        protected void SetCommandParameters(DbCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null) return;

            DbParameter parameterDb = null;
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                parameterDb = GetDataFactory().CreateParameter();
                parameterDb.ParameterName = parameter.Key;
                parameterDb.Value = parameter.Value;
                command.Parameters.Add(parameterDb);
            }
        }
    }
}