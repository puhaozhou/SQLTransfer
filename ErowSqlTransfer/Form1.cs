using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Linq;
using System.Threading;
using System.Data.Common;
using System.Windows.Forms;
using System.Xml.Serialization;
using ErowSqlTransfer.Business;
using ErowSqlTransfer.DataAccess.Entity;
using ErowSqlTransfer.DataAccess;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ErowSqlTransfer
{
    public partial class Form1 : Form
    {
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(nameof(Form1));

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;            
            Thread thread = new Thread(InsertDataIntoOracle);
            thread.Start();
        }

        public void InsertDataIntoOracle()
        {
            var manager = new SqlManager();
            var result = new List<ExecuteResult>();
            try
            {
                var ctTableNames = DalExecuteSql.GetTableNameOfCt();
                progressBar1.Minimum = 0;
                progressBar1.Maximum = ctTableNames.Count;
                progressBar1.Value = 0;
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
                            lock (this)
                            {
                                progressBar1.Value++;
                                textBox1.AppendText($"已完成表：{tableName.ToUpper()}，操作结果：{item.Result}\r\n");
                            }
                            return;
                        }
                        var ctData = DalExecuteSql.GetCtDataByTableName(tableName);
                        if (ctData != null && ctData.Tables[0].Rows.Count > 0)
                        {
                            var columnInfo = manager.GetTableColumnNameAndDataType(ctData);
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
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        _logger.Error($"{tableName}插入数据出错", ex);
                                        item.Result = "Fail";
                                        item.Error = ex.Message;
                                    }
                                }
                                connection.Close();
                            }
                        }
                        else
                        {
                            item.Result = "Sql Server表中暂无数据";
                        }
                        result.Add(item);
                        lock (this)
                        {
                            progressBar1.Value++;
                            textBox1.AppendText($"已完成表：{tableName.ToUpper()}，操作结果：{item.Result}\r\n");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            EditReportFile(result.OrderBy(p => p.Id).ToList());
            MessageBox.Show(@"同步完成，请查看同步报告");
            button1.Enabled = true;
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
