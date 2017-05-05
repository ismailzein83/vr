using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class SettingOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("955CB0A1-A801-428F-8430-C046C0D3EAD3"); }
        }

        public Guid SettingId { get; set; }

        public string OverriddenName { get; set; }

        public string OverriddenCategory { get; set; }

        public object OverriddenData { get; set; }
    }
}
