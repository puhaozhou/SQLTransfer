using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErowSqlTransfer.DataAccess.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ErowSqlTransfer.DataAccess
{
    public class DalSyncSequences
    {
        public static List<string> GetSequenceNames()
        {
            var result = new List<string>();
            var sql = @"SELECT DISTINCT SEQUENCE_NAME FROM USER_SEQUENCES";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            var data = db.ExecuteDataSet(cmd)?.Tables[0];
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow dr in data.Rows)
                {
                    result.Add(dr["SEQUENCE_NAME"].ToString());
                }
            }
            return result;
        }

        public static bool IsExistSequence(string seqName)
        {
            var result = false;
            var sql = @"SELECT COUNT(*) FROM USER_SEQUENCES WHERE SEQUENCE_NAME = :pSeqName";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, ":pSeqName", DbType.String, seqName);
            var row = db.ExecuteScalar(cmd);
            if (row != null)
            {
                result = Convert.ToInt32(row) > 0;
            }
            return result;
        }

        public static string GetPrimaryKey(string tableName)
        {
            var result = string.Empty;
            var sql = @"select ucs.column_name from user_CONS_columns ucs,
                        user_constraints uc
                        where ucs.constraint_name = uc.constraint_name
                        and ucs.table_name = :pTableName 
                        and uc.CONSTRAINT_TYPE = 'P'";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            db.AddInParameter(cmd, ":pTableName", DbType.String, tableName);
            var row = db.ExecuteScalar(cmd);
            if (row != null && !row.Equals(DBNull.Value))
            {
                result = row.ToString();
            }
            return result;
        }

        public static int GetMaxPrimaryKeyValue(string tableName, string columnName)
        {
            var result = 0;
            var sql = $"SELECT MAX({columnName.ToUpper()}) from {tableName.ToUpper()}";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            var row = db.ExecuteScalar(cmd);
            if (row != null)
            {
                result = Convert.ToInt32(row);
            }
            return result;
        }

        public static int? GetSequenceLastNumber(string seqName)
        {
            int? result = null;
            var sql = $"SELECT LAST_NUMBER FROM USER_SEQUENCES WHERE SEQUENCE_NAME = '{seqName}'";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(sql);
            var data = db.ExecuteScalar(cmd);
            if (data != null)
            {
                result = Convert.ToInt32(data);
            }
            return result;
        }

        public static void UpdateSequence(string seqName, int idValue)
        {
            //var sqlTemplete = @"ALTER SEQUENCE {0} Increment By {1};
            //                    Select {0}.NextVal From Dual;
            //                    Alter Sequence {0} Increment By 1;
            //                   ";
            var dropTemplete = $"DROP sequence {seqName.ToUpper()}";
            var createTemplete = @"create sequence {0}
                                minvalue 1
                                maxvalue 999999999999999999999999
                                start with {1}
                                increment by 1
                                cache 20";
            Database db = DatabaseFactory.CreateDatabase(Constant.OracleConnectionName);
            DbCommand cmd = db.GetSqlStringCommand(dropTemplete);
            db.ExecuteNonQuery(cmd);
            cmd = db.GetSqlStringCommand(string.Format(createTemplete, seqName.ToUpper(), idValue + 1));
            db.ExecuteNonQuery(cmd);
        }
    }
}
