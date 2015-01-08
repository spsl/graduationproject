using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using MySql.Data.MySqlClient;
using web.CData;

namespace web
{
    public partial class SendEmail : Form
    {
        private DataSet ds = new DataSet();
        //private static string UserName; 

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


            MySqlConnection sqlconnection = new MySqlConnection();
            sqlconnection.ConnectionString = "server=localhost;user id=root;password=271828;database=jiedian";
            sqlconnection.Open();
            String ps = "'and email='";
            String ed = "';";
            String sql = "select * from jd_user where username='" + txtName.Text + ps + txtEmail.Text + ed;
            MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlconnection);
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
            try
            {           
                //MessageBox.Show(reader[0].ToString());
                if (reader.HasRows)
                {

                       
//                     string newpassword = "123456";
//                     //创建一封邮件
//                     MailMessage m = new MailMessage();
//                     //to,from,subject,body,
//                     m.To.Add(txtEmail.Text);
//                     m.From = new MailAddress("NJUPT_IPV6@163.com");
//                     m.Subject = "您的密码";
//                     m.Body = newpassword;
//                     //创建一个虚拟的smtp客户端
//                     SmtpClient c = new SmtpClient();
//                        //Credentials凭据(用户名和密码),Host,Port,EnableSsl;
////                        c.Credentials = new NetworkCredential("wangyuyan999@gmail.com", "loveyan1314");
//                     c.Credentials = new NetworkCredential("NJUPT_IPV6@163.com", "wsn202");
//                     c.Host = "smtp.163.com";
//                     c.Port = 587;
//                     c.EnableSsl = true;
//                     //通过smtp客户端发送邮件
//                     c.Send(m);
                    try
                    {

                        //邮件发送类 

                        MailMessage mail = new MailMessage();

                        //是谁发送的邮件 

                        mail.From = new MailAddress("kaka_272@126.com","某某人");

                        //发送给谁 

                        mail.To.Add(txtEmail.Text);

                        //标题 

                        mail.Subject = "你的密码！！！";

                        //内容编码 

                        mail.BodyEncoding = Encoding.Default;

                        //发送优先级 

                        mail.Priority = MailPriority.High;

                        //邮件内容 

                        String mima;
                        while (reader.Read())
                        {
                           
                        }
                        mail.Body = reader[2].ToString();

                        //是否HTML形式发送 

                        mail.IsBodyHtml = true;

                        //邮件服务器和端口 

                        SmtpClient smtp = new SmtpClient("smtp.126.com", 25);

                        smtp.UseDefaultCredentials = true;

                        //指定发送方式 

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        //指定登录名和密码 

                        smtp.Credentials = new System.Net.NetworkCredential("kaka_272", "adminadmin");

                        //超时时间 

                        smtp.Timeout = 10000;

                        smtp.Send(mail);

                        MessageBox.Show("send ok"); 

                    }

                    catch (Exception exp)
                    {

                        MessageBox.Show(exp.Message);

                    } 
          
                }
                else
                {
                    MessageBox.Show("用户名或邮箱错误！！");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("读取失败！");
            }
            finally
            {
                reader.Close();
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
