using System;
using System.Linq;

namespace coordinate.Formats
{
    /// <summary>
    /// Represents a coordinate in UTM (Universal Transverse Mercator) format.
    /// </summary>
    public class UTM : Coordinate
    {
        public override double Latitude
        {
            get
            {
                return ToDD().Latitude;
            }
        }

        public override double Longitude
        {
            get
            {
                return ToDD().Longitude;
            }
        }

        /// <summary>
        /// Gets the hemisphere (N/S) of the latitude.
        /// </summary>
        public char LatitudeCardinal
        {
            get
            {
                char hemisphere = 'N';
                string southernHemisphere = "ACDEFGHJKLM";

                if (southernHemisphere.Contains(LatZone))
                {
                    hemisphere = 'S';
                }

                return hemisphere;
            }
        }

        /// <summary>
        /// Represents a coordinate in the UTM (Universal Transverse Mercator) format.
        /// </summary>
        public string LongZone { get; set; } = "31";
        /// <summary>
        /// Represents the latitude zone of the UTM coordinate.
        /// </summary>
        public char LatZone { get; set; } = 'N';

        /// <summary>
        /// Represents the easting value of the UTM coordinate.
        /// </summary>
        public int Easting { get; set; } = (int)166021.44;

        /// <summary>
        /// Represents the northing value of the UTM coordinate.
        /// </summary>
        public int Northing { get; set; } = (int)0;

        /// <summary>
        /// This variable represents the eccentricity of the Earth's ellipsoid
        /// </summary>
        private double e = 0.081819190837554151;

        /// <summary>
        /// This variable represents the square of the eccentricity, denoted as "e^2". It is the square of the eccentricity value
        /// </summary>
        private double esq = 0.0066943799893121048;

        /// <summary>
        /// This variable represents a modified form of the square of the eccentricity, denoted as "(e'^2)", where "e'" is another variation of the eccentricity
        /// </summary>
        private double e0sq = 0.0067394967414360083;

        /// <summary>
        /// This variable represents another variation of the square of the eccentricity, denoted as "(e1^2)", where "e1" is a different value related to the Earth's ellipsoid
        /// </summary>
        private double e1sq = 0.006739497;

        /// <summary>
        /// This variable represents the scale factor at the central meridian of a UTM zone, denoted as "k0"
        /// </summary>
        private double k0 = 0.9996;

        /// <summary>
        /// Array of characters representing the first letter of UTM grid zones.
        /// </summary>
        protected char[] digraphArrayN = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        /// <summary>
        /// Constructs a new UTM (Universal Transverse Mercator) coordinate object based on the given latitude and longitude.
        /// </summary>
        /// <param name="latitude">The latitude in decimal degrees.</param>
        /// <param name="longitude">The longitude in decimal degrees.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the supplied latitude and longitude values are not valid.</exception>
        public UTM(double latitude, double longitude) : base(0, 0)
        {
            if (!VerifyLatLon(latitude, longitude))
                throw new ArgumentOutOfRangeException("The supplied latitude and longitude values are not valid.");

            Convert(latitude, longitude, out int longitudeZoneValue, out int latitudeZoneValue, out double eastingValue, out double northingValue);

            LongZone = longitudeZoneValue.ToString().PadLeft(2, '0');
            LatZone = digraphArrayN[latitudeZoneValue];
            Easting = (int)Math.Round(eastingValue);
            Northing = (int)Math.Round(northingValue);
        }

        /// <summary>
        /// Constructs a new UTM (Universal Transverse Mercator) coordinate object based on the given latitude and longitude.
        /// </summary>
        /// <param name="utm">The UTM coordinate string.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the supplied latitude and longitude values are not valid.</exception>
        public UTM(string utm) : base(0, 0)
        {
            string[] utmArr = utm.Split(' ');
            if (utmArr.Length == 4)
            {
                LongZone = utmArr[0];
                LatZone = char.Parse(utmArr[1]);
                Easting = int.Parse(utmArr[2]);
                Northing = int.Parse(utmArr[3]);
            }
            else if (utmArr.Length == 3)
            {
                int utmZoneCharIdx = char.IsDigit(utm.ToCharArray()[1]) ? 2 : 1;
                LongZone = utmArr[0].Substring(0, utmZoneCharIdx);
                LatZone = char.Parse(utmArr[0].Substring(utmZoneCharIdx));

                int easting = 0;
                int northing = 0;
                int.TryParse(utmArr[1], out easting);
                int.TryParse(utmArr[2], out northing);

                Easting = easting;
                Northing = northing;
            }
        }

        public override string ToString()
        {
            return $"{LongZone}{LatZone} {Easting} {Northing}";
        }

        /// <summary>
        /// Creates a clone of the coordinate object.
        /// </summary>
        /// <returns>A new Coordinate object with the same latitude and longitude values.</returns>
        public new UTM Clone()
        {
            return new UTM(0, 0)
            {
                LongZone = LongZone,
                LatZone = LatZone,
                Easting = Easting,
                Northing = Northing
            };
        }

