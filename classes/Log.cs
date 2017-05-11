using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ReceiptExport
{
    public static class Log
    {
        private static string filePath;
        private static StreamWriter logWriter;
        static string errMsg;

        private static DateTime currentDateTime;
       

        public static string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public static DateTime CurrentDateTime
        {
            get {
                currentDateTime = DateTime.Now;
                return currentDateTime; 
            }
            set { currentDateTime = value; }
        }

        public static void Exit(string _errMsg, ExitCode exitCode)
        {
            try
            {
                bool fileExists = File.Exists(filePath);
                if (fileExists)
                {
                    logWriter.WriteLine(_errMsg);
                    logWriter.Close();
                }
            }
            finally
            {
                Program.EmailNotification((int)exitCode, _errMsg);
                System.Environment.Exit((int)exitCode);
            }
        }

        public static void WriteLine(string text)
        {
            logWriter.WriteLine(text);
        }

        //write Write process here for successful loggin
        public static void CreateLogFile(string fileDir)
        {
            try
            {
                string strDate = String.Format("{0:MMddyyyy}", CurrentDateTime);
                FilePath = String.Format("{0}ReceiptExport_{1}.log", fileDir, strDate);

                logWriter = new StreamWriter(filePath);

                logWriter.WriteLine(String.Format("----- Start: {0} -----", DateTime.Now.ToString()));
            }
            catch
            {
                throw;                
            }
        }

        public static void Close()
        {
            logWriter.WriteLine(String.Format("----- Done: {0} -----", DateTime.Now.ToString()));
            logWriter.Close();
        }
        public static bool IsLogWriterNull()
        {
            return logWriter == null ? true : false;
        }

    }
}
