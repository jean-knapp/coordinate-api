# Knapp Coordinate API

The `coordinate` namespace provides classes and utilities for working with geographic coordinates.

## Classes

### `Coordinate`

Represents a geographic coordinate.

#### Properties

- `Latitude`: The angular distance of a point on Earth's surface north or south of the equator, in decimal degrees.
- `Longitude`: The angular distance of a point on Earth's surface east or west of the prime meridian, in decimal degrees.
- `LatitudeString`: A string representation of the latitude value.
- `LongitudeString`: A string representation of the longitude value.

#### Constructors

- `Coordinate(double latitude, double longitude)`: Initializes a new instance of the `Coordinate` class with the specified latitude and longitude values.

#### Methods

- `ToString()`: Converts the coordinate to a string representation.
- `ToString(Format format)`: Converts the coordinate to a string representation using the specified format.
- `Clone()`: Creates a clone of the coordinate object.
- `ToMetres()`: Converts the coordinate to a projected coordinate system represented in meters.
- `FromMetres(double x, double y)`: Converts a projected coordinate system represented in meters to a geographic coordinate.
- `GetDistanceTo(Coordinate target, Earth.Model earthModel)`: Calculates the geodesic distance to a target coordinate.
- `GetBearingTo(Coordinate target, Earth.Model earthModel)`: Calculates the bearing from the current coordinate to a target coordinate.

### `Coordinate.Format`

Represents different coordinate formats.

- `DD`: Decimal degrees format.
- `DMM`: Degrees and decimal minutes format.
- `DMS`: Degrees, minutes, and seconds format.
- `UTM`: Universal Transverse Mercator (UTM) format.
- `MGRS`: Military Grid Reference System (MGRS) format.

## Usage

To use the `coordinate` namespace, you need to import the `coordinate.Formats` namespace and create an instance of the `Coordinate` class:

```csharp
using coordinate.Formats;
using System;

namespace YourNamespace
{
    public class YourClass
    {
        public static void Main()
        {
            Coordinate coordinate = new Coordinate(40.7128, -74.0060);
            Console.WriteLine(coordinate.ToString()); // Output: 40.7128°, -74.0060°
        }
    }
}
```

You can convert the coordinate to different formats using the ToString method:

```csharp
Coordinate coordinate = new Coordinate(40.7128, -74.0060);
Console.WriteLine(coordinate.ToString(Format.DMM)); // Output: 40° 42.768', -74° 0.360'
```

You can also perform calculations such as getting the distance to a target coordinate:

```csharp
Coordinate coordinate1 = new Coordinate(40.7128, -74.0060);
Coordinate coordinate2 = new Coordinate(34.0522, -118.2437);
double distance = coordinate1.GetDistanceTo(coordinate2);
Console.WriteLine(distance); // Output: 3939129.33567856 meters
```

For more information on the available methods and properties, refer to the documentation of each class and method.

That's it! You now have an overview of the `coordinate` namespace and how to use it in your applications.
