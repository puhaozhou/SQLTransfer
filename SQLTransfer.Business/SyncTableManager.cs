using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using SQLTransfer.DataAccess;
using SQLTransfer.DataAccess.Entity;

namespace SQLTransfer.Business
{
    public class SyncTableManager
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(nameof(SyncTableManager));

        private readonly List<string> OracleSpecialColumnName = new List<string>{ "APPLICANT_USER_ID" };

        public bool IsUseOracleColumn { get; set; }

        public List<TableColumns> OracleTablesInfo { get; set; }

        public void TransferTableData(string tableName, ref TableResult item)
        {
            try
            {
                item.TableName = tableName.ToUpper();
                item.SyncResult = "success";
                if (DalSyncTable.IsExistOracleTable(tableName)) //判断Oracle中是否存在该表
                {
                    DalSyncTable.TrunCateOracleTable(tableName); //清空表内数据
                }
                else
                {
                    item.SyncResult = $"表{tableName.ToUpper()}在Oracle中不存在";
                    return;
                }
                var ctData = DalSyncTable.GetCtDataByTableName(tableName);
                if (ctData != null && ctData.Tables[0].Rows.Count > 0)
                {
                    var mssqlColumnInfo = GetTableColumnNameAndDataType(ctData);
                    var oracleColumnInfo = IsUseOracleColumn ? GetOracleTablesInfo(tableName, mssqlColumnInfo) : null;
                    var columnInfo = IsUseOracleColumn ? oracleColumnInfo : mssqlColumnInfo;
                    Database db = DatabaseFactory.CreateDatabase("OracleConn"); //链接到Oracle
                    using (DbConnection connection = db.CreateConnection())
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                foreach (DataRow dr in ctData.Tables[0].Rows)
                                {
                                    var sql = GenerateSql(tableName,
                                        columnInfo.Select(p => p.Item1).AsEnumerable()); //生成sql
                                    if (!string.IsNullOrWhiteSpace(sql))
                                    {
                                        DbCommand cmd = db.GetSqlStringCommand(sql);
                                        AppendDbParameters(db, cmd, dr, columnInfo); //绑定参数
                                        db.ExecuteNonQuery(cmd, transaction);
                                    }
                                }
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                _logger.Error($"{tableName}插入数据出错", ex);
                                item.SyncResult = "Fail";
                                item.SyncError = ex.Message;
                            }
                        }
                        connection.Close();
                    }
                }
                else
                {
                    item.SyncResult = $"表{item.SyncError}在Sql Server中暂无数据";
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                item.SyncResult = "Fail";
                item.SyncError = ex.Message;
            }         
        }        

        public List<string> GetTableNames()
        {
            var result = new List<string>();
            try
            {
                result = DalSyncTable.GetTableNames();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return result;
        }        

        public List<Tuple<string,Type>> GetTableColumnNameAndDataType(DataSet data)
        {
            List<Tuple<string, Type>> result = new List<Tuple<string, Type>>();
            try
            {
                var columnCount = data.Tables[0].Columns.Count;
                for (var i = 0; i < columnCount; i++)
                {
                    var columnName = data.Tables[0].Columns[i].ColumnName;
                    var dataType = data.Tables[0].Columns[i].DataType;
                    result.Add(Tuple.Create(columnName, dataType));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return result;
        }

        public string GenerateSql(string tableName, IEnumerable<string> columnInfo)
        {
            string sql = string.Empty;
            try
            {
                var insertSqlTemplate = "insert into {0} ({1}) values ({2})";
                StringBuilder columnBuilder = new StringBuilder();
                StringBuilder valueBuilder = new StringBuilder();
                foreach (var col in columnInfo)
                {
                    columnBuilder.Append($"{col},");
                    valueBuilder.Append($":{col},");
                }
                columnBuilder.Remove(columnBuilder.Length - 1, 1);
                valueBuilder.Remove(valueBuilder.Length - 1, 1);
                sql = string.Format(insertSqlTemplate, tableName.ToUpper(), columnBuilder, valueBuilder);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return sql;
        }

        public void AppendDbParameters(Database db, DbCommand cmd, DataRow dr, List<Tuple<string, Type>> columnInfo)
        {
            foreach (var col in columnInfo)
            {
                switch (col.Item2.Name)
                {
                    case "Int32":
                        db.AddInParameter(cmd, $":{col.Item1}", DbType.String, dr[col.Item1]);
                        break;
                    case "DateTime":
                        db.AddInParameter(cmd, $":{col.Item1}", DbType.Date, dr[col.Item1]);
                        break;
                    case "Guid":
                        db.AddInParameter(cmd, $":{col.Item1}", DbType.String, dr[col.Item1]?.ToString().ToUpper());
                        break;
                    default:
                        db.AddInParameter(cmd, $":{col.Item1}", DbType.String, dr[col.Item1]);
                        break;
                }
            }
        }

        public List<TableColumns> GetAllOracleTablesInfo(string tableNames)
        {
            List<TableColumns> result = new List<TableColumns>();
            try
            {
                result = DalSyncTable.GetOracleTableColumns(tableNames);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 只取Oracle中有的字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="mssqlColumns"></param>
        /// <returns></returns>
        public List<Tuple<string, Type>> GetOracleTablesInfo(string tableName, List<Tuple<string, Type>> mssqlColumns)
        {
            IEnumerable<string> oracleColumns = new List<string>();
            var data = OracleTablesInfo.Where(p=>p.TableName.Equals(tableName.ToUpper())); //获取Oracle的表结构
            if (data.Any())
            {
                var columns = data.FirstOrDefault()?.TableInfos;
                if (columns != null && columns.Any())
                    oracleColumns = columns.Select(p => p.ColumnName.ToUpper());
            }
            var result = mssqlColumns.Where(p => oracleColumns.Contains(p.Item1.ToUpper())).ToList();
            return result;
        }
    }
}
