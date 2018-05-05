using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using SQLTransfer.Business;
using SQLTransfer.DataAccess.Entity;

namespace SQLTransfer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var manager = new SqlManager();
            var result = manager.InsertDataIntoOracle().OrderBy(p=>p.Id).ToList();
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
            MessageBox.Show(@"同步完成，请查看同步报告");
        }
    }
}
