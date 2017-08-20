using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace Erow.Component.FrameWork
{
    public sealed class SqlHelper
    {
        // Methods
        private SqlHelper()
        {
        }

        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters != null) && (parameterValues != null))
            {
                if (commandParameters.Length != parameterValues.Length)
                {
                    throw new ArgumentException("Parameter count does not match Parameter Value count.");
                }
                int index = 0;
                int length = commandParameters.Length;
                while (index < length)
                {
                    commandParameters[index].Value = parameterValues[index];
                    index++;
                }
            }
        }

        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter parameter in commandParameters)
            {
                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
                command.Parameters.Add(parameter);
            }
        }

        public static SqlTransaction BeginTransaction(string con)
        {
            SqlConnection connection1 = new SqlConnection(con);
            connection1.Open();
            return connection1.BeginTransaction();
        }

        public static DataRow ExecuteDataRow(SqlConnection connection, CommandType commandType, string commandText)
        {
            DataTable table = ExecuteDataTable(connection, commandType, commandText);
            if ((table != null) && (table.Rows.Count != 0))
            {
                return table.Rows[0];
            }
            return null;
        }

        public static DataRow ExecuteDataRow(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            DataTable table = ExecuteDataTable(connection, commandType, commandText, parameters);
            if ((table != null) && (table.Rows.Count != 0))
            {
                return table.Rows[0];
            }
            return null;
        }

        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, null);
        }

        public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, null);
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, null);
        }

        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
        }

        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, connection, null, commandType, commandText, commandParameters);
            DataSet dataSet = new DataSet();
            new SqlDataAdapter(command).Fill(dataSet);
            command.Parameters.Clear();
            return dataSet;
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters);
            DataSet dataSet = new DataSet();
            new SqlDataAdapter(command).Fill(dataSet);
            command.Parameters.Clear();
            return dataSet;
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }

        public static DataSet ExecuteDataset2(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, connection, null, commandType, commandText, commandParameters);
            command.CommandTimeout = 0;
            DataSet dataSet = new DataSet();
            new SqlDataAdapter(command).Fill(dataSet);
            command.Parameters.Clear();
            return dataSet;
        }

        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, commandType, commandText);
            return PerformanceWatcher.Watch<DataTable>(commandText, delegate {
                DataTable dataTable = new DataTable();
                new SqlDataAdapter(cmd).Fill(dataTable);
                cmd.Parameters.Clear();
                return dataTable;
            });
        }

        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, commandType, commandText, parameters);
            return PerformanceWatcher.Watch<DataTable>(commandText, delegate {
                DataTable dataTable = new DataTable();
                new SqlDataAdapter(cmd).Fill(dataTable);
                cmd.Parameters.Clear();
                return dataTable;
            });
        }

        public static DataTable ExecuteDataTable2(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, commandType, commandText, parameters);
            return PerformanceWatcher.Watch<DataTable>(commandText, delegate {
                SqlDataAdapter adapter1 = new SqlDataAdapter(cmd)
                {
                    SelectCommand = { CommandTimeout = 0 }
                };
                DataTable dataTable = new DataTable();
                adapter1.Fill(dataTable);
                cmd.Parameters.Clear();
                return dataTable;
            });
        }

        public static DataTable ExecuteDataTable2(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
            cmd.CommandTimeout = 0;
            return PerformanceWatcher.Watch<DataTable>(commandText, delegate {
                SqlDataAdapter adapter1 = new SqlDataAdapter(cmd)
                {
                    SelectCommand = { CommandTimeout = 0 }
                };
                DataTable dataTable = new DataTable();
                adapter1.Fill(dataTable);
                cmd.Parameters.Clear();
                return dataTable;
            });
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, null);
        }

        public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, null);
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, null);
        }

        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
        }

        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
            cmd.Parameters.Clear();
            return PerformanceWatcher.Watch<int>(commandText, () => cmd.ExecuteNonQuery());
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters);
            int num = command.ExecuteNonQuery();
            command.Parameters.Clear();
            return num;
        }

        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        public static int ExecuteNonQueryV2(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, connection, null, commandType, commandText, commandParameters);
            command.CommandTimeout = 0;
            int num = command.ExecuteNonQuery();
            command.Parameters.Clear();
            return num;
        }

        public static int ExecuteNonQueryV2(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters);
            command.CommandTimeout = 0;
            int num = command.ExecuteNonQuery();
            command.Parameters.Clear();
            return num;
        }

        public static int ExecuteOutPutNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, connection, null, commandType, commandText, commandParameters);
            return command.ExecuteNonQuery();
        }

        public static DataSet ExecuteOutputParamDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, connection, null, commandType, commandText, commandParameters);
            DataSet dataSet = new DataSet();
            new SqlDataAdapter(command).Fill(dataSet);
            return dataSet;
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, null);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteReader(connection, CommandType.StoredProcedure, spName);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(transaction, commandType, commandText, null);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            SqlDataReader reader;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);
            if (connectionOwnership == SqlConnectionOwnership.External)
            {
                reader = PerformanceWatcher.Watch<SqlDataReader>(commandText, () => cmd.ExecuteReader());
            }
            else
            {
                reader = PerformanceWatcher.Watch<SqlDataReader>(commandText, () => cmd.ExecuteReader(CommandBehavior.CloseConnection));
            }
            cmd.Parameters.Clear();
            return reader;
        }

        public static SqlDataReader ExecuteReaderV2(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReaderV2(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        private static SqlDataReader ExecuteReaderV2(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            SqlDataReader reader;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);
            cmd.CommandTimeout = 0;
            if (connectionOwnership == SqlConnectionOwnership.External)
            {
                reader = PerformanceWatcher.Watch<SqlDataReader>(commandText, () => cmd.ExecuteReader());
            }
            else
            {
                reader = PerformanceWatcher.Watch<SqlDataReader>(commandText, () => cmd.ExecuteReader(CommandBehavior.CloseConnection));
            }
            cmd.Parameters.Clear();
            return reader;
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, null);
        }

        public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
        }

        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteScalar(transaction, commandType, commandText, null);
        }

        public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, null);
        }

        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
        }

        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
            cmd.Parameters.Clear();
            return PerformanceWatcher.Watch<object>(commandText, () => cmd.ExecuteScalar());
        }

        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
            cmd.Parameters.Clear();
            return PerformanceWatcher.Watch<object>(commandText, () => cmd.ExecuteScalar());
        }

        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteXmlReader(connection, commandType, commandText, null);
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
        }

        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteXmlReader(transaction, commandType, commandText, null);
        }

        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length != 0))
            {
                SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
                AssignParameterValues(spParameterSet, parameterValues);
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
        }

        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, connection, null, commandType, commandText, commandParameters);
            XmlReader reader = command.ExecuteXmlReader();
            command.Parameters.Clear();
            return reader;
        }

        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters);
            XmlReader reader = command.ExecuteXmlReader();
            command.Parameters.Clear();
            return reader;
        }

        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
        }

        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
        }

        public static SqlDataReader ProcOutputExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlDataReader reader;
            try
            {
                reader = ProcOutputExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                connection.Close();
                throw;
            }
            return reader;
        }

        public static SqlDataReader ProcOutputExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlDataReader reader;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            try
            {
                reader = ProcOutputExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                connection.Close();
                throw;
            }
            return reader;
        }

        private static SqlDataReader ProcOutputExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters);
            if (connectionOwnership == SqlConnectionOwnership.External)
            {
                return command.ExecuteReader();
            }
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public static SqlDataReader ProcOutputExecuteReaderV2(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ProcOutputExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
        }

        // Nested Types
        private enum SqlConnectionOwnership
        {
            Internal,
            External
        }
    }
}




