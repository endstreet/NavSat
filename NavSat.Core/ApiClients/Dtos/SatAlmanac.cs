namespace NavSat.Core.ApiClients.Dtos {
    public class SatAlmanac {

        /*
            <SatAlmanac>
                <AscensionRate>-0.0004545</AscensionRate>
                <Eccentricity>0.004511</Eccentricity>
                <GPSWeek>1854</GPSWeek>
                <Health>0</Health>
                <Inclination>1.1694</Inclination>
                <MeanAnomaly>-18.028</MeanAnomaly>
                <Perigee>24.111</Perigee>
                <RightAscension>-167.908</RightAscension>
                <SVN>1</SVN>
                <SatClockDrift>0</SatClockDrift>
                <SatClockOffset>0</SatClockOffset>
                <SemiMajorAxis>5153.6</SemiMajorAxis>
                <TimeOfAlmanac>405504</TimeOfAlmanac>
            </SatAlmanac>
        */

        public double AscensionRate { get; set; }

        public double Eccentricity { get; set; }

        public int GpsWeek { get; set; }

        public int Health { get; set; }

        public double Inclination { get; set; }

        public double MeanAnomaly { get; set; }

        public double Perigee { get; set; }

        public double RightAscension { get; set; }

        public int SVN { get; set; }

        public double SatClockDrift { get; set; }

        public double SatClockOffset { get; set; }

        public double SemiMajorAxis { get; set; }

        public double TimeOfAlmanac { get; set; }

    }
}
