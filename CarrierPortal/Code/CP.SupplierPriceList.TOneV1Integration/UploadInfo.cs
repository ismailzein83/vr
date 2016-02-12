using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPriceList.TOneV1Integration
{
    public class UploadInfo
    {
        public byte[] ContentBytes { get; set; }
        public string FileName { get; set; }
        public int Status { get; set; }
    }
}
