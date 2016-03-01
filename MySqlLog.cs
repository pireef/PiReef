using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace PiReefComplete
{
	/// <summary>
	/// Helper class for writing log messages to a database. 
	/// </summary>
    static class MySqlLog
    {
    	/// <summary>
    	/// Writes data to the log.  
    	/// </summary>
    	/// <param name="module">A string representing what module </param>
    	/// <param name="message">A string representing what message we are writing</param>
        public static void LogData(String module, String message )
        {
            String cnString = @"server=localhost;userid=pireef;
            password=123pireef;database=PIREEF";
            string cmdText = "INSERT INTO log (TimeStamp, Module, Message) VALUES (@Time, @mod, @mes)";

            MySqlConnection cn = new MySqlConnection(cnString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cn;

            try
            {
                cn.Open();
                cmd.CommandText = cmdText;
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@mod", module);
                cmd.Parameters.AddWithValue("@mes", message);
                cmd.Parameters.AddWithValue("@Time", DateTime.Now);
                cmd.ExecuteNonQuery();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            finally
            {
                if (cn != null)
                    cn.Close();
            }
        }
    }
}
