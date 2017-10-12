using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ErowSqlTransfer.DataAccess.Entity;

namespace ErowSqlTransfer
{
    public class CommonHelper
    {
        public static void EditReportFile(SyncResult result)
        {
            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SyncResult));
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
