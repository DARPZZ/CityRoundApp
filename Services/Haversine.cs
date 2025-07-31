using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vamdrup_rundt.Services
{
    public class Haversine
    {
        public double ReverseGeocodeSnapChecker(double lat1, double lon1, double lat2, double lon2)
        {
            const double RadiusEarthKm = 6371.0;

            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return RadiusEarthKm * c * 1000;
        }

        private static double DegreesToRadians(double deg) => deg * (Math.PI / 180);
    }
}
