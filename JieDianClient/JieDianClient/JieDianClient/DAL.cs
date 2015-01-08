using System;
using System.Collections.Generic;

using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Xml;
using MySql.Data.MySqlClient;

namespace web
{
    public class DAL
    {
         #region 构造函数
        ///<summary>
        ///构造函数
        ///</summary>
        public DAL()
        {
        }
        #endregion

        #region 配置数据库连接字符串
        /// <summary>
        /// 配置数据库连接字符串
        /// </summary>
        public static string ConnectionString = "server=localhost;user id=root;password=271828;database=jiedian";
        #endregion

        #region  执行SQL语句，返回Bool值
        /// <summary>
        /// 执行SQL语句，返回Bool值
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <returns>返回BOOL值，True为执行成功</returns>
        public bool ExecuteSQL(string sql)
        {
            MySqlConnection sqlconnection = new MySqlConnection();
            sqlconnection.ConnectionString = "server=localhost;user id=root;password=271828;database=jiedian";
            sqlconnection.Open();
            MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlconnection);
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    return true; 
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                reader.Close();
            }
            return false;
        }
        #endregion

        #region 执行SQL语句，返回SqlDataReader
        /// <summary>
        /// 执行SQL语句，返回SqlDataReader
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <returns>返回SqlDataReader，需手工关闭连接</returns>
        public MySqlDataReader GetReader(string sql)
        {
            MySqlConnection con = new MySqlConnection(DAL.ConnectionString);
            MySqlCommand cmd = new MySqlCommand(sql, con);
            MySqlDataReader dr = null;
            try
            {
                con.Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                dr.Close();
                con.Dispose();
                cmd.Dispose();
                throw new Exception(ex.ToString());
            }
            return dr;
        }
        #endregion

        #region  分页，返回SqlDataReader
        /// <summary>
        /// 分页，返回SqlDataReader
        /// </summary>
        /// <param name="tblName">查询的表名</param>
        /// <param name="fldName">排序字段名</param>
        /// <param name="PageSize">每页中记录的数量</param>
        /// <param name="PageIndex">当前查询的页码</param>
        /// <param name="OrderType">设置排序类型, 非 0 值则降序</param>
        /// <param name="strWhere">查询条件(注意:不要加 where)</param>
        ///<returns>返回SqlDataReader，需手工关闭连接</returns>
        public MySqlDataReader GetReaderPage(string tblName, string fldName, int PageSize, int PageIndex, int OrderType, string strWhere)
        {
            string strTmp, strOrder;
            string sql = "";
            if (OrderType != 0)
            {
                strTmp = "< (select min";
                strOrder = " order by " + fldName + " desc";
            }
            else
            {
                strTmp = ">(select max";
                strOrder = " order by " + fldName + " asc";
            }
            if (strWhere != "")
            {
                sql = "select top " + PageSize + " * from " + tblName + " where " + fldName + strTmp + "(";
                sql += fldName + ") from (select top " + (PageIndex - 1) * PageSize + " " + fldName + " from " + tblName + " where (" + strWhere + ") ";
                sql += strOrder + ") as tblTmp) and (" + strWhere + ") " + strOrder;
            }
            if (PageIndex == 1)
            {
                strTmp = "";
                if (strWhere != "")
                {
                    strTmp = " where (" + strWhere + ")";
                }
                sql = "select top " + PageSize + " * from " + tblName + strTmp + " " + strOrder;
            }
            MySqlConnection con = new MySqlConnection(DAL.ConnectionString);
            MySqlCommand cmd = new MySqlCommand(sql, con);
            MySqlDataReader dr = null;
            try
            {
                con.Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                dr.Close();
                con.Dispose();
                cmd.Dispose();
                throw new Exception(ex.ToString());
            }
            return dr;
        }
        #endregion

        #region  执行SQL语句，返回DataSet
        /// <summary>
        /// 执行SQL语句，返回DataSet
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="tablename">DataSet中要填充的表名</param>
        /// <returns>返回dataSet类型的执行结果</returns>
        public DataSet GetDataSet(string sql, string tablename)
        {
            DataSet ds = new DataSet();
            MySqlConnection con = new MySqlConnection(DAL.ConnectionString);
            MySqlDataAdapter da = new MySqlDataAdapter(sql, con);
            try
            {
                da.Fill(ds, tablename);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                con.Close();
                con.Dispose();
                da.Dispose();
            }
            return ds;
        }
        #endregion

        #region  执行SQL语句，返回DataTable
        /// <summary>
        /// 执行SQL语句，返回DataTable
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <returns>返回DataTable类型的执行结果</returns>
        public DataTable GetDataTable(string sql)
        {
            DataSet ds = new DataSet();
            MySqlConnection con = new MySqlConnection(DAL.ConnectionString);
            MySqlDataAdapter da = new MySqlDataAdapter(sql, con);
            try
            {
                da.Fill(ds, "tb");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                con.Close();
                con.Dispose();
                da.Dispose();
            }
            DataTable result = ds.Tables["tb"];
            return result;
        }
        #endregion

        #region  执行SQL语句并返回受影响的行数
        /// <summary>
        /// 执行SQL语句并返回受影响的行数
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <returns>返回Int类型的受影响的行数</returns>
        public int GetCount(string sql)
        {

            MySqlConnection con = new MySqlConnection(DAL.ConnectionString);
            MySqlCommand cmd = new MySqlCommand(sql, con);

            try
            {
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count;
            }
            catch
            {
                return 0;
            }
            finally
            {
                con.Close();
                con.Dispose();
                cmd.Dispose();
            }
        }
        #endregion

        #region  过滤用户名中的非法字符
        /// <summary>
        /// 过滤用户名中的非法字符
        /// </summary>
        /// <param name="str">要被过滤的字符串</param>
        /// <returns>返回String类型的过滤后的字符串</returns>
        public string NameReplace(string str)
        {
            str = str.Trim();
            str = str.Replace("=", "");
            str = str.Replace("'", "");
            return str;
        }
        #endregion
    }
}
