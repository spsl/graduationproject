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
    
    public class Tool
    {
        int alive = 0;
        public List<String> filterAliveNodes(List<String> allNodes)
        {
            List<String> aliveNodes = new List<string>();

            foreach (String ip in allNodes)
            {
                voltage(ip);
                while (alive == 0)
                {
                    
                }

                if (alive == 1)
                {
                    aliveNodes.Add(ip);
                    
                    alive = 0;
                    MessageBox.Show("alive:" + ip);
                }
                if (alive == -1)
                {
                    
                    alive = 0;
                }

            }

            return aliveNodes;
           
        }

        public int voltage(String ipAddress)
        {
            String payload = null;
            Request request = new Request(Method.GET);


            String re_uri = "coap://[" + ipAddress + "]/voltage";
           
            request.SetPayload(payload);

            request.Timeout += new EventHandler(request_Timeout);

            try
            {
                request.Respond += this.OnResponse;
                request.Send();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed executing request: " + ex.Message);
            }

            return 0;

        }

        void request_Timeout(object sender, EventArgs e)
        {
            MessageBox.Show("Request timeout");
        }

        private void OnResponse(Object sender, ResponseEventArgs e)
        {

            MessageBox.Show("onresponse1");
            Response response = e.Response;
          
            if (response == null)
            {
                MessageBox.Show("Request timeout");
                alive = -1;
            }
            else
            {
                MessageBox.Show("onresponse2");
                if (response.PayloadString != null && !MediaType.IsImage(response.ContentType))
                {
                    if (response.Payload == null) MessageBox.Show("Time (ms): " + response.RTT);


                    String voltage = response.PayloadString;// 从节点获得电压数据，下面要保存到数据库里面；

                    alive = 1;
                  
                }

               
            }
          
        }

    }
}
