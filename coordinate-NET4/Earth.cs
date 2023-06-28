namespace coordinate
{
    public static class Earth
    {
        /// <summary>
        /// Represents the models available for defining the Earth's shape and dimensions.
        /// </summary>
        public enum Model
        {
            /// <summary>
            /// The Earth is modeled as a perfect sphere.
            /// </summary>
            SPHERE,

            /// <summary>
            /// The Earth is modeled using the WGS84 ellipsoid model.
            /// </summary>
            WGS84
        }


        /// <summary>
        /// The Earth's equatorial radius, in meters.
        /// </summary>
        public static double EquatorialRadius = 6378137.0;

        /// <summary>
        /// The Earth's semi-major axis, representing the equatorial radius, in meters.
        /// </summary>
        public static double SemiMajorAxis = 6378137.0;

        /// <summary>
        /// The Earth's semi-minor axis, representing the polar radius, in meters.
        /// </summary>
        public static double SemiMinorAxis = 6356752.3142;

        /// <summary>
        /// The flattening parameter of the Earth, defining the deviation from a perfect sphere.
        /// </summary>
        public static double Flattening = 1 / 298.257223563;

    }
}
