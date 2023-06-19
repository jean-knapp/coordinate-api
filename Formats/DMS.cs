using System;

namespace coordinate.Formats
{
    /// <summary>
    /// Represents a coordinate in degree-minute-second format.
    /// </summary>
    public class DMS : DD
    {
        public override double Latitude
        {
            get
            {
                return (LatitudeCardinal == 'N' ? 1 : -1) * (LatitudeDegrees + LatitudeMinutes / 60.0 + LatitudeSeconds / 3600.0);
            }
            set
            {
                LatitudeCardinal = value >= 0 ? 'N' : 'S';
                LatitudeDegrees = (short)Math.Floor(Math.Abs(value));
                LatitudeMinutes = (short)((Math.Abs(value) - LatitudeDegrees) * 60);
                LatitudeSeconds = (Math.Abs(value) - LatitudeDegrees - LatitudeMinutes / 60.0) * 3600;
            }
        }

        public override double Longitude
        {
            get
            {
                return (LongitudeCardinal == 'E' ? 1 : -1) * (LongitudeDegrees + LongitudeMinutes / 60.0 + LongitudeSeconds / 3600.0);
            }
            set
            {
                LongitudeCardinal = value >= 0 ? 'E' : 'W';
                LongitudeDegrees = (short)Math.Floor(Math.Abs(value));
                LongitudeMinutes = (short)((Math.Abs(value) - LongitudeDegrees) * 60);
                LongitudeSeconds = (Math.Abs(value) - LongitudeDegrees - LongitudeMinutes / 60.0) * 3600;
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
        public short LatitudeMinutes { get; set; }

        /// <summary>
        /// Gets or sets the seconds part of the latitude.
        /// </summary>
        public double LatitudeSeconds { get; set; }

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
        public short LongitudeMinutes { get; set; }

        /// <summary>
        /// Gets or sets the seconds part of the longitude.
        /// </summary>
        public double LongitudeSeconds { get; set; }

        /// <summary>
        /// Initializes a coordinate in degree-minute-second format.
        /// </summary>
        /// <param name="latitude">The latitude value.</param>
        /// <param name="longitude">The longitude value.</param>
        public DMS(double latitude, double longitude) : base(latitude, longitude) { }

        /// <summary>
        /// Initializes a coordinate in degree-minute-second format.
        /// </summary>
        /// <param name="latitude">The latitude value in degree-minute-second format.</param>
        /// <param name="longitude">The longitude value in degree-minute-second format.</param>
        public DMS(string latitude, string longitude) : base(0,0)
        {
            latitude = latitude.ToUpper();
            longitude = longitude.ToUpper();

            latitude = latitude.Replace("º", "°").Replace("´", "'").Replace("''", "\"").Replace(" ", "").Replace(",", ".");
            longitude = longitude.Replace("º", "°").Replace("´", "'").Replace("''", "\"").Replace(" ", "").Replace(",", ".");

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

            latitude = latitude.Replace("\"", "");
            longitude = longitude.Replace("\"", "");

            if (latitude.Contains("°") && latitude.Contains("'"))
            {
                var parts = latitude.Split(new char[] { '°', '\'' });

                short sDegrees = 0;
                short sMinutes = 0;
                double dSeconds = 0;
                short.TryParse(parts[0], out sDegrees);
                short.TryParse(parts[1], out sMinutes);
                double.TryParse(parts[2], out dSeconds);
                LatitudeDegrees = sDegrees;
                LatitudeMinutes = sMinutes;
                LatitudeSeconds = dSeconds;
            }
            else
            {
                LatitudeDegrees = 0;
                LatitudeMinutes = 0;
                LatitudeSeconds = 0;
            }

            if (longitude.Contains("°") && longitude.Contains("'"))
            {
                var parts2 = longitude.Split(new char[] { '°', '\'' });

                short sDegrees = 0;
                short sMinutes = 0;
                double dSeconds = 0;
                short.TryParse(parts2[0], out sDegrees);
                short.TryParse(parts2[1], out sMinutes);
                double.TryParse(parts2[2], out dSeconds);
                LongitudeDegrees = sDegrees;
                LongitudeMinutes = sMinutes;
                LongitudeSeconds = dSeconds;
            }
            else
            {
                LongitudeDegrees = 0;
                LongitudeMinutes = 0;
                LongitudeSeconds = 0;
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
                return $"{LatitudeCardinal}{LatitudeDegrees:00}°{LatitudeMinutes:00}'{LatitudeSeconds:00}\"";
            }
        }

        public override string LongitudeString
        {
            get
            {
                return $"{LongitudeCardinal}{LongitudeDegrees:000}°{LongitudeMinutes:00}'{LongitudeSeconds:00}\"";
            }
        }

        /// <summary>
        /// Creates a clone of the coordinate object.
        /// </summary>
        /// <returns>A new Coordinate object with the same latitude and longitude values.</returns>
        public new DMS Clone()
        {
            return new DMS(0, 0)
            {
                LatitudeCardinal = LatitudeCardinal,
                LatitudeDegrees = LatitudeDegrees,
                LatitudeMinutes = LatitudeMinutes,
                LatitudeSeconds = LatitudeSeconds,
                LongitudeCardinal = LongitudeCardinal,
                LongitudeDegrees = LongitudeDegrees,
                LongitudeMinutes = LongitudeMinutes,
                LongitudeSeconds = LongitudeSeconds
            };
        }
    }
}
