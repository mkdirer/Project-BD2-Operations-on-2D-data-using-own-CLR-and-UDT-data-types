using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace TwoDimensionalUnitTests
{
    [TestClass]
    public class PolygonTests
    {
        static string sqlconnection = "Data Source=MSSQLSERVER008;Initial Catalog=newDB2D;Integrated Security=True";
        static SqlConnection conn = new SqlConnection(sqlconnection);


        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            String sqlcommand = @"create table PolygonTable ( polygon dbo.Polygon);
                                insert into PolygonTable (polygon) values ('0;0/0;2/4;2/4;0');
                                create table TablePointNew (point dbo.Point);
                                insert into TablePointNew (point) values ('2;1');
                                insert into TablePointNew (point) values ('6;6');";

            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(sqlcommand, conn);
                SqlDataReader datareader = command.ExecuteReader();
                datareader.Read();
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }


        [ClassCleanup()]
        public static void ClassCleanup()
        {

            String sqlcommand = @"DROP TABLE PolygonTable;
                                  DROP TABLE TablePointNew;";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(sqlcommand, conn);
                SqlDataReader datareader = command.ExecuteReader();
                datareader.Read();
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestAreaOfPolygon()
        {
            string query = "select polygon.area() as AreaTest from dbo.PolygonTable;";
            double expected = 8;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["AreaTest"]);
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestPolygonToString()
        {
            string query = "select polygon.ToString() as ToStringTest from dbo.PolygonTable;";
            string expected = "[0;0][0;2][4;2][4;0]";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["ToStringTest"]);
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestTrueForPolygonPointInside()
        {
            string query = @"declare @tempPoint as dbo.Point;
                            select top 1 @tempPoint = point from dbo.TablePointNew where point.X = 2 and point.Y = 1;
                            select polygon.pointInsidePolygon(@tempPoint) as IsInsidePlygonTest from dbo.PolygonTable;";
            bool expected = true;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["IsInsidePlygonTest"]);
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestFalseForPolygonPointInside()
        {
            string query = @"declare @tempPoint as dbo.Point;
                            select top 1 @tempPoint = point from dbo.TablePointNew where point.X = 6 and point.Y = 6;
                            select polygon.pointInsidePolygon(@tempPoint) as IsInsidePlygonTest from dbo.PolygonTable;";
            bool expected = false;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["IsInsidePlygonTest"]);
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestInsertWithEqualVerticesToPolygon()
        {
            string query = @"insert into dbo.PolygonTable (polygon) values ('0;0/0;0/0;0');";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                int status = command.ExecuteNonQuery();
                Assert.Fail();
            }
            catch (SqlException except)
            {
                Console.WriteLine("Test Passed");
                Console.WriteLine(except.Message);
            }
            catch (Exception except)
            {
                Assert.Fail();
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestInsertWithLessThan3VerticesToPolygon()
        {
            string query = @"insert into dbo.PolygonTable (polygon) values ('0;0/0;2');";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                int status = command.ExecuteNonQuery();
                Assert.Fail();
            }
            catch (SqlException except)
            {
                Console.WriteLine("Test Passed");
                Console.WriteLine(except.Message);
            }
            catch (Exception except)
            {
                Assert.Fail();
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestPolygonTryToInsertDoublePoint()
        {
            string query = @"insert into dbo.PolygonTable (polygon) values ('0;0/0;2/3.3;4');";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                int status = command.ExecuteNonQuery();
                Assert.Fail();
            }
            catch (SqlException except)
            {
                Console.WriteLine("Test Passed");
                Console.WriteLine(except.Message);
            }
            catch (Exception except)
            {
                Assert.Fail();
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestPolygonTryToInsertStringPoint()
        {
            string query = @"insert into dbo.PolygonTable (polygon) values ('test;0/0;2/4;4');";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                int status = command.ExecuteNonQuery();
                Assert.Fail();
            }
            catch (SqlException except)
            {
                Console.WriteLine("Test Passed");
                Console.WriteLine(except.Message);
            }
            catch (Exception except)
            {
                Assert.Fail();
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestPolygonInsertTrue()
        {
            string query = @"insert into dbo.PolygonTable (polygon) values ('0;0/2;2/3;0');";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                int status = command.ExecuteNonQuery();
                if (status == 1)
                {
                    query = @"delete from dbo.PolygonTable where polygon.ToString() = '[0;0][2;2][3;0]';";
                }
                SqlCommand command1 = new SqlCommand(query, conn);
                command1.ExecuteNonQuery();

                Assert.AreEqual(1, status);

            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

    }
}
