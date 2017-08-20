using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ErowSqlTransfer.Business
{
    public class SqlManager
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(nameof(SqlManager));

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
