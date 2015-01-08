using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using web;

namespace web.CData
{
    class UserDataOperation
    {
        private static DAL dal = new DAL();

        public static bool GetUserName(string UserName)
        {
            bool isAdd = false;
            string sql = "select * from [users] where username='"+ UserName +"'";
            web.DAL dal = new DAL();
            DataTable dt = dal.GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                isAdd = true;
            }
            return isAdd;
        }

        //获取个人信息
        public static DataSet getUserAll(string name)
        {
            string sql = "select * from [users] where username='"+ name +"'";
            return dal.GetDataSet(sql, "users");
        }

        //添加用户
        public static bool insertUser(UserData data)
        {
            string sql = "insert into [users](username,password,ismanager,sex,telphone,email) values("+
                "'" + data.username + "'," +
                "'" + data.password + "'," +
                "'" + data.ismanager + "'," +
                "'" + data.sex + "'," +
                "'" + data.telphone + "'," +
                "'"+ data.email+"')";

            return dal.ExecuteSQL(sql);
        }
      
        //查看全部用户信息
        public static DataSet getAllUserInfo()
        {
            string sql = "select * from [users]";
            return dal.GetDataSet(sql, "users");
        }

        //管理员修改用户信息
        public static bool updateuser(UserData data)
        {
            string sql = "update [users] set " +
                 "sex='" + data.sex + "', " +
                 "email='" + data.email + "', " +
                 "telphone='" + data.telphone + "', " +
                 "isdel='" + data.isdel + "', " +
                 "ismanager='" + data.ismanager + "' where username='" + data.username + "'";
            return dal.ExecuteSQL(sql);
        }

        //修改用户信息
        public static bool updateinfo(UserData data)
        {
            string sql = "update [users] set sex ='" + data.sex + "',telphone='"+data.telphone+"',email='"+data.email+"' where username='" + data.username + "'";
            return dal.ExecuteSQL(sql);
        }

        //删除用户isdel=1
        public static bool updatedel(UserData data)
        {
            string sql = "update [users] set isdel =1 where username='" + data.username + "'";
            return dal.ExecuteSQL(sql);
        }

        //修改密码
        public static bool updatepwd(UserData data)
        {
            string sql = "update [users] set password ='"+data.password+"' where username='"+data.username+"'";
            return dal.ExecuteSQL(sql);
        }

        //search
        public static DataSet getUserInfoByUname( string userName)
        {
            string sql = "select * from [users] where username like  '%"+ userName +"%'";
            return dal.GetDataSet(sql, "users");
        }
    }
}
