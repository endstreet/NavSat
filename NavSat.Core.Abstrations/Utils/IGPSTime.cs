using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavSat.Core.Abstrations.Utils {
    public interface IGPSTime : IComparable {
        DateTime DateTimeUTC { get; }
        decimal RogueTime { get; }
        double Seconds { get; }
        int Week { get; }

        void GetTimeGPS(out int year, out int month, out int day, out int hour, out int minute, out double seconds);
        void GetTimeGPS(out int year, out int month, out int day, out int hour, out int minute, out double roundedSeconds, int numberOfDecimals);
    }
}
