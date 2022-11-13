using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Globalization;

//klasa reprezentujaca typ zlozony - Polygon

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(
    Format.UserDefined,
    MaxByteSize = 8000,
    IsByteOrdered = true
 )]
public struct Polygon : INullable, IBinarySerialize
{
    private List<Point> polygonPoints;
    private bool isNull;

    // method encoding the class object as a string
    public override string ToString()
    {
        StringBuilder stringPoints = new StringBuilder();
        foreach (var point in polygonPoints)
        {
            stringPoints.Append("[");
            stringPoints.Append(point);
            stringPoints.Append("]");

        }
        return stringPoints.ToString();
    }



    public bool IsNull
    {
        get
        {
            return isNull;
        }
    }

    public static Polygon Null
    {
        get
        {
            Polygon tmp = new Polygon();
            tmp.isNull = true;
            return tmp;
        }
    }

    // private method returning a point inside the polygon
    private Point averagePointInside()
    {
        int x = 0;
        int y = 0;
        foreach (var point in polygonPoints)
        {
            x += point.X;
            y += point.Y;
        }
        Point value = new Point();
        value.X = x/polygonPoints.Count;
        value.Y = y/polygonPoints.Count;
        return value;
    }

    // private method sorting the list of points making up the polygon
    private void sortAngular()
    {
        Point averagePoint = averagePointInside();
        Comparison<Point> comparisonPoint = (a, b) => {
            double atan1 = Math.Atan2(a.Y - averagePoint.Y, a.X - averagePoint.X);
            double atan2 = Math.Atan2(b.Y - averagePoint.Y, b.X - averagePoint.X);
            double angle1 = (atan1* (180/Math.PI) + 360) % 360;
            double angle2 = (atan2 * (180 / Math.PI) + 360) % 360;

            return (int)(angle2 - angle1);
        };
        polygonPoints.Sort(comparisonPoint);
    }

    // method calculating the area of a polygon
    public SqlDouble area()
    {
        sortAngular();
        int firstsum = 0;
        int secondsum = 0;

        for (int i = 0; i < polygonPoints.Count; i++)
        {
            int sindex = (i + 1) % polygonPoints.Count;
            int prod = polygonPoints[i].X * polygonPoints[sindex].Y;
            firstsum += prod;
        }

        for (int i = 0; i < polygonPoints.Count; i++)
        {
            int sindex = (i + 1) % polygonPoints.Count;
            int prod = polygonPoints[sindex].X * polygonPoints[i].Y;
            secondsum += prod;
        }
        
        return Math.Abs(0.5*(firstsum - secondsum));
    }

    // method checking if a point is inside a polygon - crossing number test
    public SqlBoolean pointInsidePolygon(Point P)
    {
        int counter = 0;
        List<Point> V = new List<Point>();
        foreach (var point in polygonPoints)
        {
            V.Add(point);
        }
        V.Add(polygonPoints[0]);
        // loop through all edges of the polygon
        for (int i = 0; i < V.Count-1; i++)
        { 
            if (((V[i].Y <= P.Y) && (V[i + 1].Y > P.Y))
             || ((V[i].Y > P.Y) && (V[i + 1].Y <= P.Y)))
            { 
                double vt = (double)(P.Y - V[i].Y) / (V[i + 1].Y - V[i].Y);
                if (P.X < V[i].X + vt * (V[i + 1].X - V[i].X))
                    ++counter;
            }
        }
        if (counter == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // method of validating if the created polygon is correct
    private bool isPolygonCorrect()
    {
        for (int k = 0; k < polygonPoints.Count; k++)
        {
            int previousX = polygonPoints[k].X;
            int previousY = polygonPoints[k].Y;
            for (int i = k+1; i < polygonPoints.Count; i++)
            {
                if (previousX == polygonPoints[i].X && previousY == polygonPoints[i].Y)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // method to parse the string from the SQL command to create a new object
    public static Polygon Parse(SqlString value)
    {
        
        if (value.IsNull)
            return Null;
        Polygon newPolygon = new Polygon();
  
        string[] stringArrayPoints = value.Value.Split("/".ToCharArray());
        if (stringArrayPoints.Length < 3)
        {
            throw new ArgumentException("Invalid Polygon. Provide more then 3 vertices.");
        }
        List<Point> listOfPoints = new List<Point>();
        foreach (var point in stringArrayPoints)
        {
            string[] separate = point.Split(";".ToCharArray());
            Point temp = new Point();
            temp.X = Int32.Parse(separate[0]);
            temp.Y = Int32.Parse(separate[1]);
            listOfPoints.Add(temp);

        }
        newPolygon.polygonPoints = listOfPoints;
        if (!newPolygon.isPolygonCorrect())
        {
            throw new ArgumentException("Invalid Polygon. Some vertices are equal.");
        }
        
        return newPolygon;
    }

    // method serializing the object
    public void Write(System.IO.BinaryWriter value)
    {
        StringBuilder stringPoints = new StringBuilder();

        for (int i = 0; i < polygonPoints.Count-1; i++)
        {
            stringPoints.Append(polygonPoints[i].ToString());
            stringPoints.Append("/");
        }

        stringPoints.Append(polygonPoints[polygonPoints.Count-1]);

        int maxStringSize = 255;
        string paddedString;
        paddedString = stringPoints.ToString().PadRight(maxStringSize, '\0');
        for (int i = 0; i < paddedString.Length; i++)
        {
            value.Write(paddedString[i]);
        }
    }

    // method to deserialize the object
    public void Read(System.IO.BinaryReader value)
    {
        char[] chars;
        int maxStringSize = 255;
        int stringEnd;
        string stringValue;

        chars = value.ReadChars(maxStringSize);
        stringEnd = Array.IndexOf(chars, '\0');
        if (stringEnd == 0)
        {
            stringValue = null;
            return;
        }
        stringValue = new String(chars, 0, stringEnd);
        
        string[] stringArrayPoints = stringValue.Split("/".ToCharArray());
        List<Point> listOfPoints = new List<Point>();
        
        foreach(var point in stringArrayPoints){
            string [] separate = point.Split(";".ToCharArray());
            Point temp = new Point();
            temp.X = Int32.Parse(separate[0]);
            temp.Y = Int32.Parse(separate[1]);
            listOfPoints.Add(temp);  
        } 
        polygonPoints = listOfPoints;
    }
}


