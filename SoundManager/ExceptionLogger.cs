using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SharpTools
{
    /// <summary>
    /// Simple error log generator. Logs all exceptions to the specified log file.
    /// By ORelio - (c) 2026 - Available under the CDDL-1.0 license
    /// </summary>
    static class ExceptionLogger
    {
        private static object LogLock = new object();
        private static string LogFile = null;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// Start logging exceptions to the specified log file.
        /// </summary>
        /// <param name="logFile">Destination log file. Example: Application.ExecutablePath + ".debug.log"</param>
        /// <param name="appVersion">Application version to include in log file header</param>
        public static void StartLogging(string logFile, string appVersion)
        {
            try
            {
                LogFile = logFile;
                File.WriteAllLines(LogFile, new[] { "-- Starting debug log --", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffffK"), "Version: " + appVersion });
                AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            catch (Exception e)
            {
                string error = String.Format("ExceptionLogger: Failed to initialize logging to '{0}': {1}\n{2}\n\n{3}",
                    logFile,
                    e.GetType().Name,
                    e.Message,
                    e.StackTrace
                );

                if (GetConsoleWindow() != IntPtr.Zero)
                {
                    Console.Error.WriteLine(error);
                }
                else
                {
                    MessageBox.Show(error, "ExceptionLogger", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// Log all exceptions, including caught exceptions
        /// </summary>
        private static void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            LogException("Exception", e.Exception);
        }

        /// <summary>
        /// Log unhandled exception just before application crashes
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException("Unhandled Exception", e.ExceptionObject as Exception);
        }

        /// <summary>
        /// Log exception (internal)
        /// </summary>
        private static void LogException(string header, Exception e)
        {
            List<string> errorLines = new List<string>();

            errorLines.Add("");
            errorLines.Add("-- " + header + " --");
            errorLines.Add(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffffK"));
            errorLines.Add(e.GetType().Name + ": " + e.Message);
            errorLines.Add(e.StackTrace);

            lock (LogLock)
            {
                File.AppendAllLines(LogFile, errorLines);
            }
        }
    }
}
