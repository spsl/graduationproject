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
    public partial class PerInfo : Form
    {
        private DataSet ds = new DataSet();

        public PerInfo()
        {
            InitializeComponent();
            this.txtname.Text = Login.username;
            ds = UserDataOperation.getUserAll(txtname.Text);
            this.txtemail.Text = ds.Tables[0].Rows[0]["email"].ToString();
            this.txtphone.Text = ds.Tables[0].Rows[0]["telphone"].ToString();
            this.cmbsex.SelectedItem=ds.Tables[0].Rows[0]["sex"].ToString();

            txtname.Enabled = false;
            txtemail.Enabled = false;
            txtphone.Enabled = false;
            cmbsex.Enabled = false;
            btnsave.Visible = false;
        }

        private void btnupdate_Click(object sender, EventArgs e)
        {
            txtemail.Enabled = true;
            txtphone.Enabled = true;
            cmbsex.Enabled = true;

            btnupdate.Visible = false;
            btnsave.Visible = true;

        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            UserData data = new UserData();
            data.telphone = this.txtphone.Text;
            data.email = this.txtemail.Text;
            data.sex = (string)this.cmbsex.SelectedItem;
            data.username = Login.username;

            try
            {
                if (UserDataOperation.updateinfo(data))
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
    }
}
