using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OneTool.Tools
{
    public static class checkInternet
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                 WebClient webClient = new WebClient();
                using (webClient.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
