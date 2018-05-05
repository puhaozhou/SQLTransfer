using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SQLTransfer.DataAccess
{
    public class DalExecuteSql
    {
        public const string SqlConnectionName = "CT";
        public const string OracleConnectionName = "DATA1";

        /// <summary>
        /// 获取库ct_ct中ct和jxc的表名
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTableNameOfCt()
        {
            List<string> result = new List<string>();
            var sql = @"SELECT name FROM sysobjects WITH (NOLOCK) WHERE type = 'u'";
            Database db = DatabaseFactory.CreateDatabase(SqlConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            DataSet ds = db.ExecuteDataSet(cmd);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["name"].ToString().Substring(0, 2).ToUpper().Equals("CT") ||
                    dr["name"].ToString().Substring(0, 3).ToUpper().Equals("JXC"))
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
            if (tableName.Substring(0, 2).ToUpper().Equals("CT"))
            {
                tableName = "ct." + tableName;
            }
            if (tableName.Substring(0, 3).ToUpper().Equals("JXC"))
            {
                tableName = "jxc." + tableName;
            }
            var sql = $"SELECT * FROM {tableName} WITH (NOLOCK)";
            Database db = DatabaseFactory.CreateDatabase(SqlConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            var result = db.ExecuteDataSet(cmd);
            return result;
        }

        public static bool IsExistOracleTable(string tableName)
        {
            var result = false;
            var sql = $" SELECT COUNT(*) FROM USER_TABLES WHERE table_name = '{tableName.ToUpper()}'";
            Database db = DatabaseFactory.CreateDatabase(OracleConnectionName);
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
            Database db = DatabaseFactory.CreateDatabase(OracleConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            db.ExecuteNonQuery(cmd);
        }
    }
}
