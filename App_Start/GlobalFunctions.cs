using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Net;
using System.Net.Mail;

/// <summary>
/// Summary description for GlobalFunctions
/// </summary>
public static class GlobalFunctions
{
    #region Public
    public static void LogError(Exception ex)
    {
        try
        {
            string strWebAppPath = HttpContext.Current.Server.MapPath(".") + "\\logs";
            if (!Directory.Exists(strWebAppPath))
            {
                Directory.CreateDirectory(strWebAppPath);
            }
            strWebAppPath += "\\" + DateTime.Now.ToString("MMddyyyy") + ".log";
            if (!File.Exists(strWebAppPath))
            {
                // Create a new file to write to.
                using (StreamWriter objStreamWriter = File.CreateText(strWebAppPath))
                {
                    objStreamWriter.WriteLine(DateTime.Now.ToString());
                    objStreamWriter.WriteLine(ex.Message.ToString());
                    objStreamWriter.WriteLine(ex.StackTrace.ToString());
                    objStreamWriter.WriteLine("-------------------------------------------------------");
                    objStreamWriter.Close();
                }
            }
            else
            {
                // Append in exisiting file.
                using (StreamWriter objStreamWriter = File.AppendText(strWebAppPath))
                {
                    objStreamWriter.WriteLine(DateTime.Now.ToString());
                    objStreamWriter.WriteLine(ex.Message.ToString());
                    objStreamWriter.WriteLine(ex.StackTrace.ToString());
                    objStreamWriter.WriteLine("-------------------------------------------------------");
                    objStreamWriter.Close();
                }
            }
        }
        catch
        {
        }
    }
    public static void LogError(System.Data.SqlClient.SqlException ex)
    {
        try
        {
            string strWebAppPath = HttpContext.Current.Server.MapPath(".") + "\\logs";
            if (!Directory.Exists(strWebAppPath))
            {
                Directory.CreateDirectory(strWebAppPath);
            }
            strWebAppPath += "\\" + DateTime.Now.ToString("MMddyyyy") + ".log";
            if (!File.Exists(strWebAppPath))
            {
                // Create a new file to write to.
                using (StreamWriter objStreamWriter = File.CreateText(strWebAppPath))
                {
                    objStreamWriter.WriteLine(DateTime.Now.ToString());
                    foreach (System.Data.SqlClient.SqlError myError in ex.Errors)
                    {
                        objStreamWriter.WriteLine(myError.Message.ToString());
                    }
                    objStreamWriter.WriteLine("-------------------------------------------------------");
                    objStreamWriter.Close();
                }
            }
            else
            {
                // Append in exisiting file.
                using (StreamWriter objStreamWriter = File.AppendText(strWebAppPath))
                {
                    objStreamWriter.WriteLine(DateTime.Now.ToString());
                    foreach (System.Data.SqlClient.SqlError myError in ex.Errors)
                    {
                        objStreamWriter.WriteLine(myError.Message.ToString());
                    }
                    objStreamWriter.WriteLine("-------------------------------------------------------");
                    objStreamWriter.Close();
                }
            }
        }
        catch
        {
        }
    }

    public static void LogError(string sError)
    {
        try
        {
            string strWebAppPath = HttpContext.Current.Server.MapPath(".") + "\\logs";
            if (!Directory.Exists(strWebAppPath))
            {
                Directory.CreateDirectory(strWebAppPath);
            }
            strWebAppPath += "\\" + DateTime.Now.ToString("MMddyyyy") + ".log";
            if (!File.Exists(strWebAppPath))
            {
                // Create a new file to write to.
                using (StreamWriter objStreamWriter = File.CreateText(strWebAppPath))
                {
                    objStreamWriter.WriteLine(DateTime.Now.ToString());
                    objStreamWriter.WriteLine(sError);
                    objStreamWriter.WriteLine("-------------------------------------------------------");
                    objStreamWriter.Close();
                }
            }
            else
            {
                // Append in exisiting file.
                using (StreamWriter objStreamWriter = File.AppendText(strWebAppPath))
                {
                    objStreamWriter.WriteLine(DateTime.Now.ToString());
                    objStreamWriter.WriteLine(sError);
                    objStreamWriter.WriteLine("-------------------------------------------------------");
                    objStreamWriter.Close();
                }
            }
        }
        catch
        {
        }
    }

    public static string FormatPhoneFax(string PhoneFaxNumber)
    {
        try
        {
            string strPhoneFaxNumber;
            PhoneFaxNumber = PhoneFaxNumber.Trim().Replace("-", "").Replace(" ", "");
            strPhoneFaxNumber = PhoneFaxNumber;
            if (PhoneFaxNumber == "00000000000000" || PhoneFaxNumber == "0000000000" || PhoneFaxNumber == "" || PhoneFaxNumber == "&nbsp;")
            {
                return "";
            }
            if (PhoneFaxNumber.Length > 10)
            {
                strPhoneFaxNumber = PhoneFaxNumber.Substring(0, 3) + "-" + PhoneFaxNumber.Substring(3, 3) + "-" + PhoneFaxNumber.Substring(6, 4);
                if (PhoneFaxNumber.Substring(10) != "0000" && PhoneFaxNumber.Substring(10) != "000" && PhoneFaxNumber.Substring(10) != "00" && PhoneFaxNumber.Substring(10) != "0")
                {
                    strPhoneFaxNumber += " ext " + PhoneFaxNumber.Substring(10);
                }
            }
            else if (PhoneFaxNumber.Length > 7)
            {
                strPhoneFaxNumber = PhoneFaxNumber.Substring(0, 3) + "-" + PhoneFaxNumber.Substring(3, 3) + "-" + PhoneFaxNumber.Substring(6);
            }
            return strPhoneFaxNumber;
        }
        catch (Exception Ex)
        {
            LogError(Ex);
            return PhoneFaxNumber;
        }
    }
    public static string GetNumericFromString(string strInput)
    {
        try
        {
            string rem = "";
            if (IsNumeric(strInput))
            {
                return strInput;
            }
            else if (IsNumeric(strInput) == false)
            {
            AGAIN:
                rem = rem + strInput.Remove(1);
                strInput = strInput.Substring(1);
                if (IsNumeric(strInput) == false)
                {
                    goto AGAIN;
                }
            }
            return strInput;
        }
        catch (Exception Ex)
        {
            LogError(Ex);
            return strInput;
        }
    }
    public static bool IsNumeric(string s)
    {
        try
        {
            Int32.Parse(s);
        }
        catch
        {
            return false;
        }
        return true;
    }
    public static string getNodeData(string Data, string BeginingNode, string EndingNode, int StartingIndex)
    {
        try
        {
            int intIndexStarting = Data.IndexOf(BeginingNode, StartingIndex);
            if (intIndexStarting > -1)
            {
                intIndexStarting = intIndexStarting + BeginingNode.Length;
                int intIndexEnding = Data.IndexOf(EndingNode, intIndexStarting);
                return Data.Substring(intIndexStarting, intIndexEnding - intIndexStarting).Trim();
            }
            return "";
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    public static string CheckTopOrder()
    {
        try
        {
            return ConfigurationManager.AppSettings["TopOrder"].ToString();
        }
        catch (Exception ex)
        { return "0"; }
    }

    #endregion

    #region Private
    #endregion
}
