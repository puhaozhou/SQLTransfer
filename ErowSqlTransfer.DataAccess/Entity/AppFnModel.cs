using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErowSqlTransfer.DataAccess.Entity
{
    public class AppFnModel
    {
        public string FnCode { get; set; }

        public string TableName { get; set; }

        public int CurrentNumber { get; set; }

        public string DjNoName { get; set; }
    }
}
