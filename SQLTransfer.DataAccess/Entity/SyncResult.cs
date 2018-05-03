using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErowSqlTransfer.DataAccess.Entity
{
    public class SyncResult
    {
        public SyncResult()
        {
            TableResults = new List<TableResult>();

            SequenceResults = new List<SequenceResult>();

            DjNoResults = new List<DjNoResult>();
        }

        public List<TableResult> TableResults { get; set; }

        public List<SequenceResult> SequenceResults { get; set; }

        public List<DjNoResult> DjNoResults { get; set; }
    }

    public class TableResult
    {
        public int Id { get; set; }
        public string TableName { get; set; }

        public string SyncResult { get; set; }

        public string SyncError { get; set; }
    }

    public class SequenceResult
    {
        public int Id { get; set; }
        public string SequenceName { get; set; }

        public string SyncResult { get; set; }

        public string SyncError { get; set; }

        public int BeforeSync { get; set; }

        public int AfterSync { get; set; }
    }

    public class DjNoResult
    {
        public int Id { get; set; }

        public string FnCode { get; set; }

        public string DjNoName { get; set; }

        public string SyncResult { get; set; }

        public string SyncError { get; set; }

        public int BeforeSync { get; set; }

        public int AfterSync { get; set; }
    }
}
