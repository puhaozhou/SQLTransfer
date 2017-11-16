using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using ErowSqlTransfer.Business;
using ErowSqlTransfer.DataAccess.Entity;
using System.Threading.Tasks;

namespace ErowSqlTransfer
{
    public partial class Form1 : Form
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(nameof(Form1));
        public readonly  List<string> TableNamesList = new List<string>{ "adm_car_serviceteam" };
        public readonly List<string> SequenceNamesList = new List<string>{"SEQ_ADM_CAR"};
        public readonly bool IsSyncAllTables = true;
        public readonly bool IsSyncAllSequences = true;
        public readonly bool IsUseSequencesFilter = true;

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void BtnSyncData_Click(object sender, EventArgs e)
        {
            BtnSyncData.Enabled = false;
            BtnSyncSequence.Enabled = false;
            SyncDjNo.Enabled = false;
            TotalNum.Text = "0";
            NumTransfered.Text = "0";
            textBox1.Clear();
            Thread thread = new Thread(SyncTables);
            thread.Start();
        }

        private void BtnSyncSequence_Click(object sender, EventArgs e)
        {
            BtnSyncData.Enabled = false;
            BtnSyncSequence.Enabled = false;
            SyncDjNo.Enabled = false;
            TotalNum.Text = "0";
            NumTransfered.Text = "0";
            textBox1.Clear();
            Thread thread = new Thread(SyncSequences);
            thread.Start();
        }

        private void SyncDjNo_Click(object sender, EventArgs e)
        {
            BtnSyncData.Enabled = false;
            BtnSyncSequence.Enabled = false;
            SyncDjNo.Enabled = false;
            TotalNum.Text = "0";
            NumTransfered.Text = "0";
            textBox1.Clear();
            Thread thread = new Thread(SyncDjNos);
            thread.Start();
        }

        public void SyncTables()
        {
            var manager = new SyncTableManager();
            var result = new SyncResult();
            try
            {
                var tableNames = IsSyncAllTables ? manager.GetTableNames() : TableNamesList;
                TotalNum.Text = tableNames.Where(p => !string.IsNullOrEmpty(p)).ToList().Count.ToString();

                progressBar1.Minimum = 0;
                progressBar1.Maximum = tableNames.Count;
                progressBar1.Value = 0;
                //最大并行度
                var o = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                Parallel.ForEach(tableNames, o, (tableName) =>
                {
                    if (string.IsNullOrWhiteSpace(tableName)) return;
                    var item = new TableResult
                    {
                        Id = tableNames.IndexOf(tableName)
                    };
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    manager.TransferTableData(tableName, ref item);
                    result.TableResults.Add(item);
                    stopwatch.Stop();
                    lock (this)
                    {
                        progressBar1.Value++;
                        textBox1.AppendText($"已完成表：{tableName.ToUpper()}，操作结果：{item.SyncResult},用时：{stopwatch.Elapsed.TotalSeconds:0.00} s\r\n");
                        NumTransfered.Text = result.TableResults.Count.ToString();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            result.TableResults = result.TableResults.OrderBy(p => p.Id).ToList();
            CommonHelper.EditReportFile(result);
            MessageBox.Show(@"同步完成，请查看同步报告");
            BtnSyncData.Enabled = true;
            BtnSyncSequence.Enabled = true;
            SyncDjNo.Enabled = true;
        }

        public void SyncSequences()
        {
            var manager = new SyncSequencesManager();
            var result = new SyncResult();
            try
            {
                Func<string, bool> seqFilter;
                if (IsUseSequencesFilter)
                {
                    seqFilter = (name) =>
                    {
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            return false;
                        }
                        var tableName = name.ToUpper().Replace("SEQ_", "");
                        var tabNamePrefix = tableName.Split('_')[0];
                        return !tabNamePrefix.ToUpper().Equals("T");
                    };
                }
                else
                {
                    seqFilter = (name) => true;
                }
                var sequenceNames = IsSyncAllSequences ? manager.GetSequenceNames() : SequenceNamesList;
                TotalNum.Text = sequenceNames.Where(seqFilter).ToList().Count.ToString();
                progressBar1.Minimum = 0;
                progressBar1.Maximum = sequenceNames.Where(seqFilter).ToList().Count;
                progressBar1.Value = 0;               
                //最大并行度
                var o = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                Parallel.ForEach(sequenceNames.Where(seqFilter), o, (seqName) =>
                {
                    if (string.IsNullOrWhiteSpace(seqName)) return;
                    var item = new SequenceResult
                    {
                        Id = sequenceNames.IndexOf(seqName)
                    };
                    manager.UpdateSequence(seqName, ref item);
                    result.SequenceResults.Add(item);
                    lock (this)
                    {
                        progressBar1.Value++;
                        textBox1.AppendText($"已完成队列：{seqName.ToUpper()}，操作结果：{item.SyncResult}\r\n");
                        NumTransfered.Text = result.SequenceResults.Count.ToString();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            result.SequenceResults = result.SequenceResults.OrderBy(p => p.Id).ToList();
            CommonHelper.EditReportFile(result);
            MessageBox.Show(@"同步完成，请查看同步报告");
            BtnSyncData.Enabled = true;
            BtnSyncSequence.Enabled = true;
            SyncDjNo.Enabled = true;
        }

        public void SyncDjNos()
        {
            var manager = new SyncDjNoManager();
            var result = new SyncResult();
            try
            {
                var appfns = manager.GetAppFnInfo();
                TotalNum.Text = appfns.Where(p => !string.IsNullOrEmpty(p.FnCode)).ToList().Count.ToString();

                progressBar1.Minimum = 0;
                progressBar1.Maximum = appfns.Count;
                progressBar1.Value = 0;
                //最大并行度
                var o = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                Parallel.ForEach(appfns, o, (appfn) =>
                {
                    if (string.IsNullOrWhiteSpace(appfn.FnCode)) return;
                    var item = new DjNoResult
                    {
                        Id = appfns.IndexOf(appfn)
                    };
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    manager.SyncDjNoValue(appfn, ref item);
                    result.DjNoResults.Add(item);
                    stopwatch.Stop();
                    lock (this)
                    {
                        progressBar1.Value++;
                        textBox1.AppendText($"已完成表：{appfn.TableName.ToUpper()}，操作结果：{item.SyncResult},用时：{stopwatch.Elapsed.TotalSeconds:0.00} s\r\n");
                        NumTransfered.Text = result.DjNoResults.Count.ToString();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            result.DjNoResults = result.DjNoResults.OrderBy(p => p.Id).ToList();
            CommonHelper.EditReportFile(result);
            MessageBox.Show(@"同步完成，请查看同步报告");
            BtnSyncData.Enabled = true;
            BtnSyncSequence.Enabled = true;
            SyncDjNo.Enabled = true;
        }        
    }
}
