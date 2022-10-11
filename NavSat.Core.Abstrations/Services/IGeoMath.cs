namespace NavSat.Core.Abstrations.Services {
    public interface IGeoMath {
        double Deg2Rad(double deg);
        double Rad2Deg(double rad);

        void Ecef2Geo(double[] xyz, out double lat, out double lon, out double hgt);
        double[] Geo2Ecef(double lat, double lon, double hgt);

        double[] EcefToElAz(double[] stat, double[] xecf);

    }
}
