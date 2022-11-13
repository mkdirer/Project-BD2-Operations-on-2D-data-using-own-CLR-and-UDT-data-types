using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace MainProgram
{
    // Main class of the console API program
    class Program
    {

        static String sqlconnection = "Data Source=MSSQLSERVER008;Initial Catalog=newDB2D;Integrated Security=True";

        
        static void Main(string[] args)
        {
            bool displayMenu = true;
            while (displayMenu)
            {
                displayMenu = AppMenu();
            }
        }

        // main menu application
        private static bool AppMenu()
        {
            Console.Clear();
            Console.WriteLine("-----------------------------------------------------------\n");
            Console.WriteLine("-  Project BD2 Operation on two-dimensional coordinates.  -\n");
            Console.WriteLine("-----------------------------------------------------------\n");
            Console.WriteLine("-                 1. Insert Point                         -\n");
            Console.WriteLine("-                 2. Insert Polygon                       -\n");
            Console.WriteLine("-                 3. Display List of Points               -\n");
            Console.WriteLine("-                 4. Display List of Polygons             -\n");
            Console.WriteLine("-                 5. Remove Point                         -\n");
            Console.WriteLine("-                 6. Remove Polygon                       -\n");
            Console.WriteLine("-                 7. Polygon area                         -\n");
            Console.WriteLine("-                 8. Check if Point inside Polygon        -\n");
            Console.WriteLine("-                 9. Distance between points              -\n");
            Console.WriteLine("-                 0. Exit                                 -\n");
            Console.Write("\r\n Please choose option: ");

            switch (Console.ReadLine())
            {
                case "1"://insert point
                    Console.WriteLine("! Enter coordinates in format: x;y (for example: 1;2)");
                    String value = Console.ReadLine();
                    if (!String.IsNullOrEmpty(value))
                        addPoint(value);
                    Console.WriteLine("! Press Enter to continue...");
                    Console.ReadLine();
                    return true;
                case "2"://insert polygon
                    Console.WriteLine("! Put [a] if you want to use points from table or [l]  to enter points manually");
                    String flag = Console.ReadLine();
                    if (flag.Equals("a"))
                    {
                        getPoints();
                        Console.WriteLine("! Select points ID's (for example: 1,2,3)");
                        string listOfId = Console.ReadLine();
                        if (!String.IsNullOrEmpty(listOfId))
                            addPolygonUsingPointFromTable(listOfId);
                        Console.WriteLine("! Press Enter to continue...");
                        Console.ReadLine();
                    }
                    else if (flag.Equals("l"))
                    {
                        Console.WriteLine("! Enter coordinates in format: x1;y1/x2;y2\n (for example: 1;2/2;4/6;6)");
                        string listOfPoints = Console.ReadLine();
                        addManuallyPolygon(listOfPoints);
                        Console.WriteLine("! Press Enter to continue...");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Wrong parameter;");
                    }
                    return true;
                case "3"://list points
                    Console.WriteLine("All points:\n");
                    getPoints();
                    Console.WriteLine("! Press Enter to continue...");
                    Console.ReadLine();
                    return true;

                case "4"://list polygons
                    Console.WriteLine("All polygons:\n");
                    getPolygons();
                    Console.WriteLine("! Press Enter to continue...");
                    Console.ReadLine();
                    return true;
                case "5"://delete points
                    getPoints();
                    Console.WriteLine("\n! Select point ID (for example: 1)");
                    string deletePointID = Console.ReadLine();
                    if (deletePointID.All(char.IsDigit) && !String.IsNullOrEmpty(deletePointID))
                        removePoint(deletePointID);
                    Console.WriteLine("! Press Enter to continue...");
                    Console.ReadLine();
                    return true;

                case "6"://delete polygon
                    getPolygons();
                    Console.WriteLine("\n! Select polygon ID (for example: 1)");
                    string deletePolygonID = Console.ReadLine();
                    if (deletePolygonID.All(char.IsDigit) && !String.IsNullOrEmpty(deletePolygonID))
                        removePolygon(deletePolygonID);
                    Console.WriteLine("! Press Enter to continue...");
                    Console.ReadLine();
                    return true;
                case "7"://polygon area
                    getPolygons();
                    Console.WriteLine("! Select polygon ID (for example: 1)");
                    string idValue = Console.ReadLine();
                    if (idValue.All(char.IsDigit) && !String.IsNullOrEmpty(idValue))
                        returnAreaValueFromPolygon(idValue);
                    else
                    {
                        Console.WriteLine("! Wrong parameter");
                        Console.WriteLine("! Press Enter to continue...");
                        Console.ReadLine();
                        return true;
                    }


                    Console.WriteLine("! Press Enter to continue...");
                    Console.ReadLine();
                    return true;
                case "8"://point insite polygon
                    getPolygons();
                    Console.WriteLine("\n! Select polygon ID (for example: 1)");
                    string idPolygon = Console.ReadLine();
                    Console.WriteLine("\n");
                    getPoints();
                    Console.WriteLine("\n! Select point ID (for example: 1)");
                    string idPoint = Console.ReadLine();
                    if (idPolygon.All(char.IsDigit) && idPoint.All(char.IsDigit) && !String.IsNullOrEmpty(idPoint))
                        pointInsidePolygon(idPolygon, idPoint);
                    else
                    {
                        Console.WriteLine("! Wrong parameters");
                        Console.WriteLine("! Press Enter to continue...");
                        Console.ReadLine();
                        return true;
                    }
                    Console.WriteLine("! Press Enter to continue...");
                    Console.ReadLine();
                    return true;

                case "9"://points distance
                    getPoints();
                    Console.WriteLine("\n! Select 1st point ID (for example: 1)");
                    string firstPoint = Console.ReadLine();
                    Console.WriteLine("\n! Select 2st point ID (for example: 2)");
                    string secondPoint = Console.ReadLine();
                    if (firstPoint.All(char.IsDigit) && secondPoint.All(char.IsDigit) && !String.IsNullOrEmpty(firstPoint) && !String.IsNullOrEmpty(secondPoint))
                        distanceOfPoints(firstPoint, secondPoint);
                    else
                    {
                        Console.WriteLine("! Wrong parameters");
                        Console.WriteLine("! Press Enter to continue...");
                        Console.ReadLine();
                        return true;
                    }
                    Console.WriteLine("! Press Enter to continue...");
                    Console.ReadLine();
                    return true;
                case "0":
                    return false;
                default:
                    return true;
            }
        }

        // method displaying all points from the database
        private static void getPoints()
        {
            String query = "select ID, point.ToString() as point from dbo.Point;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                Console.WriteLine("\n##########################");
                while (datareader.Read())
                {
                    string id = datareader["ID"].ToString();
                    Console.WriteLine("ID = " + id + ": " + datareader["point"].ToString());
                     
                }
                Console.WriteLine("##########################\n");

            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        // method to introduce a new point to the database
        private static void addPoint(string value)
        {
            String query = "insert into dbo.Point (point) values (@value);";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {

                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@value", SqlDbType.VarChar);
                command.Parameters["@value"].Value = value;


                Int32 numberRows = command.ExecuteNonQuery();
                Console.WriteLine("Affected to rows: {0}", numberRows);
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        // method deleting the point
        private static void removePoint(string idPoint)
        {
            String query = "delete from dbo.Point where ID = @idPoint;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {

                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@idPoint", SqlDbType.Int);
                command.Parameters["@idPoint"].Value = int.Parse(idPoint);

                Int32 numberRows = command.ExecuteNonQuery();
                Console.WriteLine("Removed rows: {0}", numberRows);
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        // method displaying all polygons from the database
        private static void getPolygons()
        {
            String query = "select ID, polygon.ToString() as polygon from dbo.Polygon;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                Console.WriteLine("\n##########################");

                while (datareader.Read())
                {
                    string id = datareader["ID"].ToString();
                    Console.WriteLine("ID=" + id + ": " + datareader["polygon"].ToString());
                }
                Console.WriteLine("##########################\n");

            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        // method introducing a new polygon to the database (points given manually)
        private static void addManuallyPolygon(string value)
        {
            String query = "insert into dbo.Polygon (polygon) values (@value);";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {

                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@value", SqlDbType.VarChar);
                command.Parameters["@value"].Value = value;


                Int32 numberRows = command.ExecuteNonQuery();
                Console.WriteLine("Affected to rows: {0}", numberRows);
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        // method introducing a new polygon to the database (points selected from the Point table)
        private static void addPolygonUsingPointFromTable(string value)
        {
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {

                conn.Open();
                SqlCommand command = new SqlCommand("dbo.addPolygonUsingPointFromTable", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Sequence",value));

                Int32 numberRows = command.ExecuteNonQuery();
                Console.WriteLine("Affected to rows: 1");
            }
            catch (SqlException except)
            {

                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        // method deleting a polygon
        private static void removePolygon (string idPolygon)
        {
            String query = "delete from dbo.Polygon where ID = @idPolygon;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {

                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@idPolygon", SqlDbType.Int);
                command.Parameters["@idPolygon"].Value = int.Parse(idPolygon);
                Int32 numberRows = command.ExecuteNonQuery();
                Console.WriteLine("Removed rows: {0}", numberRows);
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        // method displaying the result of the query about the content of a point in a polygon
        private static void pointInsidePolygon(string idPolygon, string idPoint)
        {
            String query = @"declare @tempPoint as dbo.Point;
                            select @tempPoint = point from dbo.Point where ID = @idPoint;
                            select polygon.pointInsidePolygon(@tempPoint) as status from dbo.Polygon where ID = @idPolygon;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@idPoint", SqlDbType.Int);
                command.Parameters.Add("@idPolygon", SqlDbType.Int);

                command.Parameters["@idPoint"].Value = int.Parse(idPoint);
                command.Parameters["@idPolygon"].Value = int.Parse(idPolygon);

                SqlDataReader datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    string stringStatus = datareader["status"].ToString();
                    bool pointInside = bool.Parse(stringStatus);
                    if (pointInside)
                    {
                        Console.WriteLine("Point ID = " + idPoint + " is inside Polygon ID = " + idPolygon + "\n");
                    }
                    else
                    {
                        Console.WriteLine("Point ID = " + idPoint + " is outside Polygon ID = " + idPolygon + "\n");
                    }
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        // method displaying the returned polygon area
        private static void returnAreaValueFromPolygon(string idPolygon)
        {
            String query = "select ID, polygon.area() as area from dbo.Polygon where ID = @ID;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = int.Parse(idPolygon);

                SqlDataReader datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    string id = datareader["ID"].ToString();
                    Console.WriteLine("Polygon with ID = " + id + " has area = " + datareader["area"].ToString());
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        // method displaying distance between two points
        private static void distanceOfPoints(string firstPoint, string secondPoint)
        {
            String query = @"declare @tempPoint as dbo.Point;
                            select @tempPoint = point from dbo.Point where ID = @firstPoint;
                            select point.distance(@tempPoint) as distance from dbo.Point where ID = @secondPoint;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@firstPoint", SqlDbType.Int);
                command.Parameters.Add("@secondPoint", SqlDbType.Int);

                command.Parameters["@firstPoint"].Value = int.Parse(firstPoint);
                command.Parameters["@secondPoint"].Value = int.Parse(secondPoint);

                SqlDataReader datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    string distance = datareader["distance"].ToString();
                    Console.WriteLine("\nDistance between points ID1 = " + firstPoint + " and ID2 = " + secondPoint + "  is = " + distance + "\n");
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

    }
}
