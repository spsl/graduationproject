using System;
using System.Collections.Generic;
using System.ComponentModel;
using CoAP.EndPoint;
using CoAP.EndPoint.Resources;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CoAP;
using CoAP.Examples;
using web.CData;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data.OleDb;

namespace web
{
    public partial class Info : Form
    {
         private DataSet ds = new DataSet();

        public Info()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string uname = Login.username;
            string IP = txtIP.Text.ToString();
            string info = "voltage";
            string[] args = new String[] { "GET", "coap://["+IP+"]/voltage" };
            CoAPClient.Mains(args, info, uname, IP);
            //args = new String[] { "GET", "coap://[aaaa::33]/temperature" };
            args = new String[] { "GET", "coap://[" + IP + "]/temperature" };
            info = "temperature";
            CoAPClient.Mains(args, info, uname, IP);
            args = new String[] { "GET", "coap://[" + IP + "]/light" };
            info = "light";
            CoAPClient.Mains(args, info, uname, IP);
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            //图片属性设置
            chart1.Width = 820;    //图片宽度
            chart1.Height = 380;                      //图片高度
            chart1.BackColor = Color.Azure;           //图片背景色

            //数据集显示属性设置

           
            Series series1 = new Series("温度");        //数据集声明  
            series1.ChartType = SeriesChartType.Line;   //数据显示方式 Line:为折线  Spline:曲线 
            series1.Color = Color.Green;                //线条颜色
            series1.BorderWidth = 2;                    //线条宽度
            series1.ShadowOffset = 1;                   //阴影宽度
            series1.IsVisibleInLegend = true;           //是否显示数据说明
            series1.IsValueShownAsLabel = false;        //线条上是否给吃数据的显示
            series1.MarkerStyle = MarkerStyle.Circle;   //线条上的数据点标志类型
            series1.MarkerSize = 8;                     //              标志的大小

            chart1.Series.Add(series1);                 //把数据集添加到chart中

          
            Series series2 = new Series("光度");
            series2.ChartType = SeriesChartType.Line;   //数据显示方式 Line:为折线  Spline:曲线 
            series2.Color = Color.Red;                //线条颜色
            series2.BorderWidth = 2;                    //线条宽度
            series2.ShadowOffset = 1;                   //阴影宽度
            series2.IsVisibleInLegend = true;           //是否显示数据说明
            series2.IsValueShownAsLabel = false;        //线条上是否给吃数据的显示
            series2.MarkerStyle = MarkerStyle.Circle;   //线条上的数据点标志类型
            series2.MarkerSize = 8;                     //              标志的大小

            chart1.Series.Add(series2);

            Series series3 = new Series("电压");
            series3.ChartType = SeriesChartType.Line;   //数据显示方式 Line:为折线  Spline:曲线 
            series3.Color = Color.Yellow;                //线条颜色
            series3.BorderWidth = 2;                    //线条宽度
            series3.ShadowOffset = 1;                   //阴影宽度
            series3.IsVisibleInLegend = true;           //是否显示数据说明
            series3.IsValueShownAsLabel = false;        //线条上是否给吃数据的显示
            series3.MarkerStyle = MarkerStyle.Circle;   //线条上的数据点标志类型
            series3.MarkerSize = 8;                     //              标志的大小

            chart1.Series.Add(series3);

            ds = UserDataOperation.getUserAll(Login.username);
            string ismanager = ds.Tables[0].Rows[0]["ismanager"].ToString();

            if (ismanager == "是")
            {
                ds = InfoDataOperation.getAllInfoByIP(txtIP.Text);

            }
            else
            {
                ds = InfoDataOperation.getInfoByIP(txtIP.Text, Login.username);
            }


            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                series1.Points.AddXY(ds.Tables[0].Rows[i]["time"].ToString(), ds.Tables[0].Rows[i]["temperature"].ToString());
            }

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                series2.Points.AddXY(ds.Tables[0].Rows[i]["time"].ToString(), ds.Tables[0].Rows[i]["light"].ToString());
            }

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                series3.Points.AddXY(ds.Tables[0].Rows[i]["time"].ToString(), ds.Tables[0].Rows[i]["voltage"].ToString());
            }
            //作图区的显示属性设置
            chart1.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
            chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;
            //背景色设置
            chart1.ChartAreas["ChartArea1"].ShadowColor = Color.Transparent;
            chart1.ChartAreas["ChartArea1"].BackColor = Color.Azure;         //该处设置为了由天蓝到白色的逐渐变化
            chart1.ChartAreas["ChartArea1"].BackGradientStyle = GradientStyle.TopBottom;
            chart1.ChartAreas["ChartArea1"].BackSecondaryColor = Color.White;
            //X,Y坐标线颜色和大小
            chart1.ChartAreas["ChartArea1"].AxisX.LineColor = Color.Blue;
            chart1.ChartAreas["ChartArea1"].AxisY.LineColor = Color.Blue;
            chart1.ChartAreas["ChartArea1"].AxisX.LineWidth = 2;
            chart1.ChartAreas["ChartArea1"].AxisY.LineWidth = 2;
            chart1.ChartAreas["ChartArea1"].AxisY.Title = txtIP.Text + "节点";
            //中间X,Y线条的颜色设置
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineColor = Color.Blue;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.Blue;
            //X.Y轴数据显示间隔
            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;  //X轴数据显示间隔
            chart1.ChartAreas["ChartArea1"].AxisY.Interval = 10;
            //X轴线条显示间隔
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Interval = 1;

        }
    }
}
