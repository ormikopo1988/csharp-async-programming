using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Networking
{
    [TestClass]
    public class DownloadTests
    {
        // deelay is a Delay proxy for http resources
        // Slow loading resources (images, scripts, etc) can break your code. 
        // Test it simulating unexpected network conditions applied to specific resource.
        // See more in http://www.deelay.me/
        string url = "http://www.deelay.me/5000/https://github.com";

        [TestMethod]
        public void Test_Download_Synchronous()
        {
            var httpRequestInfo = HttpWebRequest.CreateHttp(url);

            // initial round trip to server - this is where the initial cost over the network was issued 
            var httpResponseInfo = httpRequestInfo.GetResponse();

            // downloading the page content
            var responseStream = httpResponseInfo.GetResponseStream();

            using (var sr = new StreamReader(responseStream))
            {
                var webPage = sr.ReadToEnd();
            }
        }

        [TestMethod]
        public async Task Test_Download_AsynchronousWithAsyncAwaitPattern()
        {
            var httpRequestInfo = HttpWebRequest.CreateHttp(url);

            // initial round trip to server - this is where the initial cost over the network was issued 
            var httpResponseInfo = await httpRequestInfo.GetResponseAsync();

            // downloading the page content
            var responseStream = httpResponseInfo.GetResponseStream();

            using (var sr = new StreamReader(responseStream))
            {
                var webPage = sr.ReadToEnd();
            }
        }

        [TestMethod]
        public void Test_Download_AsynchronousWithBeginEnd()
        {
            var httpRequestInfo = HttpWebRequest.CreateHttp(url);

            var callback = new AsyncCallback(HttpResponseAvailable);

            // initial round trip to server - this is where the initial cost over the network was issued 
            IAsyncResult ar = httpRequestInfo.BeginGetResponse(callback, httpRequestInfo);

            // We need here to explicitly tell the test framework that this test will not be 
            // completed until the download completes. That means there is a background thread 
            // and we need to wait for it to finish
            ar.AsyncWaitHandle.WaitOne();
        }

        private static void HttpResponseAvailable(IAsyncResult ar)
        {
            // This code here will be done asynchronously in a background thread
            var httpRequestInfo = ar.AsyncState as HttpWebRequest;

            var httpResponseInfo = httpRequestInfo.EndGetResponse(ar) as HttpWebResponse;

            // downloading the page content
            var responseStream = httpResponseInfo.GetResponseStream();

            using (var sr = new StreamReader(responseStream))
            {
                var webPage = sr.ReadToEnd();
            }
        }

        [TestMethod]
        public void Test_Download_AsynchronousWithTasks()
        {
            var httpRequestInfo = HttpWebRequest.CreateHttp(url);

            Task<WebResponse> taskWebResponse = httpRequestInfo.GetResponseAsync();

            Task taskContinuation = taskWebResponse.ContinueWith(HttpResponseAvailableWithTask, 
                TaskContinuationOptions.OnlyOnRanToCompletion); // run continuation only if the previous task completed successfully

            Task.WaitAll(taskWebResponse, taskContinuation);
        }

        private static void HttpResponseAvailableWithTask(Task<WebResponse> taskResponse)
        {
            var httpResponseInfo = taskResponse.Result as HttpWebResponse;
            
            var responseStream = httpResponseInfo.GetResponseStream();

            using (var sr = new StreamReader(responseStream))
            {
                var webPage = sr.ReadToEnd();
            }
        }
    }
}
