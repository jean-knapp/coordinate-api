using System;

namespace coordinate.Formats
{
    /// <summary>
    /// Represents a coordinate in Military Grid Reference System (MGRS) format.
    /// </summary>
    public class MGRS : UTM
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
        /// The two-letter digraph representing the grid zone.
        /// </summary>
        public string Digraph { get; protected set; }

        /// <summary>
        /// The easting value in string format.
        /// </summary>
        public new string Easting { get; private set; }

        /// <summary>
        /// The northing value in string format.
        /// </summary>
        public new string Northing { get; private set; }

        /// <summary>
        /// Array of digraphs for the 100km grid zones in the easting direction.
        /// </summary>
        private char[] digraphArrayE = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        private int[] SET_ORIGIN_COLUMN_LETTERS = { 'A', 'J', 'S', 'A', 'J', 'S' };
        private int[] SET_ORIGIN_ROW_LETTERS = { 'A', 'F', 'A', 'F', 'A', 'F' };
        private int NUM_100K_SETS = 6;

        /// <summary>
        /// Initializes a coordinate in Military Grid Reference System (MGRS) format.
        /// </summary>
        /// <param name="latitude">The latitude in decimal degrees.</param>
        /// <param name="longitude">The longitude in decimal degrees.</param>
        public MGRS(double latitude, double longitude) : base(0, 0)
        {
            if (!VerifyLatLon(latitude, longitude))
                System.Diagnostics.Debugger.Break();

            int longitudeZoneValue;
            int latitudeZoneValue;
            double eastingValue;
            double northingValue;

            Convert(latitude, longitude, out longitudeZoneValue, out latitudeZoneValue, out eastingValue, out northingValue);

            LongZone = longitudeZoneValue.ToString().PadLeft(2, '0');
            LatZone = digraphArrayN[latitudeZoneValue];
            Digraph = CalcDigraph(longitudeZoneValue, eastingValue, northingValue);
            Easting = FormatIngValue(eastingValue);
            Northing = FormatIngValue(northingValue);
        }

        /// <summary>
        /// Initializes a coordinate in Military Grid Reference System (MGRS) format.
        /// </summary>
        /// <param name="mgrs">The MGRS string.</param>
        public MGRS(string mgrs) : base(0, 0)
        {
            try
            {
                mgrs = mgrs.Replace(" ", "").ToUpper();  // remove whitespace
                int length = mgrs.Length;

                char[] charArray = mgrs.ToCharArray();
                int utmZoneCharIdx = char.IsDigit(charArray[1]) ? 2 : 1;
                LongZone = mgrs.Substring(0, utmZoneCharIdx);
                LatZone = mgrs.ToCharArray()[utmZoneCharIdx];
                Digraph = mgrs.Substring(utmZoneCharIdx + 1, 2);

                int i = utmZoneCharIdx + 3;
                int remainder = length - i;

                Easting = mgrs.Substring(utmZoneCharIdx + 3, remainder / 2);
                Northing = mgrs.Substring(utmZoneCharIdx + 3 + remainder / 2);
            }
            catch (Exception)
            {
                throw new Exception("Invalid MGRS string: " + mgrs);
            }
        }

        public override string ToString()
        {
            return $"{LongZone}{LatZone} {Digraph} {Easting} {Northing}";
        }

        /// <summary>
        /// Creates a clone of the coordinate object.
        /// </summary>
        /// <returns>A new Coordinate object with the same latitude and longitude values.</returns>
        public new MGRS Clone()
        {
            return new MGRS(0, 0)
            {
                LongZone = LongZone,
                LatZone = LatZone,
                Digraph = Digraph,
                Easting = Easting,
                Northing = Northing
            };
        }

        private string CalcDigraph(int longitudeZoneValue, double eastingValue, double northingValue)
        {
            int letter = (int)Math.Floor((longitudeZoneValue - 1) * 8 + eastingValue / 100000.0);
            int letterIdx = (letter % 24 + 23) % 24;

            char digraph = digraphArrayE[letterIdx];

            letter = (int)Math.Floor(northingValue / 100000.0);
            if (longitudeZoneValue / 2.0 == Math.Floor(longitudeZoneValue / 2.0))
            {
                letter = letter + 5;
            }

            letterIdx = letter - (int)(20 * Math.Floor(letter / 20.0));

            return digraph.ToString() + digraphArrayN[letterIdx].ToString();
        }

        private string FormatIngValue(double value)
        {
            string str = ((int)Math.Round(value - 100000 * Math.Floor(value / 100000))).ToString().PadLeft(5, '0');
            return str.Substring(str.Length - 5);
        }

