using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Voucher.Entities;
using Vanrise.Common;
namespace Vanrise.Voucher.Business
{
    public class ConfigManager
    {   
        #region  Public Methods
        public int GetSerialNumberPartInitialSequence()
        {
            return GetVoucherCardSettings().SerialNumberPartInitialSequence;
        }
        public string GetSerialNumberPattern()
        {
            return GetVoucherCardSettings().SerialNumberPattern;
        }
        public VoucherCardSettings GetVoucherCardSettings()
        {
            SettingManager settingManager = new SettingManager();
            VoucherCardSettings voucherCardSettings = settingManager.GetSetting<VoucherCardSettings>(VoucherCardSettings.SETTING_TYPE);
            voucherCardSettings.ThrowIfNull("voucherCardSettings");
            return voucherCardSettings;
        }
        #endregion
    }
}
