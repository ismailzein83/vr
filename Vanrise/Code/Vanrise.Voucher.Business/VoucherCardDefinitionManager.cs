using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Voucher.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
namespace Vanrise.Voucher.Business
{
    public class VoucherCardDefinitionManager
    {
        GenericBusinessEntityDefinitionManager _genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
        Guid _definitionId = VoucherCardsManager._definitionId;
        public List<VoucharCardSerialNumberPart> GetVoucherCardDefinition()
        {
            var genericBEDefinitionSettings = _genericBusinessEntityDefinitionManager.GetGenericBEDefinitionSettings(_definitionId);
            genericBEDefinitionSettings.ThrowIfNull("genericBEDefinitionSettings", _definitionId);
            genericBEDefinitionSettings.ExtendedSettings.ThrowIfNull("genericBEDefinitionSettings.ExtendedSettings", _definitionId);
            return genericBEDefinitionSettings.ExtendedSettings.CastWithValidate<VoucharCardsExtendedSettings>("VoucharCardsExtendedSettings").SerialNumberParts;
        }
      
    }
}
