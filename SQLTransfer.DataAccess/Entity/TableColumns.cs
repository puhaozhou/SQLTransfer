using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErowSqlTransfer.DataAccess.Entity
{
    public class TableColumns
    {
        public TableColumns()
        {
            TableInfos = new List<TableInfo>();
        }

        public string TableName { get; set; }

        public List<TableInfo> TableInfos { get; set; }
    }

    public class TableInfo
    {
        public string ColumnName { get; set; }

        public string DataType { get; set; }
    }
}
