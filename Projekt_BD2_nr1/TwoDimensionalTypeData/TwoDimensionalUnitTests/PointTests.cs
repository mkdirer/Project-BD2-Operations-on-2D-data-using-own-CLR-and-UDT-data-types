using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace TwoDimensionalUnitTests
{
    [TestClass]
    public class PointTests
    {

      
        static string sqlconnection = "Data Source=MSSQLSERVER008;Initial Catalog=newDB2D;Integrated Security=True";
        static SqlConnection conn = new SqlConnection(sqlconnection);
        
        
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            String sqlcommand = "create table TablePoint ( point dbo.Point);"
                              + "insert into TablePoint (point) values ('2;3');";
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
            
            String sqlcommand = "DROP TABLE TablePoint;";
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
        public void TestPointToString()
        {
            string query = "select point.ToString() as toStringTest from dbo.TablePoint;";
            string expected = "2;3";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["toStringTest"]);
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestGetXFromPoint()
        {
            string query = "select point.X as xvalue from dbo.TablePoint;";
            int expected = 2;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["xvalue"]);
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestGetYFromPoint()
        {
            string query = "select point.Y as yvalue from dbo.TablePoint;";
            int expected = 3;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["yvalue"]);
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestDistanceOfPoints()
        {
            
            string query = @"declare @tempPoint as dbo.Point;
                            select top 1 @tempPoint = point from dbo.TablePoint;
                            select point.distance(@tempPoint) as DistanceTest from dbo.TablePoint;";
            double expected = 0;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["DistanceTest"]);
                }
            }
            catch (SqlException except)
            {
                Console.WriteLine(except.Message);
            }
            finally { conn.Close(); }
        }
        
        [TestMethod]
        public void TestPointTryToInsertStringPoint()
        {
            string query = @"insert into TablePoint (point) values ('test;5');";
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
        public void TestPointTryToInsertDoublePoint()
        {
            string query = @"insert into TablePoint (point) values ('3.3;7');";
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
         
    }
}
