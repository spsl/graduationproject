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
    public partial class AddUser : Form
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Dispose();
            login.Show();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string username = this.txtuname.Text;
            string usersex = (string)this.cmbSex.SelectedItem;
            string userphone = this.txtPhone.Text;
            string useremail = this.txtEmail.Text;
            string ismanager = (string)this.cmbManager.SelectedItem;

            if(username == null || username.Trim().Equals(""))
            {
                MessageBox.Show("请输入用户名！", "提示");
                this.txtuname.Focus();
                return;
            }

            if (useremail == null || useremail.Trim().Equals(""))
            {
                MessageBox.Show("请输入email！", "提示");
                this.txtEmail.Focus();
                return;
            }

            if (ismanager == null || ismanager.Trim().Equals(""))
            {
                MessageBox.Show("请选择是否为管理员！", "提示");
                return;
            }

            UserData data = new UserData();
            data.username = username;
            data.password = "123456";
            data.sex = usersex;
            data.ismanager = ismanager;
            data.telphone = userphone;
            data.email = useremail;

            try
            {
                if(UserDataOperation.insertUser(data))
                {
                    MessageBox.Show("添加成功！", "提示");
                    Login login = new Login();
                    this.Hide();
                    login.Show();
                }
                else
                {
                    MessageBox.Show("添加失败！", "错误");
                    this.txtuname.Text = "";                  
                    this.cmbSex.Text = "";
                    this.cmbManager.Text = "";
                    this.txtPhone.Text = "";
                    this.txtEmail.Text = "";
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                MessageBox.Show("保存失败！", "错误");
            }

        }


        //检测用户名是否存在
        private void txtuname_TextChanged(object sender, EventArgs e)
        {
            if (UserDataOperation.GetUserName(txtuname.Text.ToString()))
            {
                lbluname.Text = txtuname.Text + "已存在！";

            }
            else
            {
                lbluname.Text = "*";
            }
        }
    }
}
