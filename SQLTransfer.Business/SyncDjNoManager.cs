using System;
using System.Collections.Generic;
using SQLTransfer.DataAccess;
using SQLTransfer.DataAccess.Entity;

namespace SQLTransfer.Business
{
    public class SyncDjNoManager
    {
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(nameof(SyncDjNoManager));

        public List<AppFnModel> GetAppFnInfo()
        {
            var result = new List<AppFnModel>();
            try
            {
                result = DalSyncDjNo.GetAppFnInfo();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return result;
        }

        public void SyncDjNoValue(AppFnModel appfn, ref DjNoResult item)
        {
            try
            {
                item.FnCode = appfn.FnCode;
                var currentNum = DalSyncDjNo.GetCurrentNumber(appfn.TableName, appfn.DjNoName);
                if (currentNum == null)
                {
                    item.SyncResult = $"表{appfn.TableName}中可能没有数据，或查询数据出错";
                    item.SyncResult = "fail";
                    return;
                }
                item.BeforeSync = appfn.CurrentNumber;
                var result = DalSyncDjNo.UpdateAppFnCode(Convert.ToInt32(currentNum), appfn.FnCode);
                item.SyncResult = result ? "success" : "fail";
                item.AfterSync = Convert.ToInt32(currentNum);
            }
            catch (Exception ex)
            {
                _logger.Error($"{appfn.FnCode}同步出错", ex);
                item.SyncResult = "fail";
                item.SyncError = ex.Message;
            }
        }
    }
}
