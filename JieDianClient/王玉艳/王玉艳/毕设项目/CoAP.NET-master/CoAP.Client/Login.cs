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
    public partial class Login : Form
    {
        private DataSet ds = new DataSet();
        private static string UserName; 

        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (uname.Text == null || uname.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入用户名！", "提示");
                this.uname.Focus();
                return;
            }

            if (pwd.Text == null || pwd.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入密码！", "提示");                
                this.pwd.Focus();
                return;
            }

            if (UserDataOperation.GetUserName(uname.Text.ToString()))
            {
                UserName = this.uname.Text;
                UserData data = new UserData();
                data.username = this.uname.Text;
                ds = UserDataOperation.getUserAll(UserName);

                if (ds.Tables[0].Rows[0]["isdel"].ToString() == "0")
                {
                    //检查输入的密码是否正确，不正确给出提示
                    if (this.pwd.Text == ds.Tables[0].Rows[0]["password"].ToString())
                    {
                        Main main = new Main();
                        this.Hide();
                        main.Show();
                    }
                    else
                    {
                        MessageBox.Show("密码不正确！", "提示");
                        this.pwd.Text = "";
                        this.pwd.Focus();
                        return;
                    }
                }
                else if (ds.Tables[0].Rows[0]["isdel"].ToString() == "1")
                {
                    MessageBox.Show("账户已被注销，请联系管理员！", "提示");
                    this.uname.Text = "";
                    this.pwd.Text = "";
                    return;
                }
            }
            else
            {
                MessageBox.Show("没有该用户，请联系管理员！", "提示");
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddUser adduser = new AddUser();
            this.Hide();
            adduser.Show();
        }

        public static string username
        {
            get
            {
                return UserName;
            }
        }

        private void lklforgetpwd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SendEmail sendemail = new SendEmail();
            sendemail.Show();
        }
    }
}
