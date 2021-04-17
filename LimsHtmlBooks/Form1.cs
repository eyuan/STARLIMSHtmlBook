using LimsHtmlBooks.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LimsHtmlBooks
{
    public partial class FmMain : Form
    {
        string bookname = "";
        public FmMain()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            bookname = cboBookName.Text;
            string sql = "SELECT COUNT(1) FROM LIMSHTMLBOOKS WHERE BOOKNAME = '"+ bookname + "' ";
            int count = (int)SQLHelper.ExecuteScalar(sql);
            pb.Maximum = count;
            bgw.RunWorkerAsync();
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        { 
            string sql = "SELECT FILEPATH, FILECONTENT FROM LIMSHTMLBOOKS WHERE BOOKNAME = '"+ bookname + "' ";
            DataTable dataTable = SQLHelper.GetDataTable(sql);
            int progressValue = 0;
            foreach(DataRow dataRow in dataTable.Rows)
            {
                string FILEPATH = dataRow["FILEPATH"].ToString();//文件目录
                FILEPATH = bookname + '/' + FILEPATH;
                byte[] FILECONTENT = (byte[])dataRow["FILECONTENT"];//文件内容
                if (FILEPATH.IndexOf('/') != -1)//子目录文件，则需要创建目录（文件夹）
                {
                    FileInfo fileInfo = new FileInfo(FILEPATH);
                    DirectoryInfo directoryInfo = fileInfo.Directory;
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                }
                //写入文件
                if (FILECONTENT.Length > 0)
                {
                    FileStream fs = File.Create(FILEPATH, FILECONTENT.Length);
                    fs.Write(FILECONTENT, 0, FILECONTENT.Length);
                    fs.Close();
                } 
                // 
                bgw.ReportProgress(++progressValue);
            }

        }

        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pb.Value = e.ProgressPercentage;
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("导出完成");
        }

        private void FmMain_Load(object sender, EventArgs e)
        {
            string sql = "SELECT distinct BOOKNAME FROM LIMSHTMLBOOKS  ";
            DataTable dataTable = SQLHelper.GetDataTable(sql);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                string bookname = dataRow[0].ToString();
                cboBookName.Items.Add(bookname);
            }
        }
    }
}
