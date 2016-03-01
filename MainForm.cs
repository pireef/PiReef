/*
 * Created by SharpDevelop.
 * User: brett
 * Date: 2/16/2016
 * Time: 5:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using MySql.Data.MySqlClient;

namespace PiReefComplete
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		Control ctrl;
		SensorValue sv;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			MySqlLog.LogData("Application Start.", "Application started.");
			ctrl = new Control();
			sv = new SensorValue();
			
			DataUpdateTimer.Interval = ctrl.updateInt;
			//we do an initial call to get our data, then the timer takes over.
			MySqlLog.LogData("Application Start.", "Getting first set of data.");
			sv.UpdateSensors();
			sv.SendtoDb();
			lblAirTemp.Text = Math.Round(sv.AirTemp, 2).ToString();
			lblWaterTemp.Text = Math.Round(sv.WaterTemp, 2).ToString();
			lblPH.Text = Math.Round(sv.PH, 2).ToString();
			ctrl.CheckValues(sv);

		}
		void DataUpdateTimerTick(object sender, EventArgs e)
		{
			toolStrip.Text = "Updating Sensors...";
			sv.UpdateSensors();
			sv.SendtoDb();
			lblAirTemp.Text = Math.Round(sv.AirTemp, 2).ToString();
			lblWaterTemp.Text = Math.Round(sv.WaterTemp, 2).ToString();
			lblPH.Text = Math.Round(sv.PH, 2).ToString();
			ctrl.CheckValues(sv);
			toolStrip.Text = "Sensors Updated.";
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			var cn = new MySqlConnection(@"server=localhost;userid=pireef;
            password=123pireef;database=PIREEF; Allow User Variables=True;");
			var cmd = new MySqlCommand();
			MySqlDataReader rdr = null;
			
			cmd.Connection = cn;
			//Not exactly a fast query, but it does get us what we want, every 30th record, which is data read every 5 minutes for the last 24
			//hours.
			cmd.CommandText = "select * from (select @row := @row+1 as rownum, TimeStamp, " +
				"AirTemp, WaterTemp, PH, Salinity from (select @row :=0) r, sensorvalue) ranked " +
				"where rownum %30=1 AND TimeStamp > Date_Sub(Now(), Interval 24 hour) order by TimeStamp Asc";
			
			GraphPane pane = graph.GraphPane;
			graph.Visible = true;
			pane.Title.Text = "Last 24 Hours";
			pane.XAxis.Title.Text = "Time";
			pane.YAxis.Title.Text = "Value";
			
			pane.XAxis.Scale.MajorUnit = DateUnit.Hour;
			pane.XAxis.Scale.MinorUnit = DateUnit.Minute;
			pane.XAxis.Scale.Max = new XDate(DateTime.Now.AddHours(24));
			pane.XAxis.Scale.Min = new XDate(DateTime.Now);
			pane.XAxis.Type = AxisType.Date;
			
			var airlist = new RollingPointPairList(300);
			var waterlist = new RollingPointPairList(300);
			var phlist = new RollingPointPairList(300);
			
			try
			{
				cn.Open();
				rdr = cmd.ExecuteReader();
				
				while(rdr.Read())
				{
					double conv = (double) new XDate(rdr.GetDateTime(1));
					
					airlist.Add(conv, rdr.GetDouble(2));
					waterlist.Add(conv, rdr.GetDouble(3));
					phlist.Add(conv, rdr.GetDouble(4));
				}
			}
			catch(MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				if (rdr != null)
                    rdr.Close();
                if (cn != null)
                    cn.Close();
			}
			
			//add data to the list here before adding the curve...i think 
			double xRange = pane.XAxis.Scale.Max - pane.XAxis.Scale.Min;
			pane.XAxis.Scale.Max = new XDate(DateTime.Now);
			pane.XAxis.Scale.Min = pane.XAxis.Scale.Max - xRange;
			
			LineItem airCurve = pane.AddCurve("Air Temp", airlist, Color.Red, SymbolType.None);
			LineItem waterCurve = pane.AddCurve("Water Temp", waterlist, Color.Blue, SymbolType.None);
			//LineItem phcurve = pane.AddCurve("PH", phlist, Color.LimeGreen, SymbolType.None);
						
			graph.AxisChange();
			graph.Invalidate();
			
		}
		void ChartUpdateTimerTick(object sender, EventArgs e)
		{
			GraphPane pane = graph.GraphPane;
			LineItem Aircurve = graph.GraphPane.CurveList[0] as LineItem;
			LineItem Watercurve = graph.GraphPane.CurveList[1] as LineItem;
			
			var airlist = Aircurve.Points as RollingPointPairList;
			var waterlist = Watercurve.Points as RollingPointPairList;
			
			double conv = (double) new XDate(sv.Timestamp);
			
			airlist.Add(conv, sv.AirTemp);
			waterlist.Add(conv, sv.WaterTemp);
			
			double xRange = pane.XAxis.Scale.Max - pane.XAxis.Scale.Min;
			pane.XAxis.Scale.Max = new XDate(DateTime.Now);
			pane.XAxis.Scale.Min = pane.XAxis.Scale.Max - xRange;
			graph.AxisChange();
			graph.Invalidate();
			
			toolStrip.Text = "Chart updated.";
	
		}
		
	}
}
