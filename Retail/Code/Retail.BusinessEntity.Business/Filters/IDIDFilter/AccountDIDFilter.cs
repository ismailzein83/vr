using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountDIDFilter : BERelationChildBaseFilter, IDIDFilter
    {

        public bool IsMatched(IDIDFilterContext context)
        {
            return base.IsChildExcluded(context.DID.DIDId);
        }
    }
}
