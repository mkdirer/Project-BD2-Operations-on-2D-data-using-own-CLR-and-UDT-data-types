using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text;
using System.Globalization;

// class representing a complex type - Point

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.Native)]
public struct Point : INullable
{

    private int _x;
    private int _y;
    private bool isNull;

    // method encoding the class object as a string
    public override string ToString()
    {
        if (this.IsNull)
            return "NULL";
        else
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this._x);
            builder.Append(";");
            builder.Append(this._y);
            return builder.ToString();
        }
    }

    public bool IsNull
    {
        get
        {
            return isNull;
        }
    }

    public static Point Null
    {
        get
        {
            Point tmp = new Point();
            tmp.isNull = true;
            return tmp;
        }
    }

    // method to parse the string from the SQL command to create a new object
    [SqlMethod(OnNullCall = false)]
    public static Point Parse(SqlString value)
    {
        if (value.IsNull)
            return Null;
        Point p = new Point();
        string[] arrayPoints = value.Value.Split(";".ToCharArray());
        p.X = Int32.Parse(arrayPoints[0]);
        p.Y = Int32.Parse(arrayPoints[1]);
        return p;
    }

    //getters and setters
    public int X
    {
        get
        {
            return this._x;
        }
        set
        {
            double temp = _x;
            _x = value;
        }
    }

    public int Y
    {
        get
        {
            return this._y;
        }
        set
        {
            double temp = _y;
            _y = value;
        }
    }

    // method returning distance between points
    public SqlDouble distance(Point p)
    {
        return Math.Sqrt(Math.Pow(this._x - p._x, 2) + Math.Pow(this._y - p._y, 2));
    }

}