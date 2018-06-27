using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Voucher.Entities
{
    public class VoucherCardSettings : SettingData
    {
        public static string SETTING_TYPE = "VR_Voucher_VoucherCardSettings";
        public int SerialNumberPartInitialSequence { get; set; }
        public string SerialNumberPattern { get; set; }
    }
}