        private int Get100kSetForZone(int i)
        {
            int set = i % NUM_100K_SETS;
            if (set == 0)
            {
                set = NUM_100K_SETS;
            }
            return set;
        }

        private double GetEastingFromChar(char e, int set)
        {
            int[] baseCol = SET_ORIGIN_COLUMN_LETTERS;
            int curCol = baseCol[set - 1];
            float eastingValue = 100000f;
            bool rewindMarker = false;

            while (curCol != e)
            {
                curCol++;
                if (curCol == 'I')
                {
                    curCol++;
                }

                if (curCol == 'O')
                {
                    curCol++;
                }

                if (curCol > 'Z')
                {
                    if (rewindMarker)
                    {
                        System.Diagnostics.Debugger.Break();
                    }

                    curCol = 'A';
                    rewindMarker = true;
                }

                eastingValue += 100000f;
            }

            return eastingValue;
        }

        private float GetNorthingFromChar(char n, int set)
        {
            if (n > 'V')
            {
                System.Diagnostics.Debugger.Break();
            }

            int[] baseRow = SET_ORIGIN_ROW_LETTERS;
            // rowOrigin is the letter at the origin of the set for the
            // column
            int curRow = baseRow[set - 1];
            float northingValue = 0f;
            bool rewindMarker = false;

            while (curRow != n)
            {
                curRow++;

                if (curRow == 'I')
                {
                    curRow++;
                }

                if (curRow == 'O')
                {
                    curRow++;
                }

                // fixing a bug making whole application hang in this loop
                // when 'n' is a wrong character
                if (curRow > 'V')
                {
                    if (rewindMarker)
                    { // making sure that this loop ends
                        System.Diagnostics.Debugger.Break();
                    }

                    curRow = 'A';
                    rewindMarker = true;
                }
                northingValue += 100000f;
            }

            return northingValue;
        }

        private double GetMinNorthing(char zoneLetter)
        {
            double northing;
            switch (zoneLetter)
            {
                case 'C':
                    northing = 1100000.0f;
                    break;
                case 'D':
                    northing = 2000000.0f;
                    break;
                case 'E':
                    northing = 2800000.0f;
                    break;
                case 'F':
                    northing = 3700000.0f;
                    break;
                case 'G':
                    northing = 4600000.0f;
                    break;
                case 'H':
                    northing = 5500000.0f;
                    break;
                case 'J':
                    northing = 6400000.0f;
                    break;
                case 'K':
                    northing = 7300000.0f;
                    break;
                case 'L':
                    northing = 8200000.0f;
                    break;
                case 'M':
                    northing = 9100000.0f;
                    break;
                case 'N':
                    northing = 0.0f;
                    break;
                case 'P':
                    northing = 800000.0f;
                    break;
                case 'Q':
                    northing = 1700000.0f;
                    break;
                case 'R':
                    northing = 2600000.0f;
                    break;
                case 'S':
                    northing = 3500000.0f;
                    break;
                case 'T':
                    northing = 4400000.0f;
                    break;
                case 'U':
                    northing = 5300000.0f;
                    break;
                case 'V':
                    northing = 6200000.0f;
                    break;
                case 'W':
                    northing = 7000000.0f;
                    break;
                case 'X':
                    northing = 7900000.0f;
                    break;
                default:
                    northing = -1.0f;
                    break;
            }

            if (northing >= 0.0)
            {
                return northing;
            }
            else
            {
                System.Diagnostics.Debugger.Break();
                return 0;
            }

        }

        private new DD ToDD()
        {
            int set = Get100kSetForZone(int.Parse(LongZone));

            double east100k = GetEastingFromChar(Digraph.ToCharArray()[0], set);
            double north100k = GetNorthingFromChar(Digraph.ToCharArray()[1], set);

            // We have a bug where the northing may be 2000000 too low.
            // How do we know when to roll over?

            while (north100k < GetMinNorthing(LatZone))
                north100k += 2000000;

            double accuracyBonus = 100000f / Math.Pow(10, Easting.Length);

            double easting = 0;
            double northing = 0;
            double.TryParse(Easting, out easting);
            double.TryParse(Northing, out northing);

            double sepEasting = easting * accuracyBonus;
            double sepNorthing = northing * accuracyBonus;

            return new UTM(LongZone + LatZone + " " + (sepEasting + east100k) + " " + (sepNorthing + north100k)).ToDD();
        }
    }
}
