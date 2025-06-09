using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Configuration;

/// <summary>
/// Summary description for ClsSessionErrorlog
/// </summary>
public class ClsSessionErrorlog
{
    public ClsSessionErrorlog()
    {
    }

    public static string strPath = AppDomain.CurrentDomain.BaseDirectory;
    public static string strLogFilePath = strPath + @"ErrorLog\Sessionlog.csv";


    public void WriteToLog(string msg)
    {
        if (ConfigurationManager.AppSettings["EnableMaSessionLog"].ToString() == "true")
        {
            try
            {
                if (!File.Exists(strLogFilePath))
                {
                    File.Create(strLogFilePath).Close();
                }
                using (StreamWriter w = File.AppendText(strLogFilePath))
                {
                    w.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                WriteToLog("Error writing to log file: " + ex.Message);
            }

        }
    }

}