using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Xml;
using System.IO;
using web.CData;


namespace web
{
    public partial class Main : Form
    {
        private DataSet ds = new DataSet();

        public Main()
        {
            InitializeComponent();

            lblcontent.Text = "欢迎" + Login.username + "登录！";
            lblcontent.Text += DateTime.Now.ToString();

        }

        private void Main_Load(object sender, EventArgs e)
        {
            LoadMenu();
        }

        private void LoadMenu()
        {
            Menu menu = new Menu();

            ds = UserDataOperation.getUserAll(Login.username);
            string ismanager = ds.Tables[0].Rows[0]["ismanager"].ToString();
            if (ismanager == "是")
            {
                menu.Path = "../../ManagerMenu.xml";
                if (menu.FileExit())
                {
                    menu.LoadAllMenu(MainMenu);

                }
                else
                {
                    MessageBox.Show("XML文件加载失败！");
                }
            }
            else if (ismanager == "否")
            {
                menu.Path = "../../UserMenu.xml";
                if (menu.FileExit())
                {
                    menu.LoadAllMenu(MainMenu);

                }
                else
                {
                    MessageBox.Show("XML文件加载失败！");
                }
            }
        }

        private void logagainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Login login = new Login();
            login.Show();
        }

        private void logoutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }  
    }
}
