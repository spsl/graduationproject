using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace JieDianClient
{
    class ConMysql
    {
        public static Boolean con(String sql)
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
                MessageBox.Show("∂¡»° ß∞‹£°");
            }
            finally
            {
                reader.Close();
                       
            }return false;
        }

    }
}
