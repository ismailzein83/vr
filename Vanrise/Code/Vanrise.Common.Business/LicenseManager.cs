using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class LicenseManager
    {
        public string GetLicenseExpiryDate()
        {
            DateTime value = LicenceManagerControl.ExpiryDate;
            return value == DateTime.MinValue ? null : value.ToString("yyyy-MM-dd");
        }
    }
}
