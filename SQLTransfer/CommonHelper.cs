using System.IO;
using System.Text;
using System.Xml.Serialization;
using SQLTransfer.DataAccess.Entity;

namespace SQLTransfer
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
