using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using ErowSqlTransfer.DataAccess.Entity;

namespace ErowSqlTransfer.DataAccess
{
    public class DalSyncTable
    {
        public static readonly string[] Heads = {"CT","JXC","HR"};

        /// <summary>
        /// 获取库ct_ct中ct和jxc的表名
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTableNames()
        {
            List<string> result = new List<string>();
            var sql = @"SELECT name FROM sysobjects WITH (NOLOCK) WHERE type = 'u'";
            Database db = DatabaseFactory.CreateDatabase(Constant.SqlConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            DataSet ds = db.ExecuteDataSet(cmd);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (Heads != null && Heads.Any())
                {
                    foreach (var head in Heads)
                    {
                        if (dr["name"].ToString().Length >= head.Length &&
                            dr["name"].ToString().Substring(0, head.Length).ToUpper().Equals(head))
                        {
                            result.Add(dr["name"].ToString());
                        }
                    }
                }
                else
                {
                    result.Add(dr["name"].ToString());
                }
            }
            return result.OrderBy(p=>p).ToList();
        }

        /// <summary>
        /// 获取每张表中的数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataSet GetCtDataByTableName(string tableName)
        {
            if (Heads != null && Heads.Any())
            {
                foreach (var head in Heads)
                {
                    if (tableName.Length >= head.Length &&
                        tableName.Substring(0, head.Length).ToUpper().Equals(head))
                    {
                        tableName = head.ToLower() + "." + tableName;
                    }
                }
            }
            else
            {
                tableName = tableName.Split('_')[0] + "." + tableName;//表头要加上
            }
            var sql = $"SELECT * FROM {tableName} WITH (NOLOCK)";
            Database db = DatabaseFactory.CreateDatabase(Constant.SqlConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            var result = db.ExecuteDataSet(cmd);
            return result;
        }

        public static bool IsExistOracleTable(string tableName)
        {
            var result = false;
            var sql = $" SELECT COUNT(*) FROM USER_TABLES WHERE table_name = '{tableName.ToUpper()}'";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            var row = db.ExecuteScalar(cmd);
            if (row != null)
            {
                result = Convert.ToInt32(row) > 0;
            }
            return result;
        }

        public static void TrunCateOracleTable(string tableName)
        {
            var sql = $"TRUNCATE TABLE {tableName.ToUpper()}";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            db.ExecuteNonQuery(cmd);
        }

        public static List<TableColumns> GetOracleTableColumns(string tableNames)
        {
            var result = new List<TableColumns>();
            var sql = @"SELECT column_name, 
                               data_type,
                               table_name
                        FROM user_tab_columns utc,
                        Table(SYS.ODCIVARCHAR2LIST(:pTableNames,',')) tmp
                        where utc.table_name = tmp.column_value";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd,":pTableNames",DbType.String,tableNames.ToUpper());
            var dt = db.ExecuteDataSet(cmd)?.Tables[0] ?? new DataTable();
            if (dt.Rows.Count.Equals(0))
            {
                return result;
            }
            var data = dt.AsEnumerable().GroupBy(p => p["table_name"].ToString()).ToDictionary(o=>o.Key, o=>o.Select(t=>t));
            foreach (var item in data)
            {
                var list = new TableColumns();
                list.TableName = item.Key;
                foreach (var value in item.Value )
                {
                    var tableInfo = new TableInfo
                    {
                        ColumnName = value["column_name"].ToString(),
                        DataType = value["data_type"].ToString()
                    };
                    list.TableInfos.Add(tableInfo);
                }
                result.Add(list);
            }
            return result;
        }
    }
}
