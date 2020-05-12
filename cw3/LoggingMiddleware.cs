using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;

 public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            var requestMethod = string.Empty;
            requestMethod = httpContext.Request.Method.ToString();
            var path = string.Empty;
            path = httpContext.Request.Path.ToString();
            var Body = string.Empty;
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                Body = await reader.ReadToEndAsync();
            }
            var Query = string.Empty;
            Query = httpContext.Request.QueryString.ToString();

            string sciezka = "requestsLog.txt";
            using (StreamWriter sw = !File.Exists(sciezka) ? File.CreateText(sciezka) : File.AppendText(sciezka))
            {
                 sw.WriteLine(requestMethod);
                 sw.WriteLine(path);
            if (Body.Length != 0)
            {
                sw.WriteLine(Body);
            }
            else
            {
                sw.WriteLine("Brak treści w body");
            }
            if (Query.Length != 0)
            {
                sw.WriteLine(Query);
            }
            else
            {
                sw.WriteLine("Brak treści w query");
            }
            sw.WriteLine();
            }
            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            await _next(httpContext);
        }
    }

