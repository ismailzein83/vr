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
        #region Constructor
        public static Guid _definitionId = new Guid("6761d9be-baff-4d80-a903-16947b705395");
        GenericBusinessEntityDefinitionManager _genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
        #endregion
        #region Public Methods
        public List<VoucharCardSerialNumberPart> GetVoucherCardDefinition()
        {
            var genericBEDefinitionSettings = _genericBusinessEntityDefinitionManager.GetGenericBEDefinitionSettings(_definitionId);
            genericBEDefinitionSettings.ThrowIfNull("genericBEDefinitionSettings", _definitionId);
            genericBEDefinitionSettings.ExtendedSettings.ThrowIfNull("genericBEDefinitionSettings.ExtendedSettings", _definitionId);
            return genericBEDefinitionSettings.ExtendedSettings.CastWithValidate<VoucharCardsExtendedSettings>("VoucharCardsExtendedSettings").SerialNumberParts;
        }
        #endregion
    }
}
