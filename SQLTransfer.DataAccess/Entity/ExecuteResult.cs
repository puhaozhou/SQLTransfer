using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErowSqlTransfer.DataAccess.Entity
{
    public class ExecuteResult
    {
        public int Id { get; set; }
        public string TableName { get; set; }

        public string Result { get; set; }

        public string Error { get; set; }
    }    
}
