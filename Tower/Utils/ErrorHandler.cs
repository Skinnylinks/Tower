using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Tower.Utils
{
    public class ErrorHandler
    {
        public void ErrorLog(Exception ex)
        {
            //Fix Error Send Procedure to log

            string path = "\\Errors\\error.txt";
            string filePath = System.Web.HttpContext.Current.Server.MapPath(path);
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                if (ex != null)
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);

                }
                else
                {
                    writer.WriteLine("Message :Unknown Error" +
                      "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
        }


        public void ErrrorText(string exstring)
        {


            string path = "\\Errors\\error.txt";
            string filePath = System.Web.HttpContext.Current.Server.MapPath(path);
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + exstring + "<br/>" + Environment.NewLine +
                   "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);


            }

        }



    }
}