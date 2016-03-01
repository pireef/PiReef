using System;
using Raspberry.IO.GeneralPurpose;
using System.IO.Ports;
using Quartz;
using Quartz.Impl;
using MySql.Data.MySqlClient;

namespace PiReefComplete
{
	public class Control
	{	
		ISchedulerFactory factory;
		IScheduler scheduler;
		IJobDetail fugeOn;
		IJobDetail fugeOff;
		IJobDetail DbMaint;
		ITrigger DbMaintTrigger;
		ITrigger fugeonTrigger;
		ITrigger fugeoffTrigger;	
		bool bMainPump { get; set; }
		bool bPowerHead { get; set; }
		bool bSkimmer { get; set; }
		bool bFuge { get; set; }
		bool bGAC { get; set; }
		double minWaterTemp { get; set; }
		double maxWaterTemp { get; set; }
		Int32 fugeOnHour{ get; set; }
		Int32 fugeOnMin{ get; set; }
		Int32 fugeOffHour{ get; set; }
		Int32 fugeOffMin{ get; set; }
		Int32 DbMaitHour{ get; set; }
		Int32 DbMaitMin{ get; set; }
		public Int32 updateInt { get; set; }
		private static String connectionString = @"server=localhost;userid=pireef;
            password=123pireef;database=PIREEF";
		
		public Control ()
		{
			bMainPump = true;
			bPowerHead = true;
			bSkimmer = true;
			bFuge = false;
			bGAC = true;
			ClearGPIO ();
			LoadSettings ();
			CheckSchedule ();
			BuildSchedule ();
		}

		/// <summary>
		/// Clears all the GPIO pins that we are using so they are in a fresh/default state.
		/// </summary>
		private void ClearGPIO()
		{
			var plug1 = ConnectorPin.P1Pin11.ToProcessor();
			var plug2 = ConnectorPin.P1Pin13.ToProcessor();
			var plug3 = ConnectorPin.P1Pin15.ToProcessor();
			var plug4 = ConnectorPin.P1Pin16.ToProcessor();
			var plug5 = ConnectorPin.P1Pin18.ToProcessor();
			var plug6 = ConnectorPin.P1Pin32.ToProcessor();
			var plug7 = ConnectorPin.P1Pin22.ToProcessor();
			var plug8 = ConnectorPin.P1Pin36.ToProcessor();

			WriteToPins(plug1, true, PinDirection.Output);
			WriteToPins(plug2, bFuge, PinDirection.Output);
			WriteToPins(plug3, true, PinDirection.Output);
			WriteToPins(plug4, true, PinDirection.Output);
			WriteToPins(plug5, bGAC, PinDirection.Output);
			WriteToPins(plug6, bSkimmer, PinDirection.Output);
			WriteToPins(plug7, bPowerHead, PinDirection.Output);
			WriteToPins(plug8, bMainPump, PinDirection.Output);
			MySqlLog.LogData ("Control Class", "GPIO Cleared and reset.");

		}

		/// <summary>
		/// Writes the pin either high or low. 
		/// </summary>
		/// <param name="pin">The pin we are writing too</param>
		/// <param name="status">The status we are writing "True or False" High or Low</param>
		/// <param name="dir">Is the pin an output or input pin</param>
		private void WriteToPins(ProcessorPin pin, bool status, PinDirection dir)
		{ 
			var driver = GpioConnectionSettings.DefaultDriver;

			driver.Allocate(pin, dir);
			driver.Write(pin, status);
		}

		/// <summary>
		/// Loads all the settings (times, min and max values, update intervals, etc from the database.  
		/// </summary>
		private void LoadSettings()
		{
			MySqlConnection cn = new MySqlConnection(connectionString);
			MySqlDataReader rdr = null;
			string selStr = "SELECT settingname, value FROM settings";

			try
			{
				cn.Open();
				MySqlCommand cmd = new MySqlCommand(selStr, cn);
				rdr = cmd.ExecuteReader();

				while (rdr.Read())
				{
					switch(rdr.GetString(0))
					{
					//All values in this table are VarChar, so we read the string in, then convert it to what we need. 
					case "minWaterTemp":
						minWaterTemp = Convert.ToDouble(rdr.GetString(1));
						break;
					case "maxWaterTemp":
						maxWaterTemp = Convert.ToDouble(rdr.GetString(1));
						break;
					case "fugeOnHour":
						fugeOnHour = Convert.ToInt32(rdr.GetString(1));
						break;
					case "fugeOnMin":
						fugeOnMin = Convert.ToInt32(rdr.GetString(1));
						break;
					case "fugeOffHour":
						fugeOffHour = Convert.ToInt32(rdr.GetString(1));
						break;
					case "fugeOffMin":
						fugeOffMin = Convert.ToInt32(rdr.GetString(1));
						break;
					case "DbMaintHour":
						DbMaitHour = Convert.ToInt32(rdr.GetString(1));
						break;
					case "DbMaintMin":
						DbMaitMin = Convert.ToInt32(rdr.GetString(1));
						break;
					case "updateInt":
						updateInt = Convert.ToInt32(rdr.GetString(1));
						break;
					}
				}
			}
			catch (MySqlException ex)
			{
				//MessageBox.Show(ex.ToString());
			}
			finally
			{
				if (rdr != null)
					rdr.Close();
				if (cn != null)
					cn.Close();
				MySqlLog.LogData ("Control Class", "Settings loaded successfully.");
			}
		}

