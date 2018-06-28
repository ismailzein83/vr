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
        GenericBusinessEntityDefinitionManager _GenericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
        Guid _definitionId = new Guid("6761d9be-baff-4d80-a903-16947b705395"); // could be removed when VoucherCardsManager._definitionId becomes public
        public List<VoucharCardSerialNumberPart> GetVoucherCardDefinition()
        {
            GenericBEDefinitionSettings entity = new GenericBEDefinitionSettings();
            entity = _GenericBusinessEntityDefinitionManager.GetGenericBEDefinitionSettings(_definitionId);
            return entity.ExtendedSettings.CastWithValidate<VoucharCardsExtendedSettings>("VoucharCardsExtendedSettings").SerialNumberParts;
        }

      
    }
}
