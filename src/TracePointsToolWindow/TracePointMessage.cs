using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TracePointsToolWindow
{
    public class TracePointMessage
    {
        public DateTime Timestamp
        {
            get;
            set;
        }
        public double DeltaTimeMs
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }
        public string DisplayTime
        {
            get;
            set;
        }

        public TracePointMessage(DateTime Timestamp, double DeltaTime, string message)
        {
            this.Timestamp = Timestamp;
            this.DeltaTimeMs = DeltaTime;
            this.DisplayTime = String.Format("+{0:0,0}", this.DeltaTimeMs);
            this.Message = message;
        }

        public bool IsMatch(string Filter)
        {
            if (Message.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }
    }
}
