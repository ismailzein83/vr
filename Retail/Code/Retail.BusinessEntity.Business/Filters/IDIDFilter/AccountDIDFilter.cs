using Retail.BusinessEntity.Entities;
using Vanrise.GenericData.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountDIDFilter : BERelationChildBaseFilter, IDIDFilter
    {
        public bool IsMatched(IDIDFilterContext context)
        {
            return !base.IsChildExcluded(context.DID.DIDId);
        }
    }
}
