using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CoAP.EndPoint;
using CoAP.EndPoint.Resources;
using System.IO;
using MySql.Data.MySqlClient;
using System.Threading;
using CoAP;


namespace JieDianClient
{
    public partial class Form2 : Form
    {
        public Form2()
        {        
            InitializeComponent();           
        }

        private void saveImage()
        {
            String Opath = @"D:\graduationproject\resources\images";
            String phototime = DateTime.Now.Ticks.ToString();
            if (Opath.Substring(Opath.Length - 1, 1) != @"/")
                Opath = Opath + @"/";

            MessageBox.Show(Opath);
            System.Drawing.Bitmap objPic;
            try
            {
                objPic = new System.Drawing.Bitmap(pictureBox1.Image);


                String ImageName = "camera" + phototime + ".jpg";
                MessageBox.Show(ImageName);
                objPic.Save(Opath + ImageName, System.Drawing.Imaging.ImageFormat.Png);


                MySqlConnection sqlconnection = new MySqlConnection();
                sqlconnection.ConnectionString = "server=localhost;user id=root;password=sunsai;database=jiedian";//配置数据库连接
                sqlconnection.Open();

                MySqlCommand mySqlCommand = new MySqlCommand();
                mySqlCommand.Connection = sqlconnection;

                mySqlCommand.CommandText = "jd_image_create";//调用存储过程
                mySqlCommand.CommandType = CommandType.StoredProcedure;
                mySqlCommand.Parameters.Add(new MySqlParameter("in_jd_ip", MySqlDbType.VarChar, 200));
                mySqlCommand.Parameters.Add(new MySqlParameter("in_image_url", MySqlDbType.VarChar, 450));
                mySqlCommand.Parameters["in_jd_ip"].Value = textBox1.Text.Trim();

                mySqlCommand.Parameters["in_image_url"].Value = Opath + ImageName;
                mySqlCommand.ExecuteNonQuery();

                MessageBox.Show("保存成功");


            }
            catch (Exception exp) { throw exp; }
            finally
            {
                objPic = null;
                //objNewPic = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
             if (pictureBox1.Image != null)
                { 
                   String Opath = @"D:\graduationproject\resources\images";
                   String phototime = DateTime.Now.Ticks.ToString();
                   if (Opath.Substring(Opath.Length - 1, 1) != @"/")
                       Opath = Opath + @"/";

                   MessageBox.Show(Opath);
                   System.Drawing.Bitmap objPic;
                   try
                   {
                       objPic = new System.Drawing.Bitmap(pictureBox1.Image);
                       

                       String ImageName ="camera"+phototime+".jpg";
                       MessageBox.Show(ImageName);
                       objPic.Save(Opath + ImageName, System.Drawing.Imaging.ImageFormat.Png);


                       MySqlConnection sqlconnection = new MySqlConnection();
                       sqlconnection.ConnectionString = "server=localhost;user id=root;password=sunsai;database=jiedian";//配置数据库连接
                       sqlconnection.Open();

                       MySqlCommand mySqlCommand = new MySqlCommand();
                       mySqlCommand.Connection = sqlconnection;

                       mySqlCommand.CommandText = "jd_image_create";//调用存储过程
                       mySqlCommand.CommandType = CommandType.StoredProcedure;
                       mySqlCommand.Parameters.Add(new MySqlParameter("in_jd_ip", MySqlDbType.VarChar, 200));
                       mySqlCommand.Parameters.Add(new MySqlParameter("in_image_url", MySqlDbType.VarChar, 450));
                       mySqlCommand.Parameters["in_jd_ip"].Value = textBox1.Text.Trim();

                       mySqlCommand.Parameters["in_image_url"].Value = Opath + ImageName;
                       mySqlCommand.ExecuteNonQuery();

                       MessageBox.Show("保存成功");


                   }
                   catch (Exception exp) { throw exp; }
                   finally
                   {
                       objPic = null;
                       //objNewPic = null;
                   }
//////////////////////////////////////////////////////////////////////////////////////////////////////////
                 


               } 
        }

        private void button4_Click(object sender1, EventArgs ee)
        {
#region

            List<String> allNodes = ActionNodeList.getAllNodes();

            MessageBox.Show(allNodes.ToString());

            this.Text = "获取节点信息！";
            pictureBox1.Image = null;
            richTextBox1.ResetText();
            String payload = null;
            Request request = new Request(Method.GET);

                
            String re_uri = "coap://["+textBox1.Text + "]/" + comboBox1.Text;
            String imageSize = "?size=1";
            if (radioButton2.Checked)
                imageSize = "?size=2";
            else if (radioButton3.Checked)
                imageSize = "?size=3";


            if (comboBox1.Text == "camera") re_uri = re_uri + imageSize;
            request.URI = new Uri(re_uri);
            MessageBox.Show(re_uri);

            request.SetPayload(payload);
          
            request.Timeout += new EventHandler(request_Timeout);
            
            try
            {
                request.Respond += OnResponse;
                request.Send();                         
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed executing request: " + ex.Message);
            }
#endregion
            //Thread th = new Thread(ThreadChild);
            //th.Start(); 
           
        }

        void OnResponse(Object sender, ResponseEventArgs e)
        {
            if (this.InvokeRequired)
            {
                MessageBox.Show("invoke");
                this.Invoke(new EventHandler<ResponseEventArgs>(OnResponse), sender, e);
                return;
            }

            MessageBox.Show("onresponse1");
            Response response = e.Response;
            //response.ContentType == MediaType.TextPlain, MediaType.ImageJpeg
            if (response == null)
            {
                MessageBox.Show("Request timeout");
            }
            else
            {
                MessageBox.Show("onresponse2");
                if (response.PayloadString != null && !MediaType.IsImage(response.ContentType))
                {
                    if (response.Payload == null) MessageBox.Show("Time (ms): " + response.RTT);

                    

                    MySqlConnection sqlconnection = new MySqlConnection();
                    sqlconnection.ConnectionString = "server=localhost;user id=root;password=sunsai;database=jiedian";//配置数据库连接
                    sqlconnection.Open();

                    MySqlCommand mySqlCommand = new MySqlCommand();
                    mySqlCommand.Connection = sqlconnection;

                    mySqlCommand.CommandText = "jd_voltage_create";//调用存储过程
                    mySqlCommand.CommandType = CommandType.StoredProcedure;
                    mySqlCommand.Parameters.Add(new MySqlParameter("in_ip", MySqlDbType.VarChar, 100));
                    mySqlCommand.Parameters.Add(new MySqlParameter("in_voltage",MySqlDbType.Int16));
                    mySqlCommand.Parameters["in_ip"].Value = textBox1.Text.Trim();
                    
                    mySqlCommand.Parameters["in_voltage"].Value = response.PayloadString;
                    mySqlCommand.ExecuteNonQuery();


                    richTextBox1.Text = response.PayloadString;// 从节点获得电压数据，下面要保存到数据库里面；
                    tabControl2.SelectedIndex = 0;
                    //tabControl2.SelectTab("文本");

                   
                }
          
                if (response.Payload == null) MessageBox.Show("response.Payload == null");
                else   if (MediaType.IsImage(response.ContentType))
                {
                    
                    pictureBox1.Image = CoapClient.ImageByte.ByteToImage(response.Payload);
                    
                    tabControl2.SelectedIndex = 1;

                    saveImage();


                    //tabControl2.SelectTab("Image");
                }
            }
            //if (!response.IsEmptyACK && !loop)
            //    Environment.Exit(0);
        }

        void request_Timeout(object sender, EventArgs e)
        {
            MessageBox.Show("Request timeout");
        }
        static void ThreadChild()
        { 
            MessageBox.Show("这是子线程执行的代码");
    
        }



        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private List<String> imagesUrls = null;
        private int imagesIndex = 0;

             

        private void button3_Click(object sender, EventArgs e)//这是查询数据库
        {
            pictureBox2.Image = null;

            pictureBox2.Image = Image.FromFile(@"D:\graduationproject\resources\images\camera635621180375445001.jpg");


            MySqlConnection sqlconnection = new MySqlConnection();
            sqlconnection.ConnectionString = "server=localhost;user id=root;password=sunsai;database=jiedian";
            sqlconnection.Open();

            MySqlCommand mySqlCommand = new MySqlCommand();

            mySqlCommand.Connection = sqlconnection;
            mySqlCommand.CommandText = "jd_images_get_by_time";
            mySqlCommand.CommandType = CommandType.StoredProcedure;


           

         

            mySqlCommand.Parameters.Add(new MySqlParameter("in_ip", MySqlDbType.VarChar, 50));
            mySqlCommand.Parameters["in_ip"].Value = tb_ip.Text;

            mySqlCommand.Parameters.Add(new MySqlParameter("in_start_time", MySqlDbType.Datetime));
            mySqlCommand.Parameters["in_start_time"].Value = dateTimePicker1.Value;

            mySqlCommand.Parameters.Add(new MySqlParameter("in_end_time", MySqlDbType.Datetime));
            mySqlCommand.Parameters["in_end_time"].Value = dateTimePicker3.Value;



            MySqlDataReader reader = mySqlCommand.ExecuteReader();

            imagesUrls = new List<string>();


            while (reader.Read())
            {
                String imageUrl = reader["image_url"].ToString();

                imagesUrls.Add(imageUrl);
            }

            sqlconnection.Close();
           

            MessageBox.Show(dateTimePicker1.Value.ToString());

   
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection sqlconnection = new MySqlConnection();
                sqlconnection.ConnectionString = "server=localhost;user id=root;password=sunsai;database=jiedian";
                sqlconnection.Open();
               // String sql = "select * from jd_user";
                MySqlCommand mySqlCommand = new MySqlCommand();

                mySqlCommand.Connection = sqlconnection;
                mySqlCommand.CommandText = "jd_user_get_all";
                mySqlCommand.CommandType = CommandType.StoredProcedure;



                MySqlDataAdapter sda = new MySqlDataAdapter(mySqlCommand);
                DataSet ds = new DataSet();
                sda.Fill(ds, "用户表");
                dataGridView2.DataSource = ds;
                dataGridView2.DataMember = "用户表";
                dataGridView2.AutoGenerateColumns = true;
                for (int i = 1; i < this.dataGridView1.ColumnCount; i++)
                {
                    this.dataGridView1.Columns[i].DefaultCellStyle.SelectionBackColor = Color.White;
                    this.dataGridView1.Columns[i].DefaultCellStyle.SelectionForeColor = Color.Black;
                    this.dataGridView1.Columns[i].ReadOnly = true;
                }
                sqlconnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.Message, "错误");
            }
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedCells == null)             
            {                  
                MessageBox.Show("请选择要删¦除的项！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);             
            }             
            else             
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 0)                
                {
                    string st = dataGridView2[0, dataGridView2.CurrentCell.RowIndex].Value.ToString();
                    MySqlConnection sqlconnection = new MySqlConnection();
                    sqlconnection.ConnectionString = "server=localhost;user id=root;password=sunsai;database=jiedian";
                    sqlconnection.Open();
                    String sql = "delete from jd_user where username='" + st + "'";
                    MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlconnection);
                    mySqlCommand.ExecuteNonQuery();
                    MessageBox.Show("已删除！请刷新！");
                    sqlconnection.Close();
                 
                }             
            }         
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection sqlconnection = new MySqlConnection();
                sqlconnection.ConnectionString = "server=localhost;user id=root;password=sunsai;database=jiedian";
                sqlconnection.Open();
              //  String sql = "select * from jd_user where username='"+textBox2.Text+"';";
                MySqlCommand mySqlCommand = new MySqlCommand();
                mySqlCommand.Connection = sqlconnection;

                mySqlCommand.CommandText = "jd_user_get_by_username";
                mySqlCommand.CommandType = CommandType.StoredProcedure;

                mySqlCommand.Parameters.Add(new MySqlParameter("in_username", MySqlDbType.VarChar, 50));
                mySqlCommand.Parameters["in_username"].Value = textBox2.Text;


                MySqlDataAdapter sda = new MySqlDataAdapter(mySqlCommand);
                DataSet ds = new DataSet();
                sda.Fill(ds, "用户表");
                dataGridView2.DataSource = ds;
                dataGridView2.DataMember = "用户表";
                dataGridView2.AutoGenerateColumns = true;
                for (int i = 1; i < this.dataGridView1.ColumnCount; i++)
                {
                    this.dataGridView1.Columns[i].DefaultCellStyle.SelectionBackColor = Color.White;
                    this.dataGridView1.Columns[i].DefaultCellStyle.SelectionForeColor = Color.Black;
                    this.dataGridView1.Columns[i].ReadOnly = true;
                }
                sqlconnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.Message, "错误");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
          /*  if (tb_username.Text != null&&tb_mima!=null)
            {
                            //建立数据库连接                  
                MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Password=271828;Database=jiedian");
                conn.Open();                   //设置命令参数                  
                string insertStr = "insert into jd_user(username,password) values(?UN,?MM)";
                MySqlCommand comm = new MySqlCommand();
                comm.Connection = conn;
                comm.CommandText = insertStr;
                comm.CommandType = CommandType.Text;
                comm.Parameters.Add(new MySqlParameter("?UN", MySqlDbType.String)).Value = tb_username.Text.Trim();
                comm.Parameters.Add(new MySqlParameter("?MM", MySqlDbType.String)).Value = tb_mima.Text.Trim();
                try
                {
                    comm.ExecuteNonQuery();
                    MessageBox.Show("添加成功！！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                } comm.Dispose();
                conn.Close();
                conn.Dispose();
            }*/
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            List<String> allNodes = ActionNodeList.getAllNodes();

            MessageBox.Show(allNodes.ToString());

            List<String> aliveNodes = new Tool().filterAliveNodes(allNodes);


        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (imagesIndex < imagesUrls.Count)
            {
                String imageUrl = imagesUrls.ToArray()[imagesIndex++];
                pictureBox2.Image = Image.FromFile(@imageUrl);

            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            if (imagesIndex > 0)
            {
                String imageUrl = imagesUrls.ToArray()[imagesIndex--];
                pictureBox2.Image = Image.FromFile(@imageUrl);

            }
        }
    }


    public class ActionNodeList //获取数据库中所有的节点，
    {
        public static List<String> nodes = null;

        private ActionNodeList(){

        }

        public static List<String> getAllNodes()
        {
            if(nodes == null)
            {
                init();
            }

            return nodes;
        }

        private static void init()
        {
            MySqlConnection sqlconnection = new MySqlConnection();
            sqlconnection.ConnectionString = "server=localhost;user id=root;password=sunsai;database=jiedian";
            sqlconnection.Open();
            
            MySqlCommand mySqlCommand = new MySqlCommand();

            mySqlCommand.Connection = sqlconnection;
            mySqlCommand.CommandText = "jd_ip_get_all";
            mySqlCommand.CommandType = CommandType.StoredProcedure;

            MySqlDataReader reader = mySqlCommand.ExecuteReader();

            nodes = new List<String>();

            while (reader.Read())
            {
                String tmpIp = reader["ip"].ToString();
                nodes.Add(tmpIp);
                MessageBox.Show(tmpIp);

            }
           
            sqlconnection.Close();
        }


    }

    
}
      
         
    
