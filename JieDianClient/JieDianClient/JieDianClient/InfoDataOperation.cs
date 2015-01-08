using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using web;

namespace web.CData
{
    class InfoDataOperation
    {
        private static DAL dal = new DAL();

        //存储节点信息
        public static bool insertInfo(InfoData data)
        {
            string sql = "insert into [Info](temperature,voltage,time,username,light,IP) values(" +
                "'" + data.temperature + "'," +
                "'" + data.voltage + "'," +
                "'" + data.time + "'," +
                "'" + data.username + "'," +
                "'" + data.light + "'," +
                "'" + data.IP + "')";

            return dal.ExecuteSQL(sql);
        }

        //删除节点信息
        public static bool deleteinfo(InfoData data)
        {
            string sql = "delete from [Info] where infoID='"+data.infoID+"'";
            return dal.ExecuteSQL(sql);
        }

        //用户查询所有节点信息
        public static DataSet getAllInfoByUname(string UserName)
        {
            string sql = "select * from [info] where  username='"+ UserName +"'";
            return dal.GetDataSet(sql, "info");
        }

        //管理员查询所有节点信息
        public static DataSet getAllInfo()
        {
            string sql = "select * from [info]";
            return dal.GetDataSet(sql, "info");
        }

        //信息查询
        public static DataSet SearchInfo(DateTime startTime, DateTime endTime, string userName, string IP)
        {
            string sql = "select * from [info] where time between '" + startTime + "' and '" + endTime + "' and username like '%" + userName+ "%' and IP like '%"+ IP +"%'";
            return dal.GetDataSet(sql, "info");
        }

        //用户根据节点地址获取info
        public static DataSet getInfoByIP(string IP,string UserName)
        {
            string sql = "select * from [info] where IP='"+ IP +"' and username='" + UserName + "'";
            return dal.GetDataSet(sql, "info");
        }

        //管理员根据节点地址获取info
        public static DataSet getAllInfoByIP(string IP)
        {
            string sql = "select * from [info] where IP='" + IP + "'";
            return dal.GetDataSet(sql, "info");
        }
    }
}
