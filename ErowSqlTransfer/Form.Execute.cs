using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using ErowSqlTransfer.Business;
using ErowSqlTransfer.DataAccess;
using ErowSqlTransfer.DataAccess.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ErowSqlTransfer
{
    partial class Form1
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(nameof(SqlManager));
        public int CompleteTableNum = 0;

        public void InsertDataIntoOracle()
        {
            var manager = new SqlManager();
            var result = new List<ExecuteResult>();
            try
            {
                var ctTableNames = DalExecuteSql.GetTableNameOfCt();
                //最大并行度
                var o = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                Parallel.ForEach(ctTableNames, o, (tableName) =>
                {
                    if (!string.IsNullOrWhiteSpace(tableName))
                    {
                        var item = new ExecuteResult
                        {
                            Id = ctTableNames.IndexOf(tableName),
                            TableName = tableName.ToUpper(),
                            Result = "Success"
                        };
                        if (DalExecuteSql.IsExistOracleTable(tableName)) //判断Oracle中是否存在该表
                        {
                            DalExecuteSql.TrunCateOracleTable(tableName); //清空表内数据
                        }
                        else
                        {
                            item.Result = $"{tableName.ToUpper()}表不存在";
                            result.Add(item);
                            return;
                        }
                        var ctData = DalExecuteSql.GetCtDataByTableName(tableName);
                        if (ctData != null && ctData.Tables[0].Rows.Count > 0)
                        {
                            var columnInfo = manager.GetTableColumnNameAndDataType(ctData);
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
                                            var sql = manager.GenerateSql(tableName,
                                                columnInfo.Select(p => p.Item1).AsEnumerable()); //生成sql
                                            if (!string.IsNullOrWhiteSpace(sql))
                                            {
                                                DbCommand cmd = db.GetSqlStringCommand(sql);
                                                manager.AppendDbParameters(db, cmd, dr, columnInfo); //绑定参数
                                                db.ExecuteNonQuery(cmd, transaction);
                                            }
                                        }
                                        transaction.Commit();
                                        CompleteTableNum++;
                                        this.progressBar1.Value = Convert.ToInt32(CompleteTableNum);
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
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            //return result;
            EditReportFile(result.OrderBy(p => p.Id).ToList());
            MessageBox.Show(@"同步完成，请查看同步报告");
        }

        public void EditReportFile(List<ExecuteResult> result)
        {
            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExecuteResult>));
                xmlSerializer.Serialize(stringWriter, result);
                FileStream fs = new FileStream("report.xml", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(stringWriter.ToString());
                sw.Close();
                fs.Close();
            }
        }
    }

    
}
