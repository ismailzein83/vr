using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace CDRComparison.Business
{
    public class FileCDRSourceManager
    {
        public decimal? GetMaxUncompressedFileSizeInMegaBytes()
        {
            string size = System.Configuration.ConfigurationManager.AppSettings["VRFile_Max_Uncompressed_MB_Size"];
            if (size != null)
                return Convert.ToDecimal(size);
            else
                return null;
        }
    }
}
