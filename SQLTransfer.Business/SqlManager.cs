using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using SQLTransfer.DataAccess;
using SQLTransfer.DataAccess.Entity;

namespace SQLTransfer.Business
{
    public class SqlManager
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(nameof(SqlManager));

        public List<ExecuteResult>  InsertDataIntoOracle()
        {
            var result = new List<ExecuteResult>();
            try
            {
                var ctTableNames = DalExecuteSql.GetTableNameOfCt();
                //var ctTableNames = new List<string> { "JXC_CGQG_HEAD" };
                //最大并行度
                var o = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                Parallel.ForEach(ctTableNames, o, (tableName) =>
                //foreach (var tableName in ctTableNames)
                {
                    if (!string.IsNullOrWhiteSpace(tableName))
                    {
                        var item = new ExecuteResult {Id = ctTableNames.IndexOf(tableName), TableName = tableName.ToUpper(), Result = "Success"};
                        if (DalExecuteSql.IsExistOracleTable(tableName)) //判断Oracle中是否存在该表
                        {
                            DalExecuteSql.TrunCateOracleTable(tableName); //清空表内数据
                        }
                        else
                        {
                            item.Result = $"{tableName.ToUpper()}表不存在";
                            result.Add(item);
                            return;
                            //continue;
                        }
                        var ctData = DalExecuteSql.GetCtDataByTableName(tableName);
                        if (ctData != null && ctData.Tables[0].Rows.Count > 0)
                        {
                            var columnInfo = GetTableColumnNameAndDataType(ctData);
                            Database db = DatabaseFactory.CreateDatabase("DATA1"); //链接到Oracle
                            using (DbConnection connection = db.CreateConnection())
                            {
                                connection.Open();
                                using (var transaction = connection.BeginTransaction())
                                {
                                    try
                                    {
                                        foreach (DataRow dr in ctData.Tables[0].Rows)
                                        {
                                            //try
                                            //{
                                            var sql = GenerateSql(tableName,
                                                columnInfo.Select(p => p.Item1).AsEnumerable()); //生成sql
                                            if (!string.IsNullOrWhiteSpace(sql))
                                            {
                                                DbCommand cmd = db.GetSqlStringCommand(sql);
                                                AppendDbParameters(db, cmd, dr, columnInfo); //绑定参数
                                                db.ExecuteNonQuery(cmd, transaction);
                                            }
                                            //}
                                            //catch (Exception ex)
                                            //{
                                            //    transaction.Rollback();
                                            //    _logger.Error($"{tableName}插入数据出错,当前行{JsonConvert.SerializeObject(dr)}",
                                            //        ex);
                                            //    isExecuteSuccess = false;
                                            //    break;
                                            //}
                                        }
                                        transaction.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        _logger.Error($"{tableName}插入数据出错", ex);
                                        item.Result = "Fail";
                                        item.Error = ex.Message;
                                    }
                                }
                            }
                        }
                        else
                        {
                            item.Result = "Sql Server表中暂无数据";
                        }
                        result.Add(item);
                    }
                    });
                //}
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
                        db.AddInParameter(cmd, $":{col.Item1}", DbType.Guid, dr[col.Item1]);
                        break;
                    default:
                        db.AddInParameter(cmd, $":{col.Item1}", DbType.String, dr[col.Item1]);
                        break;
                }
            }
        }
    }
}
