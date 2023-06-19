using coordinate.Formats;
using System;

namespace coordinate
{
    public class Coordinate
    {
        /// <summary>
        /// Represents different coordinate formats.
        /// </summary>
        public enum Format
        {
            /// <summary>
            /// Decimal degrees format.
            /// </summary>
            DD,

            /// <summary>
            /// Degrees and decimal minutes format.
            /// </summary>
            DMM,

            /// <summary>
            /// Degrees, minutes, and seconds format.
            /// </summary>
            DMS,

            /// <summary>
            /// Universal Transverse Mercator (UTM) format.
            /// </summary>
            UTM,

            /// <summary>
            /// Military Grid Reference System (MGRS) format.
            /// </summary>
            MGRS
        }

        /// <summary>
        /// The angular distance of a point on Earth's surface north or south of the equator, in decimal degrees.
        /// </summary>
        public virtual double Latitude { get; set; } = 0;

        /// <summary>
        /// The angular distance of a point on Earth's surface east or west of the prime meridian, in decimal degrees.
        /// </summary>
        public virtual double Longitude { get; set; } = 0;

        /// <summary>
        /// A string representation of the latitude value.
        /// </summary>
        public virtual string LatitudeString
        {
            get
            {
                return Latitude.ToString();
            }
        }

        /// <summary>
        /// A string representation of the longitude value.
        /// </summary>
        public virtual string LongitudeString
        {
            get
            {
                return Longitude.ToString();
            }
        }

        /// <summary>
        /// Initializes a new instance of the Coordinate class with the specified latitude and longitude values.
        /// </summary>
        /// <param name="latitude">The latitude value.</param>
        /// <param name="longitude">The longitude value.</param>
        public Coordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Converts the coordinate to a string representation.
        /// </summary>
        /// <returns>A string representation of the coordinate.</returns>
        public new virtual string ToString()
        {
            return ToString(Format.DD);
        }

        /// <summary>
        /// Converts the coordinate to a string representation.
        /// </summary>
        /// <param name="format">The desired coordinate format. Default is Decimal Degrees (DD).</param>
        /// <returns>A string representation of the coordinate.</returns>
        public string ToString(Format format = Format.DD)
        {
            switch (format)
            {
                case Format.DD:
                    return new DD(Latitude, Longitude).ToString();
                case Format.DMM:
                    return new DMM(Latitude, Longitude).ToString();
                case Format.DMS:
                    return new DMS(Latitude, Longitude).ToString();
                case Format.UTM:
                    return new UTM(Latitude, Longitude).ToString();
                case Format.MGRS:
                    return new MGRS(Latitude, Longitude).ToString();
                default:
                    return ToString();
            }
        }

        /// <summary>
        /// Creates a clone of the coordinate object.
        /// </summary>
        /// <returns>A new Coordinate object with the same latitude and longitude values.</returns>
        public Coordinate Clone()
        {
            return new Coordinate(latitude: Latitude, longitude: Longitude);
        }

        /// <summary>
        /// Converts the latitude and longitude values of the coordinate to radians.
        /// </summary>
        /// <returns>A tuple containing the latitude and longitude values in radians.</returns>
        private (double LatitudeRad, double LongitudeRad) ToRadians()
        {
            return (Latitude * (Math.PI / 180.0), Longitude * (Math.PI / 180.0));
        }

        /// <summary>
        /// Converts the given latitude and longitude values from radians to degrees and creates a new Coordinate object.
        /// </summary>
        /// <param name="latitudeRad">The latitude value in radians.</param>
        /// <param name="longitudeRad">The longitude value in radians.</param>
        /// <returns>A new Coordinate object with latitude and longitude values in degrees.</returns>
        private static Coordinate FromRadians(double latitudeRad, double longitudeRad)
        {
            return new Coordinate(latitude: latitudeRad * (180.0 / Math.PI), longitude: longitudeRad * (180.0 / Math.PI));
        }

        /// <summary>
        /// Converts a geographic coordinate to a projected coordinate system represented in meters.
        /// </summary>
        /// <returns>A tuple representing the X and Y coordinates in meters.</returns>
        public (double X, double Y) ToMetres()
        {
            // The method uses a simple conversion formula to calculate the X and Y coordinates in meters.
            // It takes into account the curvature of the Earth by multiplying the longitude and latitude
            // values by specific conversion factors.

            double x = (Longitude * 1852 * 60 * Math.Cos(Latitude * Math.PI / 180));
            double y = Latitude * 1852 * 60;
            return (x, y);
        }

