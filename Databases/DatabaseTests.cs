using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Databases
{
    [TestClass]
    public class DatabaseTests
    {
        readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        readonly string sqlSelect = "SELECT @@VERSION";

        [TestMethod]
        public void Test_DB_Synchronous()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand(sqlSelect, sqlConnection))
                {
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var data = reader[0].ToString();
                        }
                    }
                }
            }
        }

        [TestMethod]
        public async Task Test_DB_AsynchronousWithAsyncAwaitPattern()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                using (var sqlCommand = new SqlCommand(sqlSelect, sqlConnection))
                {
                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var data = reader[0].ToString();
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Test_DB_Asynchronous()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                var sqlCommand = new SqlCommand(sqlSelect, sqlConnection);

                var callback = new AsyncCallback(SqlCommandCompleted);

                IAsyncResult ar = sqlCommand.BeginExecuteReader(callback, sqlCommand);

                // Wait until asyncresult tells us that the operation we wanted has completed
                ar.AsyncWaitHandle.WaitOne();
            }
        }

        private static void SqlCommandCompleted(IAsyncResult ar)
        {
            // This will run in a background thread
            var sqlCommand = ar.AsyncState as SqlCommand;

            using (var reader = sqlCommand.EndExecuteReader(ar))
            {
                while (reader.Read())
                {
                    var data = reader[0].ToString();
                }
            }
        }

        [TestMethod]
        public void Test_DB_AsynchronousWithTasks()
        {
            var sqlConnection = new SqlConnection(connectionString);
            
            Task taskSqlConnection = sqlConnection.OpenAsync();

            taskSqlConnection.ContinueWith((Task tx, object state) => 
            {
                var sqlConn = state as SqlConnection; // what is the state? it is the passed sqlConnection from L.92

                // Just to be sure that the connection to the db is open
                Assert.IsTrue(sqlConn.State == ConnectionState.Open);

                var sqlCommand = new SqlCommand(sqlSelect, sqlConn);

                Task<SqlDataReader> sqlDataReaderTask = sqlCommand.ExecuteReaderAsync();

                Task taskForProcessingResult = sqlDataReaderTask.ContinueWith(continationTask => 
                {
                    using (var reader = continationTask.Result)
                    {
                        while (reader.Read())
                        {
                            var data = reader[0].ToString();
                        }

                        // Send the signal that all work has been done
                        manualResetEvent.Set();
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }, sqlConnection, TaskContinuationOptions.OnlyOnRanToCompletion);

            // Wait for the signal that all the background tasks have completed
            manualResetEvent.WaitOne();
        }

        ManualResetEvent manualResetEvent = new ManualResetEvent(false);
    }
}
