using OneTool.Qcom.Items.Base;
using OneTool.Qcom.Qualcomm;
using OneTool.Qcom.Qualcomm.QcdmManagers;
using OneTool.Qcom.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneTool.Tools
{
    public class LteSettings
    {


        //private List<int> selectedBands = new List<int>();
        //private List<int> UnselectedBands = new List<int>();

        //// Method to add band numbers to the selectedBands list
        //public void AddSelectedBand(int bandNumber)
        //{
        //    selectedBands.Add(bandNumber);
        //}

        //// Method to remove a band number from the selectedBands list
        //public void RemoveSelectedBand(int bandNumber)
        //{
        //    selectedBands.Remove(bandNumber);
        //}
        //public void AddUnSelectedBand(int bandNumber)
        //{
        //    UnselectedBands.Add(bandNumber);
        //}

        //// Method to remove a band number from the selectedBands list
        //public void RemoveUnSelectedBand(int bandNumber)
        //{
        //    UnselectedBands.Remove(bandNumber);
        //}
        // Function to disable bands based on the selectedBands list
        public byte[] DisableSelectedBands(LteBandsConfigBase bandsConfig , List<int> UnselectedBands)
        {
            foreach (int bandNumber in UnselectedBands)
            {
                switch (bandNumber)
                {
                    case 1:
                        bandsConfig.B1 = false;
                        break;
                    case 2:
                        bandsConfig.B2 = false;
                        break;
                    case 3:
                        bandsConfig.B3 = false;
                        break;
                    case 20:
                        bandsConfig.B20 = false;
                        break;
                    case 28:
                        bandsConfig.B28 = false;
                        break;
                    default:
                        break;
                }
            }

            // Convert the configuration to byte array and write to NV item
            byte[] data = BitConverter.GetBytes(bandsConfig.Value);
            return data;
        }

        // Function to enable bands based on the selectedBands list
        public byte[] EnableSelectedBands(LteBandsConfigBase bandsConfig, List<int> selectedBands)
        {
            foreach (int bandNumber in selectedBands)
            {
                switch (bandNumber)
                {
                    case 1:
                        bandsConfig.B1 = true;
                        break;
                    case 2:
                        bandsConfig.B2 = true;
                        break;
                          case 3:
                        bandsConfig.B3 = true;
                        break;
                    case 20:
                        bandsConfig.B20 = true;
                        break;
                    case 28:
                        bandsConfig.B28 = true;
                        break;
                    default:
                        // Handle invalid band numbers
                        break;
                }
            }

            // Convert the configuration to byte array and write to NV item
            byte[] data = BitConverter.GetBytes(bandsConfig.Value);
            return data;
        }
        public void ReadEnabledDisabledBands(byte[] nvData, out List<int> enabledBandsReturn, out List<int> disabledBandsReturn)
        {


            LteBandsConfigBase bandsConfig = new LteBandsConfigBase();
            bandsConfig.Value = BitConverter.ToUInt64(nvData, 0); // Assuming the data is stored as a UInt64

            List<int> enabledBands, disabledBands;
            bandsConfig.GetEnabledDisabledBands(out enabledBands, out disabledBands);
          //  selectedBands = enabledBands;
          //  UnselectedBands = disabledBands;

            enabledBandsReturn = enabledBands;
            disabledBandsReturn = disabledBands;

        }
    

    }
}
