using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using web.CData;
using System.Net;
using System.Net.Mail;

namespace web
{
    public partial class SendEmail : Form
    {
        private DataSet ds = new DataSet();
        private static string UserName; 

        public SendEmail()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtName.Text == null || txtName.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入用户名！", "提示");
                this.txtName.Focus();
                return;
            }

            if (txtEmail.Text == null || txtEmail.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入邮箱！", "提示");
                this.txtEmail.Focus();
                return;
            }

            if (UserDataOperation.GetUserName(txtName.Text.ToString()))
            {
                UserName = this.txtName.Text;
                UserData data = new UserData();
                data.username = this.txtName.Text;
                ds = UserDataOperation.getUserAll(UserName);

                if (ds.Tables[0].Rows[0]["isdel"].ToString() == "1")
                {
                    MessageBox.Show("账户已被注销，请联系管理员！", "提示");
                    return;
                }
                else
                {
                    if (this.txtEmail.Text == ds.Tables[0].Rows[0]["email"].ToString())
                    {
                        //
                        string newpassword = "123456";
                        //创建一封邮件
                        MailMessage m = new MailMessage();
                        //to,from,subject,body,
                        m.To.Add(ds.Tables[0].Rows[0]["email"].ToString());
                        m.From = new MailAddress("wangyuyan@gmail.com");
                        m.Subject = "您的密码";
                        m.Body = newpassword;
                        //创建一个虚拟的smtp客户端
                        SmtpClient c = new SmtpClient();

                        //Credentials凭据(用户名和密码),Host,Port,EnableSsl;
                        c.Credentials = new NetworkCredential("wangyuyan999@gmail.com", "loveyan1314");
                        c.Host = "smtp.gmail.com";
                        c.Port = 587;
                        c.EnableSsl = true;
                        //通过smtp客户端发送邮件
                        c.Send(m);
                        MessageBox.Show("邮件发送成功！", "提示");

                        data.password = newpassword;
                        data.username = this.txtName.Text;
                        UserDataOperation.updatepwd(data);
                       
                    }
                    else
                    {
                        MessageBox.Show("该邮箱和注册时不一致！", "提示");
                    }
                }
            }
            else
            {
                MessageBox.Show("没有该用户!", "提示");
                return;
            }                    
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            
            if (UserDataOperation.GetUserName(txtName.Text.ToString()))
            {
                   label3.Text = "*";
            }
            else
            {
                   label3.Text = txtName.Text + "不存在！";
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
