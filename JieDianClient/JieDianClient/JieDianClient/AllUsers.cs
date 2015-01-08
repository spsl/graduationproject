using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using web.CData;

namespace web
{
    public partial class AllUsers : Form
    {
        private DataSet ds = new DataSet();

        public AllUsers()
        {
            InitializeComponent();
            BindData();
        }

        public void BindData()
        {
            UserData data = new UserData();
            try
            {
                ds = UserDataOperation.getAllUserInfo();
                this.dataGridView1.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void btndelete_Click(object sender, EventArgs e)
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
                    UserData data = new UserData();
                    data.username= ds.Tables[0].Rows[index]["username"].ToString();
                    try                     
                    {
                        if (UserDataOperation.updatedel(data))
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

        private void btnupdate_Click(object sender, EventArgs e)
        {
            int index = this.dataGridView1.CurrentRow.Index;
            if (index < 0)
            {
                MessageBox.Show("请选择要修改的记录！", "提示");
                return;
            }
            else
            {
                string name = ds.Tables[0].Rows[index]["username"].ToString();

                string isdel = ds.Tables[0].Rows[index]["isdel"].ToString();
                string ismanager = ds.Tables[0].Rows[index]["ismanager"].ToString();
                string sex = ds.Tables[0].Rows[index]["sex"].ToString();
                string telphone = ds.Tables[0].Rows[index]["telphone"].ToString();
                string email = ds.Tables[0].Rows[index]["email"].ToString();

                UserModify usermodify = new UserModify(name,isdel,ismanager,sex,telphone,email);
                this.Hide();
                usermodify.Show();

            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            UserData data = new UserData();
            
            try
            {
                ds = UserDataOperation.getUserInfoByUname(txtname.Text);
                this.dataGridView1.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}
