using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace KJGarmentsWeb
{
    public class WebUILogging
    {
        // Add your favourite logger here

        public static void LogMessage(string message)
        {
            DoTrace(message);
        }

        public static void LogLongMessage(string message, string longMessage)
        {
            DoTrace(message);
            DoTrace(longMessage);
        }

        public static void LogException(string message, Exception ex)
        {
            DoTrace(message);
            DoTrace(ex.Message);
            DoTrace(ex.StackTrace);
        }

        private static void DoTrace(string message)
        {
            Trace.WriteLine(DateTime.Now + " - " + message);
        }
    }
}