        /// <summary>
        /// Converts a projected coordinate system represented in meters to a geographic coordinate.
        /// </summary>
        /// <param name="x">The X coordinate in meters.</param>
        /// <param name="y">The Y coordinate in metres</param>
        /// <returns>A new Coordinate object with latitude and longitude values in degrees.,
        /// 3</returns>
        public static Coordinate FromMetres(double x, double y)
        {
            double lat = (y / 1852 / 60);
            double lon = (x / 1852 / 60 / Math.Cos(lat * Math.PI / 180));
            return new Coordinate(lat, lon);
        }

        /// <summary>
        /// Calculates the geodesic distance to a target coordinate.
        /// </summary>
        /// <param name="target">The target coordinate.</param>
        /// <param name="earthModel">The model representing the Earth's shape and dimensions. Default is WGS84.</param>
        /// <returns>The distance in meters between the current coordinate and the target coordinate.</returns>
        public double GetDistanceTo(Coordinate target, Earth.Model earthModel = Earth.Model.WGS84)
        {
            switch (earthModel)
            {
                case Earth.Model.SPHERE:
                    return GetSphereDistanceTo(target);
                case Earth.Model.WGS84:
                    return GetWGS84DistanceTo(target);
                default:
                    throw new ArgumentException("Invalid Earth model specified.", nameof(earthModel));
            }
        }

        /// <summary>
        /// Calculates the geodesic distance to a target coordinate using the Haversine formula (Sphere model).
        /// </summary>
        /// <param name="target">The target coordinate.</param>
        /// <returns>The distance in meters between the current coordinate and the target coordinate.</returns>
        private double GetSphereDistanceTo(Coordinate target)
        {
            // The haversine formula is a mathematical formula that is used to calculate distances between two points on the surface of a sphere.

            // Haversine formula:
            // a = sin²(Δφ/2) + cos(φ1).cos(φ2).sin²(Δλ/2)
            // c = 2.atan2(√a, √(1−a))
            // d = R.c
            // where φ is latitude, λ is longitude, R is earth’s radius (mean radius = 6,371km);

            // Convert the latitude and longitude values to radians.
            (double latitudeRad, double longitudeRad) = ToRadians();
            (double targetLatitudeRad, double targetLongitudeRad) = target.ToRadians();

            // Apply the Haversine formula.
            double a = Math.Pow(Math.Sin((targetLatitudeRad - latitudeRad) / 2.0), 2.0) + Math.Cos(latitudeRad) * Math.Cos(targetLatitudeRad) * Math.Pow(Math.Sin((targetLongitudeRad - longitudeRad) / 2.0), 2.0);
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            double d = Earth.EquatorialRadius * c;

            return d;
        }

        /// <summary>
        /// Calculates the geodesic distance to a target coordinate using the Vincenty formula (WGS84 model).
        /// </summary>
        /// <param name="target">The target coordinate.</param>
        /// <returns>The distance in meters between the current coordinate and the target coordinate.</returns>
        private double GetWGS84DistanceTo(Coordinate target)
        {
            // This formula provides a more accurate result than the Haversine formula for calculating
            // distances over long distances on the Earth's surface, taking into account the Earth's
            // oblate ellipsoid shape.

            // Vincenty formula:
            // a = semi-major axis length (equatorial radius)
            // b = semi-minor axis length (polar radius)
            // f = flattening of the ellipsoid (f = (a-b)/a)
            // L = difference in longitude between the two points
            // U1 = atan((1 - f) * tan(latitude1))
            // U2 = atan((1 - f) * tan(latitude2))
            // sinU1, cosU1, sinU2, cosU2, lambda, and iter are intermediate variables
            // lambda = L, and iter is used for the iterative calculation

            double a = Earth.SemiMajorAxis;
            double b = Earth.SemiMinorAxis;
            double f = Earth.Flattening;

            double phi1 = Latitude * Math.PI / 180.0;
            double phi2 = target.Latitude * Math.PI / 180.0;
            double lambda = (target.Longitude - Longitude) * Math.PI / 180.0;

            double sinU1 = Math.Sin(phi1);
            double cosU1 = Math.Cos(phi1);
            double sinU2 = Math.Sin(phi2);
            double cosU2 = Math.Cos(phi2);

            double L = lambda;
            double iter;
            double cosSqAlpha;
            double sigma;
            double sinSigma;
            double cos2SigmaM;
            double cosSigma;
            double sinLambda;
            double cosLambda;

            do
            {
                sinLambda = Math.Sin(L);
                cosLambda = Math.Cos(L);
                double sinSqSigma = (cosU2 * sinLambda) * (cosU2 * sinLambda) + (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda) * (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda);
                sinSigma = Math.Sqrt(sinSqSigma);
                cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
                sigma = Math.Atan2(sinSigma, cosSigma);
                double sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
                cosSqAlpha = 1.0 - sinAlpha * sinAlpha;
                cos2SigmaM = cosSigma - 2.0 * sinU1 * sinU2 / cosSqAlpha;

                double C = f / 16.0 * cosSqAlpha * (4.0 + f * (4.0 - 3.0 * cosSqAlpha));
                iter = L;
                L = lambda + (1.0 - C) * f * sinAlpha *
                    (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1.0 + 2.0 * cos2SigmaM * cos2SigmaM)));
            }
            while (Math.Abs(L - iter) > 1e-12);

