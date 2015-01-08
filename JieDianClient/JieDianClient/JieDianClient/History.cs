using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using web.CData;
using System.IO;


namespace web
{
    public partial class History : Form
    {
        private DataSet ds = new DataSet();

        public History()
        {
            InitializeComponent();
            
            ds = UserDataOperation.getUserAll(Login.username);
            string ismanager = ds.Tables[0].Rows[0]["ismanager"].ToString();

            if (ismanager == "否")
            {
                label4.Visible = false;
                txtName.Visible = false;
            }
            BindData();
        }

        public void BindData()
        {
            //ds = UserDataOperation.getUserAll(Login.username);
            //string ismanager = ds.Tables[0].Rows[0]["ismanager"].ToString();

            //if (ismanager == "是")
            //{
            //    try
            //    {
            //        ds = InfoDataOperation.getAllInfo();
            //        this.dataGridView1.DataSource = ds.Tables[0];
            //    }
            //    catch (Exception ex)
            //    {
            //        ex.ToString();
            //    }
            //}
            //else
            //{
            //    try
            //    {
            //        ds = InfoDataOperation.getAllInfoByUname(Login.username);
            //        this.dataGridView1.DataSource = ds.Tables[0];
            //    }
            //    catch (Exception ex)
            //    {
            //        ex.ToString();
            //    }
            //}
            ds = UserDataOperation.getUserAll(Login.username);
            string ismanager = ds.Tables[0].Rows[0]["ismanager"].ToString();
            DateTime Intime1 = this.dateTimePicker1.Value;
            DateTime Intime2 = this.dateTimePicker2.Value;
            if (Intime1 > Intime2)
            {
                MessageBox.Show("from的日期不能超过to的日期！", "错误");
            }
            string searchname = txtName.Text;
            string IP = txtIP.Text;

            if (ismanager == "是")
            {
                try
                {
                    ds = InfoDataOperation.SearchInfo(Intime1, Intime2, searchname, IP);
                    this.dataGridView1.DataSource = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            else
            {
                try
                {
                    ds = InfoDataOperation.SearchInfo(Intime1, Intime2, Login.username, IP);
                    this.dataGridView1.DataSource = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int index = this.dataGridView1.CurrentRow.Index;
            if (index < 0)
            {
                MessageBox.Show("请选择要删除的记录！", "提示");
                return;
            }
            else
            {
                if (MessageBox.Show("确认要删除吗？", "删除", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    InfoData data = new InfoData();
                    data.infoID = ds.Tables[0].Rows[index]["infoID"].ToString();
                    try
                    {
                        if (InfoDataOperation.deleteinfo(data))
                        {
                            MessageBox.Show("删除成功！", "提示");
                            BindData();
                        }
                        else
                        {
                            MessageBox.Show("删除失败！", "错误");
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();

                        MessageBox.Show("删除失败！", "错误");
                    }
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog(); 
            saveFileDialog.Filter = "Execlfiles(*.xls)|*.xls"; 
            saveFileDialog.FilterIndex = 0;                
            saveFileDialog.RestoreDirectory = true;             
            saveFileDialog.CreatePrompt = true;              
            saveFileDialog.Title = "导出Excel文件到";              
            DateTime now = DateTime.Now;
            saveFileDialog.FileName = now.Year.ToString().PadLeft(2) + now.Month.ToString().PadLeft(2, '0') + now.Day.ToString().PadLeft(2, '0') + "-" + now.Hour.ToString().PadLeft(2, '0') + now.Minute.ToString().PadLeft(2, '0') + now.Second.ToString().PadLeft(2, '0') + "无线传感网的历史信息收集";      
            saveFileDialog.ShowDialog();               
            Stream myStream;          
            myStream = saveFileDialog.OpenFile(); 
           StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));             
            string str = "";                  
            try            { 
                for (int i = 0; i <   dataGridView1.ColumnCount; i++) 
                {  
                    if (i > 0)                     
                    {     
                        str += "\t";                      
                    }                     
                    str += dataGridView1.Columns[i].HeaderText;                 
                } 
                sw.WriteLine(str); 
                for (int j = 0; j < dataGridView1.Rows.Count-1; j++)  
                { 
                    string tempStr = "";                     
                    for (int k = 0; k < dataGridView1.Columns.Count; k++)                         
                    {
                        if (k > 0)
                        {
                            tempStr += "\t";
                        }
                        tempStr += dataGridView1.Rows[j].Cells[k].Value.ToString();
                    }
                    sw.WriteLine(tempStr);
                }
                sw.Close(); myStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
        }
    }
}
