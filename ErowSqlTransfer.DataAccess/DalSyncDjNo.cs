using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ErowSqlTransfer.DataAccess.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ErowSqlTransfer.DataAccess
{
    public class DalSyncDjNo
    {
        public static List<AppFnModel> GetAppFnInfo()
        {
            var result = new List<AppFnModel>();
            var sql = @"select * from app_fn where fn_code like 'adm_%' and dj_no_name is not null order by fn_code";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleBizConnName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            var dt = db.ExecuteDataSet(cmd)?.Tables[0];
            if (dt == null || dt.Rows.Count <= 0) return result;
            foreach (DataRow dr in dt.Rows)
            {
                AppFnModel item = new AppFnModel
                {
                    FnCode = dr["FN_CODE"].ToString(),
                    CurrentNumber = dr["CURRENT_NUM"] != DBNull.Value ? Convert.ToInt32(dr["CURRENT_NUM"]) : 0,
                    DjNoName = dr["DJ_NO_NAME"].ToString(),
                    TableName = dr["TABLE_NAME"].ToString()
                };
                result.Add(item);
            }
            return result;
        }

        public static int? GetCurrentNumber(string tableName, string columnName)
        {
            int? result = null;
            var sql = $"select max({columnName}) from {tableName}";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleBizConnName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            var data = db.ExecuteScalar(cmd);
            if (data != null && !data.Equals(DBNull.Value))
            {
                data = Regex.Replace(data.ToString(), @"[^0-9]+", "");
                result = Convert.ToInt32(data);
            }
            return result;
        }

        public static bool UpdateAppFnCode(int currentNum, string fnCode)
        {
            var sql = @"update app_fn set current_num = :pCurrentNum where fn_code = :pFnCode";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleBizConnName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, ":pCurrentNum", DbType.String, currentNum);
            db.AddInParameter(cmd, ":pFnCode", DbType.String, fnCode);
            return db.ExecuteNonQuery(cmd) > 0;
        }
    }
}
