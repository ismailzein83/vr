using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace TOne.WhS.Deal.Business
{
    public class DealBusinessObjectDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7b5d42b4-b05c-4f54-9683-a73dd0cebd68"); }
        }

        public override bool DoesSupportFilterOnAllFields
        {
            get { return false; }
        }

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
