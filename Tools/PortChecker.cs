using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneTool.Tools
{
    public class PortChecker
    {
        public bool IsQualcommPort(string portName)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%" + portName + "%'");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if (queryObj["Manufacturer"] != null && queryObj["Manufacturer"].ToString().Contains("Qualcomm"))
                    {
                        // Found a Qualcomm port
                        return true;
                    }
                    if (queryObj["Description"] != null && queryObj["Description"].ToString().Contains("Qualcomm"))
                    {
                        // Found a Qualcomm port
                        return true;
                    }
                }
            }
            catch (ManagementException ex)
            {
                // Handle exception
                MessageBox.Show("An error occurred while querying for WMI data: " + ex.Message);
            }
            return false;
        }
    }
}
