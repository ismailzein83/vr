using Retail.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.Data.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public decimal GetSessionOctetsLimit()
        {
            ICXDataSettings icxDataSettings = this.GetICXDataSettings();
            return icxDataSettings.SessionOctetsLimit;
        }

        #endregion

        #region Private Methods

        private ICXDataSettings GetICXDataSettings()
        {
            ICXDataSettings icxDataSettings = new SettingManager().GetSetting<ICXDataSettings>(ICXDataSettings.SETTING_TYPE);
            if (icxDataSettings == null)
                throw new NullReferenceException("icxDataSettings");

            return icxDataSettings;
        }

        #endregion
    }
}