		/// <summary>
		/// Checks to see if the time passed in, is between the start or end times passed in.
		/// This is useful when starting the application and we need the system to pick up the schedule
		/// in the middle of the day without the Quartz job firing.  
		/// </summary>
		/// <param name="datetime">The current Time</param>
		/// <param name="start">The start time</param>
		/// <param name="end">The end time</param>
		/// <returns>True or false</returns>
		private bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
		{
			// convert datetime to a TimeSpan
			TimeSpan now = datetime.TimeOfDay;
			// see if start comes before end
			if (start < end)
				return start <= now && now <= end;
			// start is after end, so do the inverse comparison
			return !(end < now && now < start);
		}

		/// <summary>
		/// Checks the schedule of all the pins that are set to a Quartz Job and picks up the schedule without that Quartz Job
		/// firing. 
		/// </summary>
		private void CheckSchedule()
		{
			TimeSpan tsStart = new TimeSpan(fugeOnHour, fugeOnMin, 0);
			TimeSpan tsEnd = new TimeSpan(fugeOffHour, fugeOffMin, 0);

			var plug2 = ConnectorPin.P1Pin13.ToProcessor();

			if (TimeBetween(DateTime.Now, tsStart, tsEnd))
			{
				WriteToPins(plug2, true, PinDirection.Output);
				bFuge = true;
			}
			else
			{
				WriteToPins(plug2, false, PinDirection.Output);
				bFuge = false;
			}
			MySqlLog.LogData ("Control Class", "Schedule set, pins adjusted.");
		}

		/// <summary>
		/// Checks the various values from the Sensors and sets the pins on the plugs appropiately. 
		/// </summary>
		/// <param name="sv">The current SensorValue class with the most current data</param>
		public void CheckValues (SensorValue sv)
		{
			var plug1 = ConnectorPin.P1Pin11.ToProcessor();

			//Check our min and max values against the latest data
			if (sv.WaterTemp <= minWaterTemp)
			{
				//water is below our min, turn on the heater, format the text to be blue
				WriteToPins(plug1, true, PinDirection.Output);
				//Console.WriteLine ("Heater On");
				//lblWaterTemp.ForeColor = Color.Blue;
				//lblHeatStatus.Text = "Heater On";
				//lblHeatStatus.ForeColor = Color.Blue;
				//MySqlLog.LogData("CheckValues", "Heater has been turned on.");
			}
			else if (sv.WaterTemp >= maxWaterTemp)
			{
				//temp is over our max, change color, turn a fan on etc.
				WriteToPins(plug1, false, PinDirection.Output);
				//Console.WriteLine ("Heater off, Fan on");
				//lblWaterTemp.ForeColor = Color.Red;
				//lblHeatStatus.Text = "Heater Off, Fan on";
				//lblHeatStatus.ForeColor = Color.Red;
				//MySqlLog.LogData("CheckValues", "Heater off, fan on.");
			}
			else
			{
				//everything is good
				WriteToPins(plug1, false, PinDirection.Output);
				//lblWaterTemp.ForeColor = Color.Black;
				//lblHeatStatus.Text = " ";
			}

			//Check for latest data, if it's more than 5 min we should worry...
			//TimeSpan span = DateTime.Now - sv.Timestamp;
			//if (span.TotalMinutes > 5 )
			//{
			//We're not getting recent data from the database
			//Change Text
			//Set our power outlet to the "safe" modes
			//lblDateTime.Text = "NO RECENT DATA RECEIVED";
			//lblDateTime.ForeColor = Color.Red;
			//MySqlLog.LogData("CheckValues", "No recent data receieved from sensors!!");
			//}
			//else
			//{
			//	lblDateTime.ForeColor = Color.Black;
			//}
		}

		/// <summary>
		/// This function builds the Quartz schedule for the various jobs we have scheduled.  
		/// 
		/// These are the current jobs:
		/// Refugium on
		/// Refugium off
		/// DB Maintenance
		/// </summary>
		private void BuildSchedule()
		{
			factory = new StdSchedulerFactory();
			scheduler = factory.GetScheduler();
			scheduler.Start();

			fugeOn = JobBuilder.Create<fugeOnJob>().WithIdentity("FugeOn", "FugeGroup").Build();
			fugeOff = JobBuilder.Create<fugeOffJob>().WithIdentity("FugeOff", "FugeGroup").Build();
			DbMaint = JobBuilder.Create<dbMaintJob>().WithIdentity("DbMaint", "Maintenance").Build();

			fugeonTrigger = TriggerBuilder.Create()
				.WithIdentity("FugeOn", "FugeGroup")
				.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(fugeOnHour, fugeOnMin))
				.ForJob("FugeOn", "FugeGroup")
				.Build();

			fugeoffTrigger = TriggerBuilder.Create()
				.WithIdentity("FugeOff", "FugeGroup")
				.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(fugeOffHour, fugeOffMin))
				.ForJob("FugeOff", "FugeGroup")
				.Build();

			DbMaintTrigger = TriggerBuilder.Create()
				.WithIdentity("DbMaint", "Maintenance")
				.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(DbMaitHour, DbMaitMin))
				.ForJob("DbMaint", "Maintenance")
				.Build();

			scheduler.ScheduleJob(fugeOn, fugeonTrigger);
			scheduler.ScheduleJob(fugeOff, fugeoffTrigger);
			scheduler.ScheduleJob(DbMaint, DbMaintTrigger);

			MySqlLog.LogData ("Control Class", "Quartz jobs built successfully.");

		}
	}
}

