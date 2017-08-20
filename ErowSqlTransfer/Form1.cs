using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using ErowSqlTransfer.Business;
using ErowSqlTransfer.DataAccess.Entity;

namespace ErowSqlTransfer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(delegate { this.InsertDataIntoOracle(); }));
            thread.Start();
        }      
    }
}
