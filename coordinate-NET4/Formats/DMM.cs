using System;

namespace coordinate.Formats
{
    /// <summary>
    /// Represents a coordinate in degree-minute format.
    /// </summary>
    public class DMM : Coordinate
    {
        public override double Latitude
        {
            get
            {
                return (LatitudeCardinal == 'N' ? 1 : -1) * (LatitudeDegrees + LatitudeMinutes / 60.0);
            }
            set
            {
                LatitudeCardinal = value >= 0 ? 'N' : 'S';
                LatitudeDegrees = (short)Math.Floor(Math.Abs(value));
                LatitudeMinutes = (Math.Abs(value) - LatitudeDegrees) * 60;
            }
        }

        public override double Longitude
        {
            get
            {
                return (LongitudeCardinal == 'E' ? 1 : -1) * (LongitudeDegrees + LongitudeMinutes / 60.0);
            }
            set
            {
                LongitudeCardinal = value >= 0 ? 'E' : 'W';
                LongitudeDegrees = (short)Math.Floor(Math.Abs(value));
                LongitudeMinutes = (Math.Abs(value) - LongitudeDegrees) * 60;
            }
        }

        /// <summary>
        /// Gets or sets the hemisphere (N/S) of the latitude.
        /// </summary>
        public char LatitudeCardinal { get; set; }

        /// <summary>
        /// Gets or sets the degrees part of the latitude.
        /// </summary>
        public short LatitudeDegrees { get; set; }

        /// <summary>
        /// Gets or sets the minutes part of the latitude.
        /// </summary>
        public double LatitudeMinutes { get; set; }

        /// <summary>
        /// Gets or sets the hemisphere (E/W) of the longitude.
        /// </summary>
        public char LongitudeCardinal { get; set; }

        /// <summary>
        /// Gets or sets the degrees part of the longitude.
        /// </summary>
        public short LongitudeDegrees { get; set; }

        /// <summary>
        /// Gets or sets the minutes part of the longitude.
        /// </summary>
        public double LongitudeMinutes { get; set; }

        /// <summary>
        /// Initializes a coordinate in degree-minute format.
        /// </summary>
        /// <param name="latitude">The latitude value.</param>
        /// <param name="longitude">The longitude value.</param>
        public DMM(double latitude, double longitude) : base(latitude, longitude) { }

        /// <summary>
        /// Initializes a coordinate in degree-minute format.
        /// </summary>
        /// <param name="latitude">The latitude value in degree-minute format.</param>
        /// <param name="longitude">The longitude value in degree-minute format.</param>
        public DMM(string latitude, string longitude) : base(0, 0)
        {
            latitude = latitude.ToUpper();
            longitude = longitude.ToUpper();

            latitude = latitude.Replace("º", "°").Replace("´", "'").Replace(" ", "").Replace(",", ".");
            longitude = longitude.Replace("º", "°").Replace("´", "'").Replace(" ", "").Replace(",", ".");

            if (latitude.Contains("N"))
            {
                latitude = latitude.Replace("N", "");
                LatitudeCardinal = 'N';
            }
            else if (latitude.Contains("S"))
            {
                latitude = latitude.Replace("S", "");
                LatitudeCardinal = 'S';
            }
            else
            {
                LatitudeCardinal = 'N';
            }

            if (longitude.Contains("W"))
            {
                longitude = longitude.Replace("W", "");
                LongitudeCardinal = 'W';
            }
            else if (longitude.Contains("E"))
            {
                longitude = longitude.Replace("E", "");
                LongitudeCardinal = 'E';
            }
            else
            {
                LongitudeCardinal = 'W';
            }

            latitude = latitude.Replace("'", "");
            longitude = longitude.Replace("'", "");

            if (latitude.Contains("°"))
            {
                var parts = latitude.Split('°');

                short latDegreesShort = 0;
                double latMinutesDouble = 0;

                short.TryParse(parts[0], out latDegreesShort);
                double.TryParse(parts[1], out latMinutesDouble);

                LatitudeDegrees = latDegreesShort;
                LatitudeMinutes = latMinutesDouble;
            }
            else
            {
                LatitudeDegrees = 0;
                LatitudeMinutes = 0;
            }

            if (longitude.Contains("°"))
            {
                var parts2 = longitude.Split('°');

                short lngDegreesShort = 0;
                double lngMinutesDouble = 0;

                short.TryParse(parts2[0], out lngDegreesShort);
                double.TryParse(parts2[1], out lngMinutesDouble);

                LongitudeDegrees = lngDegreesShort;
                LongitudeMinutes = lngMinutesDouble;
            }
            else
            {
                LongitudeDegrees = 0;
                LongitudeMinutes = 0;
            }
        }

        public override string ToString()
        {
            return $"{LatitudeString} {LongitudeString}";
        }

        public override string LatitudeString
        {
            get
            {
                return $"{LatitudeCardinal}{LatitudeDegrees:00}°{LatitudeMinutes:00.000}'";
            }
        }

        public override string LongitudeString
        {
            get
            {
                return $"{LongitudeCardinal}{LongitudeDegrees:000}°{LongitudeMinutes:00.000}'";
            }
        }

        /// <summary>
        /// Creates a clone of the coordinate object.
        /// </summary>
        /// <returns>A new Coordinate object with the same latitude and longitude values.</returns>
        public new DMM Clone()
        {
            return new DMM(0, 0)
            {
                LatitudeCardinal = LatitudeCardinal,
                LatitudeDegrees = LatitudeDegrees,
                LatitudeMinutes = LatitudeMinutes,
                LongitudeCardinal = LongitudeCardinal,
                LongitudeDegrees = LongitudeDegrees,
                LongitudeMinutes = LongitudeMinutes
            };
        }
    }
}
