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
    public partial class PwdModify : Form
    {
        private DataSet ds = new DataSet();

        public PwdModify()
        {
            InitializeComponent();
            this.txtuname.Text = Login.username;
            ds = UserDataOperation.getUserAll(txtuname.Text);            
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            if (this.txtoldpwd.Text == null || this.txtoldpwd.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入旧密码！", "提示");

                this.txtoldpwd.Focus();

                return;
            }
            if (this.txtoldpwd.Text != ds.Tables[0].Rows[0]["password"].ToString())
            {
                MessageBox.Show("原密码不正确！", "提示");

                this.txtoldpwd.Focus();

                return;
            }
            if (this.txtnewpwd.Text == null || this.txtnewpwd.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入新密码！", "提示");

                this.txtnewpwd.Focus();

                return;
            }
            if (this.txtnewpwd1.Text == null || this.txtnewpwd1.Text.Trim().Equals(""))
            {
                MessageBox.Show("请再次输入新密码！", "提示");

                this.txtnewpwd1.Focus();

                return;
            }
            if (this.txtnewpwd1.Text != this.txtnewpwd.Text)
            {
                MessageBox.Show("确认密码不正确！", "提示");

                this.txtnewpwd1.Focus();

                return;
            }

            try
            {
                UserData data = new UserData();
                data.password = this.txtnewpwd.Text;
                data.username = this.txtuname.Text;

                if (UserDataOperation.updatepwd(data))
                {
                    MessageBox.Show("修改成功！", "提示");

                    this.Dispose();
                }
                else
                {
                    MessageBox.Show("修改失败！", "错误");

                    return;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

                MessageBox.Show("修改失败！", "错误");
            }

        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
