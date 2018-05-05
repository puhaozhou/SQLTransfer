using System;
using System.Collections.Generic;
using SQLTransfer.DataAccess;
using SQLTransfer.DataAccess.Entity;

namespace SQLTransfer.Business
{
    public class SyncSequencesManager
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(nameof(SyncSequencesManager));

        public List<string> GetSequenceNames()
        {
            var result = new List<string>();
            try
            {
                result = DalSyncSequences.GetSequenceNames();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return result;
        }

        public void UpdateSequence(string seqName, ref SequenceResult item)
        {
            if (string.IsNullOrWhiteSpace(seqName)) return;
            var tableName = seqName.ToUpper().Replace("SEQ_", "");
            //var tabNamePrefix = tableName.Split('_')[0];
            //if (string.IsNullOrWhiteSpace(tabNamePrefix) || tabNamePrefix.Equals("T")) return;
            //var seqName = $"SEQ_{tableName.ToUpper()}";
            try
            {
                item.SequenceName = seqName;
                //if (!DalExecuteSql.IsExistSequence(seqName))
                //{
                //    item.SquenceResult = $"队列{seqName}不存在";
                //    return;
                //}
                var primaryKey = DalSyncSequences.GetPrimaryKey(tableName);
                if (string.IsNullOrWhiteSpace(primaryKey))
                {
                    item.SyncResult = $"表{tableName}中未设置主键";
                    item.SyncResult = "fail";
                    return;
                }
                var maxKeyValue = DalSyncSequences.GetMaxPrimaryKeyValue(tableName, primaryKey);
                if (maxKeyValue.Equals(0))
                {
                    item.SyncResult = $"表{tableName}中主键的值有误";
                    item.SyncResult = "fail";
                    return;
                }
                var seqLastNum = DalSyncSequences.GetSequenceLastNumber(seqName);
                if (seqLastNum != null)
                {
                    item.BeforeSync = (int)seqLastNum;
                }
                //var idValue = maxKeyValue - seqLastNum;
                DalSyncSequences.UpdateSequence(seqName, maxKeyValue);
                item.SyncResult = "success";
                item.AfterSync = maxKeyValue;
            }
            catch (Exception ex)
            {
                _logger.Error($"{seqName}同步出错", ex);
                item.SyncResult = "fail";
                item.SyncError = ex.Message;
            }
        }
    }
}
