namespace NavSat.Core.Abstrations.Models {
    public class SatelliteOrbit {

        public int SatId { get; set; }

        public int HealthCode { get; set; }

        public double Eccentricity { get; set; }

        public double DeltaInclination { get; set; }

        public double RootOfSemiMajorAxis { get; set; }

        public double A0 { get; set; }

        public double A1 { get; set; }

        public double Omega0 { get; set; }

        public double Omega { get; set; }

        public double M0 { get; set; }

        public double OmegaDot { get; set; }

        public double GpsSeconds { get; set; }

        public int GpsWeek { get; set; }


        public bool IsHealthy {
            get {
                return HealthCode == 0;
            }
        }
    }
}
