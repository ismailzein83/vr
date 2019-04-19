using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class TransactionTypeLKUPBEDefinitionSettings : LKUPBEDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("BA619742-DF5C-4448-A607-E38AD7B43D86"); } }

        public override Dictionary<string, LKUPBusinessEntityItem> GetAllLKUPBusinessEntityItems(ILKUPBusinessEntityExtendedSettingsContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }
    }
}
