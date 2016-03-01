using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Raspberry.IO.GeneralPurpose;
using MySql.Data.MySqlClient;

namespace PiReefComplete
{
    public class fugeOnJob : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            //do our stuff to turn the fuge light on
            var plug2 = ConnectorPin.P1Pin13.ToProcessor();
            var driver = GpioConnectionSettings.DefaultDriver;
            driver.Write(plug2, true);
            MySqlLog.LogData("Quartz Job", "Refugium Light turned on.");
        }
    }

    public class fugeOffJob : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            //do stuff to turn the fuge light off
            var plug2 = ConnectorPin.P1Pin13.ToProcessor();
            var driver = GpioConnectionSettings.DefaultDriver;
            driver.Write(plug2, false);
            MySqlLog.LogData("Quartz Job", "Refugium Light turned off.");
        }
    }

    public class dbMaintJob : IJob
    {
        private static String connectionString = @"server=localhost;userid=pireef;
            password=123pireef;database=PIREEF";

        void IJob.Execute(IJobExecutionContext context)
        {
            MySqlConnection cn = new MySqlConnection(connectionString);
            MySqlCommand cmdSensors = new MySqlCommand();
            MySqlCommand cmdLog = new MySqlCommand();

            cmdSensors.Connection = cn;
            cmdLog.Connection = cn;
            try
            {
                cn.Open();
                cmdSensors.CommandText = "DELETE from sensorvalue where TimeStamp < DATE_SUB(NOW() , INTERVAL 6 MONTH)";
                cmdLog.CommandText = "DELETE from log where TimeStamp < DATE_SUB(Now(), INTERVAL 6 MONTH)";
                cmdSensors.Prepare();
                cmdLog.Prepare();
                cmdSensors.ExecuteNonQuery();
                cmdLog.ExecuteNonQuery();
            }
            catch(MySqlException ex)
            {
                Console.WriteLine("DB Maintenance Job Failed. " + ex.ToString());
            }
            finally
            {
                if (cn != null)
                    cn.Close();
                MySqlLog.LogData("Quartz Job", "Database maitenance complete.");
            }
        }
    }
}
