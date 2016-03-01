using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
//using Window_App;

namespace PiReefComplete
{
    class PHUart
    {
        private SerialPort port
        {
            get; set;
        }
        public double PHReading { get; set;  }

        public PHUart()
        {
            port = new SerialPort("/dev/ttyAMA0", 9600);
            if (!port.IsOpen)
                port.Open();
            port.Write("\r");
            port.Write("L,1\r");
            port.Write("C,1\r");
            //not sure if it's smart to "close" the port since we are going to constantly need it. 
            //port.Close();
			MySqlLog.LogData ("PHUart Class", "Port Open, settings sent to board.");
        }

        public bool ReadPh(double currentTemp)
        {
            string response = "";
            double truevalue;

            if (!port.IsOpen)
                port.Open();

            response = port.ReadExisting();
            //temp compensate for the next reading.  
            port.Write("t," + currentTemp.ToString() + "/r");
            if(response.EndsWith("\r"))
            {
                //ok so we received a complete line of data.
                //take out the extra crap we don't need and will cause failures (most) of the TryParse.
                string stripped = new String(response.Take(5).ToArray());
                if (Double.TryParse(stripped, out truevalue))
                {
                    //Ok so the parse succeeded, meaning we have a valid number from the sensor. 
                    PHReading = truevalue;
                    return true;
                }
                else
                {
                    //This means the parse failed, in most cases the sensor responded with some response code....which we may disable. 
                    //but we need to find a way to elequently handle response messages should they occur.   
                    PHReading = 99.0;
  //                  MySqlLog.LogData("PH", "Parsing value failed, value passed in is: " + response);
                    return false;
                }
                //port.Close();
            }
            else
            {
                PHReading = 99.9;
                return false;
                //port.Close();
            }
        }
    }
}
