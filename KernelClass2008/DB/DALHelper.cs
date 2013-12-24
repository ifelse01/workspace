using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Data;

namespace KernelClass
{
    public static class DALHelper
    {
        public static string ConnectionString { get; set; }
		public static string ProviderName { get; set; }

        private static DbProviderFactory GetFactory()
        {
            return DbProviderFactories.GetFactory(GetProvider());
        }

        private static DbConnection GetConnection()
        {
            try
            {
                DbConnection connection = GetFactory().CreateConnection();
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                builder.ConnectionString = ConnectionString;
                builder.Remove("provider");
                connection.ConnectionString = builder.ToString();
                return connection;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to create connection: " + exception.Message);
                return null;
            }
        }
        
        public static DbCommand GetCommand()
        {
            return GetFactory().CreateCommand();
        }
        
        public static string GetProvider()
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            string str = "System.Data.SqlClient";
            if (builder.ContainsKey("provider"))
            {
                str = builder["provider"].ToString();
            }
			if(!string.IsNullOrEmpty(ProviderName)){
                str = ProviderName;
			}
            return str;
        }

        public static DbParameter GetParameter(string name, object value)
        {
            DbParameter parameter = GetFactory().CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }


        public static bool ExecuteScripts(List<string> scripts)
        {
            List<DbCommand> commands = new List<DbCommand>();
            foreach (string str in scripts)
            {
                if ((str != null) && (str.Trim().Length != 0))
                {
                    DbCommand item = GetCommand();
                    item.CommandText = str;
                    commands.Add(item);
                }
            }
            return ExecuteCommands(commands);
        }
        public static object ExecuteScalar(string sql, bool suppressErrors)
        {
            DbConnection connection = GetConnection();
            if (connection != null)
            {
                try
                {
                    try
                    {
                        DbCommand command = connection.CreateCommand();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        return command.ExecuteScalar();
                    }
                    catch (Exception exception)
                    {
                        if (!suppressErrors)
                        {
                            throw new Exception(sql, exception);
                        }
                    }
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
            return null;
        }

        public static object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, false);
        }

        public static void ExecuteNonQuery(string sql)
        {
            DbConnection connection = GetConnection();
            if (connection != null)
            {
                try
                {
                    try
                    {
                        DbCommand command = connection.CreateCommand();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(sql, exception);
                    }
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static bool ExecuteCommands(List<DbCommand> commands)
        {
            bool flag;
            DbConnection connection = GetConnection();
            if (connection == null)
            {
                return false;
            }
            DbTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                foreach (DbCommand command in commands)
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
                flag = true;
            }
            catch (Exception exception)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                flag = false;
                throw new Exception(exception.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return flag;
        }


        public static DataSet ExecuteQuery(string sql)
        {
            DataSet returnValue = null;

            DbConnection connection = GetConnection();
            if (connection != null)
            {
                try
                {
                    try
                    {                        
                        DbCommand command = connection.CreateCommand();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        connection.Open();

                        DbDataAdapter adapter = GetFactory().CreateDataAdapter();
                        adapter.SelectCommand = command;
                        returnValue = new DataSet();
                        adapter.Fill(returnValue);
                    }
                    catch (Exception ex)
                    {
						throw new Exception(sql, ex);
                    }
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }

            return returnValue;
        }


        public static DbDataReader ExecuteReader(string sql)
        {
            DbDataReader dr = null;
            DbConnection connection = GetConnection();
            if (connection != null)
            {
                try
                {
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    connection.Open();

                    dr = command.ExecuteReader();

                }
                catch (Exception ex)
                {				
                    throw new Exception(sql, ex);
                }
            }
            return dr;
        }
    }
}
