using coordinate;
using coordinate.Formats;

var a = new DMM("5°30.5'S", "55°30.25'W");
var c = new DMS("5°30'30\"S", "55°30'15\"W");
var b = new MGRS("21M SQ 67286 46576");

Console.WriteLine(a.ToString(Coordinate.Format.MGRS));
Console.WriteLine(a == c);

int x = 1;
int y = 2;

Console.WriteLine(x + y);