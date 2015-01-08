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
    public partial class UserModify : Form
    {
        public static string username;
        public UserModify(string name,string isdel,string ismanager,string sex,string telphone,string email)
        {
            InitializeComponent();
            username = name;
            groupBox1.Text = username + "的详细信息";

            txtname.Text = name;
            txtemail.Text = email;
            txtphone.Text = telphone;
            if (isdel == "0")
            {
                cmbisdel.SelectedItem="否";
            }
            else if (isdel == "1")
            {
                cmbisdel.SelectedItem="是";
            }
            cmbManager.SelectedItem = ismanager;
            cmbsex.SelectedItem = sex;
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            UserData data = new UserData();
            data.username = username;
            data.sex = (string)cmbsex.SelectedItem;
            data.email = txtemail.Text;
            data.telphone = txtphone.Text;
            if ((string)cmbisdel.Text == "是")
            {
                data.isdel = "1";
            }
            else if ((string)cmbisdel.Text == "否")
            {
                data.isdel = "0";
            }
            data.ismanager = (string)cmbManager.Text;

            try
            {
                if (UserDataOperation.updateuser(data))
                {
                    MessageBox.Show("修改成功！", "提示");
                    this.Close();

                }

                else
                {
                    MessageBox.Show("修改失败！", "错误");

                }
            }
            catch (Exception ex)
            {
                ex.ToString();

                MessageBox.Show("保存失败！", "错误");
            }
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
