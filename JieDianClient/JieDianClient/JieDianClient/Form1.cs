using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CoAP;
using System.IO;
using System.Drawing.Imaging;
using MySql.Data.MySqlClient;
using web;
using jieDianClient;

namespace JieDianClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // f2 = form2;
        }

        private void Btn_DengLu_Click(object sender, EventArgs e)
        {       
            /*MySqlConnection sqlconnection = new MySqlConnection();
            sqlconnection.ConnectionString = "server=localhost;user id=root;password=271828;database=jiedian";
            sqlconnection.Open();
            //SqlCommand InsertCommand = new SqlCommand();
            //InsertCommand.Connection = sqlconnection;
            //InsertCommand.CommandText = "select Image from Image where IP=@IP1";
            //InsertCommand.Parameters.Add("@IP1", SqlDbType.VarChar, 50);
            //InsertCommand.Parameters["@IP1"].Value = "127.0.0.1";           
            //object num1 = InsertCommand.ExecuteScalar();
            //SqlDataReader re = InsertCommand.ExecuteReader();
            //while (re.Read() && re.Equals(null))
            //{
            //    MessageBox.Show(re.GetName(0));
            //}
            String ps = "'and password='";
            String ed="';";
            String sql = "select * from jd_user where username='" + Tb_UserName.Text + ps + Mtb_MiMa.Text + ed;
            MySqlCommand mySqlCommand = new MySqlCommand(sql,sqlconnection);
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
            try
            {
                    if (reader.HasRows)
                    {
                        MessageBox.Show("恭喜你登陆成功！！");
                        this.Hide();
                        Form2 f2 = new Form2();
                        f2.Show(); 
                    }
                    else 
                    {
                        MessageBox.Show("用户名或密码错误！！");
                    }          
            }
            catch (Exception)
            {
                MessageBox.Show("读取失败！");
            }
            finally
            {
                reader.Close();
            }*/

            //MySqlCommand QueryCommand = new MySqlCommand();
            //QueryCommand.Connection = sqlconnection;
            //QueryCommand.CommandText = "select * from jd_user where  username=@UserName1 and password="+Mtb_MiMa.Text;
            //MySqlParameter us = new MySqlParameter();
            //us.ParameterName = "@UserName1";
            //us.DbType = System.Data.DbType.String;
            //us.Value = Tb_UserName.Text;
            //QueryCommand.Parameters.Add(us);
            //MySqlParameter ps = new MySqlParameter();
            //ps.ParameterName = "@PassWord1";
            //ps.DbType = System.Data.DbType.String;
            //ps.Value = Mtb_MiMa.Text;
            //QueryCommand.Parameters.Add(ps);
            //object nu = QueryCommand.ExecuteScalar();
            //if (nu != null)
            //{
            //    this.Hide();
            //    Form2 f2 = new Form2();
            //    //this.
            //    //f2.Show();

            //}
            //else
            //{
            //    MessageBox.Show("用户名或密码错误！！");
            //}

            //QueryCommand.CommandText = "select Image from Image where IP=@IP1";
            //QueryCommand.Parameters.Add("@IP1", SqlDbType.VarChar, 50);
            //QueryCommand.Parameters["@IP1"].Value = "192.168.1.1";
            //int num1 = QueryCommand.ExecuteNonQuery();
            //MessageBox.Show("受影响的行数为:" + nu);
            //SqlDataReader re = QueryCommand.ExecuteReader();
            //////////////////////////////////////


            ///////王玉艳：
            if (Tb_UserName.Text == null || Tb_UserName.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入用户名！", "提示");
                this.Tb_UserName.Focus();
                return;
            }

            if (Mtb_MiMa.Text == null || Mtb_MiMa.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入密码！", "提示");
                this.Mtb_MiMa.Focus();
                return;
            }

            MySqlConnection sqlconnection = new MySqlConnection();
            sqlconnection.ConnectionString = "server=localhost;user id=root;password=271828;database=jiedian";
            sqlconnection.Open();
            //SqlCommand InsertCommand = new SqlCommand();
            //InsertCommand.Connection = sqlconnection;
            //InsertCommand.CommandText = "select Image from Image where IP=@IP1";
            //InsertCommand.Parameters.Add("@IP1", SqlDbType.VarChar, 50);
            //InsertCommand.Parameters["@IP1"].Value = "127.0.0.1";           
            //object num1 = InsertCommand.ExecuteScalar();
            //SqlDataReader re = InsertCommand.ExecuteReader();
            //while (re.Read() && re.Equals(null))
            //{
            //    MessageBox.Show(re.GetName(0));
            //}
            String ps = "'and password='";
            String ed="';";
            String sql = "select * from jd_user where username='" + Tb_UserName.Text + ps + Mtb_MiMa.Text + ed;
            MySqlCommand mySqlCommand = new MySqlCommand(sql,sqlconnection);
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
            try
            {
                    if (reader.HasRows)
                    {
                        MessageBox.Show("恭喜你登陆成功！！");
                        this.Hide();
                        Form2 f2 = new Form2();
                        f2.Show(); 
                    }
                    else 
                    {
                        MessageBox.Show("用户名或密码错误！！");
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

        private void Btn_Chongzhi_Click(object sender, EventArgs e)
        {
            Mtb_MiMa.Clear();
            Tb_UserName.Clear();
        }

        private void Btn_QuXiao_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddUser adduser = new AddUser();
            this.Hide();
            adduser.Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SendEmail sendemail = new SendEmail();
            sendemail.Show();
        }

    }
}