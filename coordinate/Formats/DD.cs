using System;

namespace coordinate.Formats
{
    /// <summary>
    /// Represents a coordinate in decimal degree format.
    /// </summary>
    public class DD : Coordinate
    {
        /// <summary>
        /// Initializes a coordinate in decimal degree format.
        /// </summary>
        /// <param name="latitude">The latitude value.</param>
        /// <param name="longitude">The longitude value.</param>
        public DD(double latitude, double longitude) : base(latitude, longitude) { }

        /// <summary>
        /// Initializes a coordinate in decimal degree format.
        /// </summary>
        /// <param name="latitude">The latitude value as a string.</param>
        /// <param name="longitude">The longitude value as a string.</param>
        /// <param name="format">The format of the latitude and longitude values. Default is Format.DD.</param>
        /// <exception cref="ArgumentException">Thrown when the latitude or longitude value(s) are invalid.</exception>
        /// <exception cref="NotImplementedException">Thrown when the specified format is not implemented.</exception>
        public DD(string latitude, string longitude) : base(0, 0)
        {
            if (double.TryParse(latitude, out double lat) && double.TryParse(longitude, out double lon))
            {
                Latitude = lat;
                Longitude = lon;
            }
            else
            {
                throw new ArgumentException("Invalid latitude or longitude value(s).");
            }
        }

        public override string ToString()
        {
            return $"{LatitudeString}, {LongitudeString}";
        }

        public override string LatitudeString
        {
            get
            {
                return Latitude.ToString();
            }
        }

        public override string LongitudeString
        {
            get
            {
                return Longitude.ToString();
            }
        }

        /// <summary>
        /// Creates a clone of the coordinate object.
        /// </summary>
        /// <returns>A new Coordinate object with the same latitude and longitude values.</returns>
        public new DD Clone()
        {
            return new DD(latitude: Latitude, longitude: Longitude);
        }
    }
}