            double uSq = cosSqAlpha * (a * a - b * b) / (b * b);
            double A = 1.0 + uSq / 16384.0 * (4096.0 + uSq * (-768.0 + uSq * (320.0 - 175.0 * uSq)));
            double B = uSq / 1024.0 * (256.0 + uSq * (-128.0 + uSq * (74.0 - 47.0 * uSq)));
            double deltaSigma = B * sinSigma * (cos2SigmaM + B / 4.0 * (cosSigma * (-1.0 + 2.0 * cos2SigmaM * cos2SigmaM) -
                B / 6.0 * cos2SigmaM * (-3.0 + 4.0 * sinSigma * sinSigma) * (-3.0 + 4.0 * cos2SigmaM * cos2SigmaM)));

            double distance = b * A * (sigma - deltaSigma);

            return distance;
        }

        /// <summary>
        /// Calculates the bearing from the current coordinate to a target coordinate.
        /// </summary>
        /// <param name="target">The target coordinate.</param>
        /// <param name="earthModel">The model representing the Earth's shape and dimensions. Default is WGS84.</param>
        /// <returns>The bearing in degrees from the current coordinate to the target coordinate.</returns>
        public double GetBearingTo(Coordinate target, Earth.Model earthModel = Earth.Model.WGS84)
        {
            switch (earthModel)
            {
                case Earth.Model.SPHERE:
                    return GetSphereBearingTo(target);
                case Earth.Model.WGS84:
                    return GetWGS84BearingTo(target);
                default:
                    throw new ArgumentException("Invalid Earth model specified.", nameof(earthModel));
            }
        }


        /// <summary>
        /// Calculates the bearing from the current coordinate to a target coordinate (Sphere model).
        /// </summary>
        /// <param name="target">The target coordinate.</param>
        /// <returns>The bearing in degrees from the current coordinate to the target coordinate.</returns>
        private double GetSphereBearingTo(Coordinate target)
        {
            var dLon = (target.Longitude - Longitude) * Math.PI / 180;
            var dPhi = Math.Log(Math.Tan(target.Latitude * Math.PI / 180 / 2 + Math.PI / 4) / Math.Tan(Latitude * Math.PI / 180 / 2 + Math.PI / 4));
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : 2 * Math.PI + dLon;
            return (Math.Atan2(dLon, dPhi) * 180 / Math.PI + 360) % 360;
        }

        // <summary>
        /// Calculates the bearing from the current coordinate to a target coordinate (WGS84 model).
        /// </summary>
        /// <param name="target">The target coordinate.</param>
        /// <returns>The bearing in degrees from the current coordinate to the target coordinate.</returns>
        private double GetWGS84BearingTo(Coordinate target)
        {
            double lat1 = Latitude * Math.PI / 180;
            double lon1 = Longitude * Math.PI / 180;
            double lat2 = target.Latitude * Math.PI / 180;
            double lon2 = target.Longitude * Math.PI / 180;

            double a = Earth.SemiMajorAxis;
            double b = Earth.SemiMinorAxis;
            double f = Earth.Flattening;

            double L = lon2 - lon1;
            double sinU1 = Math.Sin(lat1);
            double cosU1 = Math.Cos(lat1);
            double sinU2 = Math.Sin(lat2);
            double cosU2 = Math.Cos(lat2);

            double lambda = L;
            double lambdaP;
            double sinLambda;

            double cosLambda, sinSigma, cosSigma, sigma, sinAlpha, cosSqAlpha, cos2SigmaM;

            do
            {
                lambdaP = lambda;
                sinLambda = Math.Sin(lambdaP);
                cosLambda = Math.Cos(lambdaP);
                sinSigma = Math.Sqrt(Math.Pow(cosU2 * sinLambda, 2) + Math.Pow(cosU1 * sinU2 - sinU1 * cosU2 * cosLambda, 2));
                cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
                sigma = Math.Atan2(sinSigma, cosSigma);
                sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
                cosSqAlpha = 1 - Math.Pow(sinAlpha, 2);
                cos2SigmaM = cosSigma - 2 * sinU1 * sinU2 / cosSqAlpha;

                double C = f / 16 * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
                lambda = L + (1 - C) * f * sinAlpha * (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * Math.Pow(cos2SigmaM, 2))));
            }
            while (Math.Abs(lambda - lambdaP) > 1e-12);

            double uSq = cosSqAlpha * (Math.Pow(a, 2) - Math.Pow(b, 2)) / Math.Pow(b, 2);
            double A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            double B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
            double deltaSigma = B * sinSigma * (cos2SigmaM + B / 4 * (cosSigma * (-1 + 2 * Math.Pow(cos2SigmaM, 2)) - B / 6 * cos2SigmaM * (-3 + 4 * Math.Pow(sinSigma, 2)) * (-3 + 4 * Math.Pow(cos2SigmaM, 2))));

            double bearing = Math.Atan2(cosU2 * sinLambda, cosU1 * sinU2 - sinU1 * cosU2 * cosLambda);
            bearing = (bearing * 180 / Math.PI + 360) % 360;

            return bearing;
        }

        /// <summary>
        /// Translates the current coordinate by a given bearing and distance, considering the specified Earth model.
        /// </summary>
        /// <param name="bearing">The bearing angle in degrees.</param>
        /// <param name="distance">The distance in meters.</param>
        /// <param name="earthModel">The Earth model to consider. Default is Earth.Model.SPHERE.</param>
        /// <returns>The translated Coordinate.</returns>
        /// <exception cref="NotImplementedException">Thrown when the Earth model is Earth.Model.WGS84 and the calculation is not implemented.</exception>
        /// <exception cref="ArgumentException">Thrown when an invalid Earth model is specified.</exception>
        public Coordinate TranslateTo(double bearing, double distance, Earth.Model earthModel = Earth.Model.SPHERE)
        {
            switch (earthModel)
            {
                case Earth.Model.SPHERE:
                    return SphereTranslateTo(bearing, distance);
                case Earth.Model.WGS84:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Invalid Earth model specified.", nameof(earthModel));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bearing"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private Coordinate SphereTranslateTo(double bearing, double distance)
        {
            bearing = (bearing + 360) % 360;
            double headingRadians = bearing * Math.PI / 180.0;

            (double latRadians, double lngRadians) = ToRadians();

            double targetLatRadians = Math.Asin(Math.Sin(latRadians) * Math.Cos(distance / Earth.EquatorialRadius) + Math.Cos(latRadians) * Math.Sin(distance / Earth.EquatorialRadius) * Math.Cos(headingRadians));
            double targetLngRadians = lngRadians + Math.Atan2(Math.Sin(headingRadians) * Math.Sin(distance / Earth.EquatorialRadius) * Math.Cos(latRadians), Math.Cos(distance / Earth.EquatorialRadius) - Math.Sin(latRadians) * Math.Sin(targetLatRadians));

            return FromRadians(targetLatRadians, targetLngRadians);
        }

        /// <summary>
        /// Verifies the validity of the latitude and longitude coordinates.
        /// </summary>
        /// <param name="latitude">The latitude value to verify.</param>
        /// <param name="longitude">The longitude value to verify.</param>
        /// <returns><c>true</c> if the latitude and longitude coordinates are valid, otherwise <c>false</c>.</returns>
        protected static bool VerifyLatLon(double latitude, double longitude)
        {
            return latitude >= -90.0 && latitude <= 90.0 && longitude >= -180.0 && longitude <= 180.0;
        }

        /// <summary>
        /// Converts the coordinate to decimal degrees (DD) format.
        /// </summary>
        /// <returns>A new coordinate in decimal degrees format.</returns>
        public DD ToDD()
        {
            return new DD(Latitude, Longitude);
        }

        /// <summary>
        /// Converts the coordinate to degrees and decimal minutes (DMM) format.
        /// </summary>
        /// <returns>A new coordinate in degrees and decimal minutes format.</returns>
        public DMM ToDMM()
        {
            return new DMM(Latitude, Longitude);
        }

        /// <summary>
        /// Converts the coordinate to degrees, minutes, and seconds (DMS) format.
        /// </summary>
        /// <returns>A new coordinate in degrees, minutes, and seconds format.</returns>
        public DMS ToDMS()
        {
            return new DMS(Latitude, Longitude);
        }

        /// <summary>
        /// Converts the coordinate to Universal Transverse Mercator (UTM) format.
        /// </summary>
        /// <returns>A new coordinate in UTM format.</returns>
        public UTM ToUTM()
        {
            return new UTM(Latitude, Longitude);
        }

        /// <summary>
        /// Converts the coordinate to Military Grid Reference System (MGRS) format.
        /// </summary>
        /// <returns>A new coordinate in MGRS format.</returns>
        public MGRS ToMGRS()
        {
            return new MGRS(Latitude, Longitude);
        }

    }
}