        protected void Convert(double latitude, double longitude, out int longitudeZoneValue, out int latitudeZoneValue, out double eastingValue, out double northingValue)
        {
            double latRad = latitude * Math.PI / 180.0;
            double utmz = 1 + Math.Floor((longitude + 180) / 6); // utm zone
            double zcm = 3 + 6 * (utmz - 1) - 180; // central meridian of a zone
            double latz = 0; // zone A-B for below 80S

            // convert latitude to latitude zone
            if (latitude > -80 && latitude < 72)
                latz = Math.Floor((latitude + 80) / 8) + 2; // zones C-W
            else
            {
                if (latitude > 72 && latitude < 84)
                    latz = 21; // zone X
                else
                    if (latitude > 84)
                    latz = 23; // zones Y-Z
            }

            double N = Earth.EquatorialRadius / Math.Sqrt(1 - Math.Pow(e * Math.Sin(latRad), 2));
            double T = Math.Pow(Math.Tan(latRad), 2);
            double C = e0sq * Math.Pow(Math.Cos(latRad), 2);
            double A = (longitude - zcm) * Math.PI / 180.0 * Math.Cos(latRad);

            // calculate M (USGS style)
            double M = latRad * (1.0 - esq * (1.0 / 4.0 + esq * (3.0 / 64.0 + 5.0 * esq / 256.0)));
            M = M - Math.Sin(2.0 * latRad) * (esq * (3.0 / 8.0 + esq * (3.0 / 32.0 + 45.0 * esq / 1024.0)));
            M = M + Math.Sin(4.0 * latRad) * (esq * esq * (15.0 / 256.0 + esq * 45.0 / 1024.0));
            M = M - Math.Sin(6.0 * latRad) * (esq * esq * esq * (35.0 / 3072.0));
            M = M * Earth.EquatorialRadius; //Arc length along standard meridian

            // calculate easting
            double k0 = 0.9996;
            double x = k0 * N * A * (1.0 + A * A * ((1.0 - T + C) / 6.0 + A * A * (5.0 - 18.0 * T + T * T + 72.0 * C - 58.0 * e0sq) / 120.0)); //Easting relative to CM
            x = x + 500000; // standard easting

            // calculate northing
            double y = k0 * (M + N * Math.Tan(latRad) * (A * A * (1.0 / 2.0 + A * A * ((5.0 - T + 9.0 * C + 4.0 * C * C) / 24.0 + A * A * (61.0 - 58.0 * T + T * T + 600.0 * C - 330.0 * e0sq) / 720.0)))); // from the equator

            if (y < 0)
                y = 10000000 + y; // add in false northing if south of the equator

            longitudeZoneValue = (int)utmz;
            latitudeZoneValue = (int)latz;
            eastingValue = x;
            northingValue = y;
        }

        private void SetVariables(out double phi1, out double fact1, out double fact2, out double fact3, out double fact4, out double _a3)
        {
            double arc = Northing / k0;
            double mu = arc
                    / (Earth.EquatorialRadius * (1 - Math.Pow(e, 2) / 4.0 - 3 * Math.Pow(e, 4) / 64.0 - 5 * Math.Pow(e, 6) / 256.0));

            double ei = (1 - Math.Pow(1 - e * e, 1 / 2.0))
                    / (1 + Math.Pow(1 - e * e, 1 / 2.0));

            double ca = 3 * ei / 2 - 27 * Math.Pow(ei, 3) / 32.0;

            double cb = 21 * Math.Pow(ei, 2) / 16 - 55 * Math.Pow(ei, 4) / 32;
            double cc = 151 * Math.Pow(ei, 3) / 96;
            double cd = 1097 * Math.Pow(ei, 4) / 512;
            phi1 = mu + ca * Math.Sin(2 * mu) + cb * Math.Sin(4 * mu) + cc * Math.Sin(6 * mu) + cd
                    * Math.Sin(8 * mu);

            double n0 = Earth.EquatorialRadius / Math.Pow(1 - Math.Pow(e * Math.Sin(phi1), 2), 1 / 2.0);

            double r0 = Earth.EquatorialRadius * (1 - e * e) / Math.Pow(1 - Math.Pow(e * Math.Sin(phi1), 2), 3 / 2.0);
            fact1 = n0 * Math.Tan(phi1) / r0;

            double _a1 = 500000 - Easting;
            double dd0 = _a1 / (n0 * k0);
            fact2 = dd0 * dd0 / 2;

            double t0 = Math.Pow(Math.Tan(phi1), 2);
            double Q0 = e1sq * Math.Pow(Math.Cos(phi1), 2);
            fact3 = (5 + 3 * t0 + 10 * Q0 - 4 * Q0 * Q0 - 9 * e1sq) * Math.Pow(dd0, 4)
                    / 24;

            fact4 = (61 + 90 * t0 + 298 * Q0 + 45 * t0 * t0 - 252 * e1sq - 3 * Q0
                    * Q0)
                    * Math.Pow(dd0, 6) / 720;

            //
            double lof1 = _a1 / (n0 * k0);
            double lof2 = (1 + 2 * t0 + Q0) * Math.Pow(dd0, 3) / 6.0;
            double lof3 = (5 - 2 * Q0 + 28 * t0 - 3 * Math.Pow(Q0, 2) + 8 * e1sq + 24 * Math.Pow(t0, 2))
                    * Math.Pow(dd0, 5) / 120;
            double _a2 = (lof1 - lof2 + lof3) / Math.Cos(phi1);
            _a3 = _a2 * 180 / Math.PI;
        }

        internal new DD ToDD()
        {
            char hemisphere = LatitudeCardinal;

            if (hemisphere == 'S')
                Northing = 10000000 - Northing;

            double phi1;
            double fact1;
            double fact2;
            double fact3;
            double fact4;
            double _a3;

            SetVariables(out phi1, out fact1, out fact2, out fact3, out fact4, out _a3);

            double latitude = 180 * (phi1 - fact1 * (fact2 + fact3 + fact4)) / Math.PI;

            int zone = int.Parse(LongZone);

            double zoneCM;
            if (zone > 0)
                zoneCM = 6 * zone - 183.0;
            else
                zoneCM = 3.0;

            double longitude = zoneCM - _a3;
            if (hemisphere == 'S')
                latitude = -latitude;

            DD coordinate = new DD(latitude, longitude);

            if (hemisphere == 'S')
                Northing = 10000000 - Northing;

            return coordinate;
        }
    }
}
