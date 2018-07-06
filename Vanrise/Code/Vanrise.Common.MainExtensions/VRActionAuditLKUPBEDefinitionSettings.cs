using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class VRActionAuditLKUPBEDefinitionSettings : LKUPBEDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("442BCFAD-1407-4158-82E7-E1B7A0AB458B"); }
        }
        public  VRActionAuditLKUPType Type { get;set }
        public override Dictionary<string, LKUPBusinessEntityItem> GetAllLKUPBusinessEntityItems(ILKUPBusinessEntityExtendedSettingsContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(object parameter, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }
    }
}
