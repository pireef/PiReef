using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using System.IO.Ports;

namespace PiReefComplete
{
    public class SensorValue
    {
        private static String connectionString = @"server=localhost;userid=pireef;
            password=123pireef;database=PIREEF";
        public string Id { get; set; }
        public string SensorName { get; set; }
        public double WaterTemp { get; set; }
        public double AirTemp { get; set; }
        public DateTime Timestamp { get; set; }
        public double PH { get; set; }
        public double Salinity { get; set; }
        private PHUart PHDevice { get; set; }
        private double WaterTempC { get; set; }
        //SerialPort port { get; set; }

        public SensorValue()
        {
            PHDevice = new PHUart();

        }
        /// <summary>
        /// Sends the current data to the database for storage.  
        /// </summary>
        /// 
        public void SendtoDb()
        {
            MySqlConnection cn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = cn;

            try
            {
                cn.Open();
                cmd.CommandText = "INSERT INTO sensorvalue (sensorname, WaterTemp, AirTemp, TimeStamp, PH, Salinity, ID) VALUES(@sensorname, @WaterTemp, @AirTemp, @TimeStamp, @PH, @Salinity, @ID)";
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@sensorname", this.SensorName);
                cmd.Parameters.AddWithValue("@WaterTemp", this.WaterTemp);
                cmd.Parameters.AddWithValue("@AirTemp", this.AirTemp);
                cmd.Parameters.AddWithValue("@TimeStamp", this.Timestamp);
                cmd.Parameters.AddWithValue("@PH", this.PH);
                cmd.Parameters.AddWithValue("@Salinity", this.Salinity);
                cmd.Parameters.AddWithValue("@ID", this.Id);
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

        /// <summary>
        /// Calls functions to read the data from the sensors. 
        /// </summary>
       
        public void UpdateSensors()
        {
            this.AirTemp = GetTemperature(0, true);
            this.WaterTemp = GetTemperature(1, true);
            if(this.PHDevice.ReadPh(WaterTempC))
            {
                //again this shoul be a valid result.
                //if this fails the value just stays what it was the last time. 
                this.PH = PHDevice.PHReading;
            }
            //keep adding other sensors here. 
            this.Timestamp = DateTime.Now;
            this.SensorName = "The Pi";
            this.Id = RandomString(32);
        }

        /*public String GetPH()
            
        {
            if (!port.IsOpen)
                port.Open();

            String response = port.ReadExisting();
            if (response.EndsWith("\r"))
            {
                return response;
            }
            else
            {
                return "Data Incomplete";
            }
        }*/
        /// <summary>
        /// Generates a random character string for an unique ID.  
        /// Shamefully copied from the web. 
        /// </summary>
        /// <param name="length">How long do we want the string to be.</param>
        /// <returns>Returns a randomly generated string.</returns>
        
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Reads the output files from the Temp probes.  
        /// </summary>
        /// <param name="sensorIndex">Which probe do we want to read.</param>
        /// <param name="convertToF">True to convert to Ferenheit, false to leave as Celsius</param>
        /// <returns>A double representing the current temperature of that probe</returns>
        /// 

        double GetTemperature(int sensorIndex, bool convertToF)
        {
            DirectoryInfo devicesDir = new DirectoryInfo("/sys/bus/w1/devices");
            var deviceDir = devicesDir.GetDirectories("28*")[sensorIndex];
            using (TextReader reader =
                new System.IO.StreamReader(deviceDir.FullName + "/w1_slave"))
            {
                string w1slavetext = reader.ReadToEnd();

                string temptext =
                    w1slavetext.Split(new string[] { "t=" },
                    StringSplitOptions.RemoveEmptyEntries)[1];

                double temp = double.Parse(temptext) / 1000;
                WaterTempC = temp;
                if (convertToF)
                {
                    temp = temp * 1.8 + 32;
                }

                return temp;
            }
        }


        /// <summary>
        /// Updates the class's members to the most current stored in the database. 
        /// Used by the window apps to get and display data. 
        /// </summary>
        /// 

        public void ReadMostCurrentDataFromDB()
        {
            MySqlConnection cn = new MySqlConnection(connectionString);
            MySqlDataReader rdr = null;

            //This one may be faster with the TimeStamp column indexed...wait and see once more records are there. 
            //string sel = "SELECT sensorname, WaterTemp, AirTemp, TimeStamp, PH, Salinity, ID from sensorvalue WHERE TimeStamp = (select Max(TimeStamp) from sensorvalue);"
            string sel = "SELECT sensorname, WaterTemp, AirTemp, TimeStamp, PH, Salinity, ID FROM sensorvalue ORDER BY TimeStamp DESC limit 1";

            try
            {
                cn.Open();
                MySqlCommand cmd = new MySqlCommand(sel, cn);

                rdr = cmd.ExecuteReader();
                
                while(rdr.Read())
                {
                    this.SensorName = rdr.GetString(0);
                    this.WaterTemp = rdr.GetDouble(1);
                    this.AirTemp = rdr.GetDouble(2);
                    this.Timestamp = rdr.GetDateTime(3);
                    this.PH = rdr.GetDouble(4);
                    this.Salinity = rdr.GetDouble(5);
                    this.Id = rdr.GetString(6);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (rdr != null)
                    rdr.Close();
                if (cn != null)
                    cn.Close();
            }
        }
    }
}
