using NavSat.Core.Abstrations.Utils;
using System;
using System.Collections.Generic;

namespace NavSat.Core.Utils {
    /// <summary>
    /// This class manages GPS time.
    /// </summary>
    public class GPSTime : IGPSTime {
        /// <summary>
        /// Seconds since jan 6, 1980
        /// </summary>
        private decimal m_RogueTime;

        /// <summary>
        /// Seconds per day
        /// </summary>
        private const decimal DAYSECS = 86400;
        /// <summary>
        /// Seconds per week
        /// </summary>
        private const decimal WEEKSECS = 604800;
        /// <summary>
        /// Difference of days between gps time and julian date
        /// </summary>
        private const decimal MJDDAYS = 44244;
        /// <summary>
        /// See Numerical Recipes function "caldat". This number has someting to do with the
        /// day when the gregorian calendar was adopted. 
        /// </summary>
        private const int IGREG = 2299161;

        private const int SIGNIFICANT_DIGITS = 18;

        /// <summary>
        /// Static constructor used to initialize the leap seconds table.
        /// </summary>
        static GPSTime() {
            InitializeLeapSecondsTable();
        }

        /// <summary>
        /// Creates reference GPS time.
        /// Use it to find out what is the DateTime object for this instance.
        /// </summary>
        public static GPSTime Empty {
            get { return new GPSTime(0m); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="GPSweek">GPS week.</param>
        /// <param name="GPSseconds">GPS second.</param>
        public GPSTime(int GPSweek, double GPSseconds) {
            if (double.IsNaN(GPSseconds) || GPSseconds < 0)
                throw new ArgumentException("Parameter is invalid", "seconds");
            if (GPSweek < 0)
                throw new ArgumentException("Parameter is invalid", "week");

            m_RogueTime = GPSweek;
            m_RogueTime = Decimal.Multiply(m_RogueTime, WEEKSECS);
            m_RogueTime = Decimal.Add(m_RogueTime, (decimal)GPSseconds);
        }

        /// <summary>
        /// Constructs a new time object with GPS rogue time.
        /// </summary>
        /// <param name="rogueTime">GPS rogue time (absolute seconds since 06.01.1980)</param>
        public GPSTime(decimal rogueTime) {
            if (rogueTime < 0)
                throw new ArgumentException();
            m_RogueTime = rogueTime;
        }

        /// <summary>
        /// Constructor uses a calendar time based on the GPS timeframe to initialize 
        /// this instance.
        /// </summary>
        /// <param name="GPSyear">Year</param>
        /// <param name="GPSmonth">Month</param>
        /// <param name="GPSday">Day</param>
        /// <param name="GPShour">Hour</param>
        /// <param name="GPSminute">Minutes</param>
        /// <param name="GPSseconds">Seconds</param>
        public GPSTime(int GPSyear, int GPSmonth, int GPSday, int GPShour, int GPSminute, double GPSseconds) {
            Set(GPSyear, GPSmonth, GPSday, GPShour, GPSminute, GPSseconds, false);
        }

        /// <summary>
        /// Constructor uses a calendar time based on the GPS or UTC timeframe to initialize 
        /// this instance.
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <param name="day">Day</param>
        /// <param name="hour">Hour</param>
        /// <param name="minute">Minutes</param>
        /// <param name="seconds">Seconds</param>
        /// <param name="utc">Flag indicating whether the given calendar time is based on UTC 
        /// or GPS timeframe. If this is set true and UTC time is given, the constructor shifts
        /// the time automatically for the actual leap seconds between GPS time.</param>
        public GPSTime(int year, int month, int day, int hour, int minute, double seconds, bool utc) {
            Set(year, month, day, hour, minute, seconds, utc);
        }

        /// <summary>
        /// Constructs a new instance by using a UTC datetime reference. Please be aware that the class
        /// DateTime just has a resolution of full milliseconds and might not be significant enough
        /// for precise time calculations and comparison.
        /// </summary>
        /// <param name="UTCTime">DateTime reference representing a valid UTC time object.</param>
        public GPSTime(DateTime UTCTime) {
            double seconds = (double)UTCTime.Second;
            if (UTCTime.Millisecond > 0)
                seconds += ((double)UTCTime.Millisecond) / 1000;

            Set(UTCTime.Year, UTCTime.Month, UTCTime.Day, UTCTime.Hour, UTCTime.Minute, seconds, true);
        }

        /// <summary>
        /// Gets the <see cref="DateTime"/> value for this instance. This time is always shifted by
        /// actual number of leap seconds between GPS and UTC time and belongs to the UTC timeframe.
        /// </summary>
        public DateTime DateTimeUTC {
            get {
                int year, month, day, hour, minute;
                double roundedSeconds;
                GetTimeGPS(out year, out month, out day, out hour, out minute, out roundedSeconds, 3);

                int seconds = (int)System.Math.Floor(roundedSeconds);
                int milliseconds = (int)System.Math.Round((roundedSeconds - seconds) * 1000.0);

                DateTime dateTime = new DateTime(year, month, day, hour, minute, seconds, milliseconds, DateTimeKind.Utc);

                // Strictly spoken we have to call GetLeapSeconds with a UTC time.
                // Here we are calling it with a a GPS time. 
                int leapSeconds = GetLeapSeconds(dateTime);
                dateTime = dateTime.AddSeconds(-leapSeconds);

                // Test to see if the new datetime has a different leap seconds.
                if (leapSeconds != GetLeapSeconds(dateTime)) {
                    // We must be on a boundary, adjust for the difference.
                    dateTime = dateTime.AddSeconds(-(GetLeapSeconds(dateTime) - leapSeconds));
                }

                return dateTime;
            }
        }

        /// <summary>
        /// Conversion to calendar time. No adjustment for leap seconds applied (it's simple GPS
        /// calendar time). The seconds are rounded to the specified number of decimals.
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <param name="day">Day</param>
        /// <param name="hour">Hour</param>
        /// <param name="minute">Minute</param>
        /// <param name="roundedSeconds">Seconds rounded</param>
        /// <param name="numberOfDecimals">The number of decimals to be rounded</param>
        public void GetTimeGPS(out int year, out int month, out int day, out int hour,
            out int minute, out double roundedSeconds, int numberOfDecimals) {
            GetTimeGPS(out year, out month, out day, out hour, out minute, out roundedSeconds);
            roundedSeconds = System.Math.Round(roundedSeconds, numberOfDecimals);

            if (roundedSeconds >= 60) {
                // We are here if the seconds of this GPSTime are in [59.5, 60.0[.
                // Rounding can lead to 60.0.
                // Adding 0.6 shifts if to [60.1, 60.6[, which is definitely in the next minute.
                GPSTime roundedTime = new GPSTime(Decimal.Add(RogueTime, new Decimal(0.6)));
                roundedTime.GetTimeGPS(out year, out month, out day, out hour, out minute, out roundedSeconds);
                roundedSeconds = 0;
            }
        }

        /// <summary>
        /// Conversion to calendar time. No adjustment for leap seconds applied (it's simple GPS
        /// calendar time). Mathematically precise function.
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <param name="day">Day</param>
        /// <param name="hour">Hour</param>
        /// <param name="minute">Minute</param>
        /// <param name="seconds">Seconds</param>
        public void GetTimeGPS(out int year, out int month, out int day, out int hour,
            out int minute, out double seconds) {
            // This function was ported from the function "caldat" from the book
            // "Numerical Recipes in C"

            int julian; // julian day
            int ja, jalpha, jb, jc, jd, je;

            int days = Decimal.ToInt32(Decimal.Divide(this.m_RogueTime, DAYSECS));
            decimal mjd = decimal.Add(MJDDAYS, (decimal)days);

            julian = Decimal.ToInt32(mjd);
            decimal secondsOfDay = Decimal.Subtract(this.m_RogueTime, Decimal.Multiply((decimal)days, DAYSECS));

            julian += 2400001;
            if (julian >= IGREG) {
                jalpha = (int)(((double)(julian - 1867216) - 0.25) / 36524.25);
                ja = julian + 1 + jalpha - (int)(0.25 * jalpha);
            }
            else
                ja = julian;
            jb = ja + 1524;
            jc = (int)(6680.0 + ((double)(jb - 2439870L) - 122.1) / 365.25);
            jd = 365 * jc + (int)(0.25 * (double)jc);
            je = (int)((double)(jb - jd) / 30.6001);
            int id = (int)(jb - jd) - (int)(30.6001 * je);
            int mm = (int)je - 1;
            if (mm > 12)
                mm -= 12;
            int iyyy = (int)jc - 4715;
            if (mm > 2)
                --(iyyy);
            if (iyyy <= 0)
                --(iyyy);

            year = iyyy;
            month = mm;
            day = id;

            hour = (int)(secondsOfDay / 3600.0m);
            minute = (int)((secondsOfDay - ((decimal)hour * 3600.0m)) / 60.0m);
            seconds = (double)(secondsOfDay % 60.0m);
        }

        /// <summary>
        /// Gets GPS week.
        /// </summary>
        public int Week {
            get {
                return Decimal.ToInt32(Decimal.Divide(m_RogueTime, WEEKSECS));
            }
        }

        /// <summary>
        /// Gets GPS seconds.
        /// </summary>
        public double Seconds {
            get {
                int iWeek = Week;
                decimal dSeconds = Decimal.Subtract(m_RogueTime, Decimal.Multiply(iWeek, WEEKSECS));
                return Decimal.ToDouble(dSeconds);
            }
        }

        /// <summary>
        /// Gets GPS rogue time
        /// </summary>
        public decimal RogueTime {
            get { return m_RogueTime; }
        }

        /// <summary>
        /// Overrides method object.Equals.
        /// </summary>
        /// <param name="obj">Object to be checked for equality.</param>
        /// <returns>True if the objects are equal, otherwise false.</returns>
        public override bool Equals(object obj) {
            if (obj == null)
                return false;

            GPSTime rhs = obj as GPSTime;
            if (rhs == null)
                return false;

            // If you change something here remember to update GetHashCode() !
            if (Decimal.Compare(Decimal.Round(this.RogueTime, SIGNIFICANT_DIGITS),
                Decimal.Round(rhs.RogueTime, SIGNIFICANT_DIGITS)) != 0)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the hash code of the instance. The rouge time of the
        /// object will be used to calculate a proper hash.
        /// </summary>
        /// <returns>A hash code for this time object.</returns>
        public override int GetHashCode() {
            // This is ok because m_RogueTime is in fact a readonly field
            return Decimal.Round(m_RogueTime, SIGNIFICANT_DIGITS).GetHashCode();
        }

        /// <summary>
        /// Comparison operator used to determine if both objects are equals.
        /// </summary>
        /// <param name="t1">The first time object.</param>
        /// <param name="t2">The second time object.</param>
        /// <returns>True if both time objects are equals, false if different.</returns>
        static public bool operator ==(GPSTime t1, GPSTime t2) {
            if (object.Equals(t1, null))
                return object.Equals(t2, null);

            return t1.Equals(t2);
        }

        /// <summary>
        /// Comparison operator used to determine if both objects are different.
        /// </summary>
        /// <param name="t1">The first time object.</param>
        /// <param name="t2">The second time object.</param>
        /// <returns>True if both time objects are different, false if equals.</returns>
        static public bool operator !=(GPSTime t1, GPSTime t2) {
            if (object.Equals(t1, null))
                return !object.Equals(t2, null);

            return !t1.Equals(t2);
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an indication 
        /// of their relative values.
        /// </summary>
        /// <param name="obj">An object to compare, or a null reference</param>
        /// <returns>A signed number indicating the relative values of this
        /// instance and value.</returns>
        public int CompareTo(object obj) {
            GPSTime rhs = obj as GPSTime;

            if (rhs == null)
                // One or both comparands can be a null reference.
                // By definition, any string, including the empty string (""),
                // compares greater than a null reference;
                // and two null references compare equal to each other.
                return 1;

            return Decimal.Compare(Decimal.Round(this.RogueTime, SIGNIFICANT_DIGITS),
                Decimal.Round(rhs.RogueTime, SIGNIFICANT_DIGITS));
        }

        /// <summary>
        /// Comparison operator used to check if the first object is earlier
        /// then the second object. This function calls CompareTo.
        /// </summary>
        /// <param name="t1">The first time object.</param>
        /// <param name="t2">The second time object.</param>
        /// <returns>True if the first time object is earlier then the 
        /// second time object.</returns>
        static public bool operator <(GPSTime t1, GPSTime t2) {
            // One or both comparands can be a null reference.
            // By definition, any string, including the empty string (""),
            // compares greater than a null reference;
            // and two null references compare equal to each other.
            if (t1 == null)
                return t2 != null;

            return t1.CompareTo(t2) < 0;
        }

        /// <summary>
        /// Comparison operator used to check if the first object has later 
        /// time then the second object. This function calls CompareTo.
        /// </summary>
        /// <param name="t1">The first time object.</param>
        /// <param name="t2">The second time object.</param>
        /// <returns>True if the first time object has later time then the 
        /// second time object.</returns>
        static public bool operator >(GPSTime t1, GPSTime t2) {
            // One or both comparands can be a null reference.
            // By definition, any string, including the empty string (""),
            // compares greater than a null reference;
            // and two null references compare equal to each other.
            if (t1 == null)
                return false;

            return t1.CompareTo(t2) > 0;
        }

        /// <summary>
        /// Comparison operator used to check if the first object is earlier or 
        /// equal then the second object. This function calls CompareTo.
        /// </summary>
        /// <param name="t1">The first time object.</param>
        /// <param name="t2">The second time object.</param>
        /// <returns>True if the first time object is earlier or equal then the 
        /// second time object.</returns>
        static public bool operator <=(GPSTime t1, GPSTime t2) {
            // One or both comparands can be a null reference.
            // By definition, any string, including the empty string (""),
            // compares greater than a null reference;
            // and two null references compare equal to each other.
            if (t1 == null)
                return true;

            return t1.CompareTo(t2) <= 0;
        }

        /// <summary>
        /// Comparison operator used to check if the first object has later or 
        /// equal time then the second object. This function calls CompareTo.
        /// </summary>
        /// <param name="t1">The first time object.</param>
        /// <param name="t2">The second time object.</param>
        /// <returns>True if the first time object has later or equal time then the 
        /// second time object.</returns>
        static public bool operator >=(GPSTime t1, GPSTime t2) {
            // One or both comparands can be a null reference.
            // By definition, any string, including the empty string (""),
            // compares greater than a null reference;
            // and two null references compare equal to each other.
            if (t1 == null)
                return t2 == null;

            return t1.CompareTo(t2) >= 0;
        }


        #region Private members

        private void Set(int year, int month, int day, int hour, int minute, double seconds, bool utc) {
            if (month < 1 || month > 12 || day < 1 || day > 31 || hour < 0 || hour > 23
                || minute < 0 || minute > 59 || seconds < 0.0 || seconds >= 60.0)
                throw new ArgumentException();
            decimal julianSeconds = cal2mjl(year, month, day, hour, minute, seconds);
            m_RogueTime = Decimal.Subtract(julianSeconds, Decimal.Multiply(MJDDAYS, DAYSECS));

            if (utc) {
                // Compute the DateTime bases on the given UTC information
                int iSeconds = (int)seconds;
                int mSeconds = (int)(seconds - iSeconds) * 1000;
                DateTime dt = new DateTime(year, month, day, hour, minute, (int)seconds, mSeconds);

                // Adjust the time by the leap seconds
                m_RogueTime = Decimal.Add(m_RogueTime, GetLeapSeconds(dt));
            }
        }

        /// <summary>
        /// Returns the julian seconds. That means the julian days, multiplied by 86400.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="secs"></param>
        /// <returns></returns>
        private static decimal cal2mjl(int year, int month, int day, int hour, int minute, double secs) {
            // This function was ported from the function "julday" from the book
            // "Numerical Recipes in C"

            // In this function, only year and month have to be in a suitable range.
            // Day, hour minute, seconds can be negative or whatever.

            if (month > 2)
                month++;
            else {
                month += 13;
                year--;
            }

            int jul = 365 * year + year / 4 + 306 * month / 10
                - 679004 - year / 100 + year / 400;

            decimal mjd = Decimal.Add(jul, day); // now mjd contains the julian day

            mjd = decimal.Multiply(mjd, DAYSECS); // convert to julian seconds

            // Add the seconds of the day
            decimal secondsOfDay = (((decimal)hour) * 60.0m + ((decimal)minute)) * 60.0m + ((decimal)secs);
            mjd = decimal.Add(mjd, secondsOfDay);

            return mjd;
        }


        #endregion

        #region Private helper code for LeapSeconds
        private static Dictionary<DateTime, int> s_gpsLeapSeconds = null;
        private static void InitializeLeapSecondsTable() {
            if (s_ReverseDateTimeComparable == null)
                s_ReverseDateTimeComparable = new ReverseDateTimeComparable();

            s_gpsLeapSeconds = new Dictionary<DateTime, int>();

            s_gpsLeapSeconds.Add(new DateTime(1980, 1, 6), 0);
            s_gpsLeapSeconds.Add(new DateTime(1981, 7, 1), 1);
            s_gpsLeapSeconds.Add(new DateTime(1982, 7, 1), 2);
            s_gpsLeapSeconds.Add(new DateTime(1983, 7, 1), 3);
            s_gpsLeapSeconds.Add(new DateTime(1985, 7, 1), 4);
            s_gpsLeapSeconds.Add(new DateTime(1988, 1, 1), 5);
            s_gpsLeapSeconds.Add(new DateTime(1990, 1, 1), 6);
            s_gpsLeapSeconds.Add(new DateTime(1991, 1, 1), 7);
            s_gpsLeapSeconds.Add(new DateTime(1992, 7, 1), 8);
            s_gpsLeapSeconds.Add(new DateTime(1993, 7, 1), 9);
            s_gpsLeapSeconds.Add(new DateTime(1994, 7, 1), 10);
            s_gpsLeapSeconds.Add(new DateTime(1996, 1, 1), 11);
            s_gpsLeapSeconds.Add(new DateTime(1997, 7, 1), 12);
            s_gpsLeapSeconds.Add(new DateTime(1999, 1, 1), 13);
            s_gpsLeapSeconds.Add(new DateTime(2006, 1, 1), 14);
            s_gpsLeapSeconds.Add(new DateTime(2009, 1, 1), 15);

            //Added by HermanR:
            s_gpsLeapSeconds.Add(new DateTime(2012, 7, 1), 16);
            s_gpsLeapSeconds.Add(new DateTime(2015, 7, 1), 17);
            s_gpsLeapSeconds.Add(new DateTime(2016, 1, 1), 18);
        }



        /// <summary>
        /// Gets the number of leap seconds for the given UTC date
        /// </summary>
        /// <param name="dateTimeUTC">The UTC date to retrieve the leap seconds.</param>
        /// <returns>Returns the number of leap seconds for the given date.</returns>
        public static int GetLeapSeconds(DateTime dateTimeUTC) {
            foreach (KeyValuePair<DateTime, int> de in s_gpsLeapSeconds) {
                if (s_ReverseDateTimeComparable.Compare(dateTimeUTC, de.Key) <= 0)
                    return (int)de.Value;
            }

            return 0;
        }

        private static ReverseDateTimeComparable s_ReverseDateTimeComparable = null;
        private class ReverseDateTimeComparable : IComparer<DateTime> {
            public ReverseDateTimeComparable() {
            }

            #region IComparer<DateTime> Members

            public int Compare(DateTime x, DateTime y) {
                return DateTime.Compare(x, y) * -1;
            }

            #endregion
        }
        #endregion

    }
}
