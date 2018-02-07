using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Common;
namespace Vanrise.GenericData.MainExtensions
{
    public class GenericBESerialNumberManager
    {
        public IEnumerable<GenericBESerialNumberPartInfo> GetSerialNumberPartDefinitionsInfo(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSetting = new GenericBusinessEntityDefinitionManager().GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            genericBEDefinitionSetting.ThrowIfNull("genericBEDefinitionSetting", businessEntityDefinitionId);
            genericBEDefinitionSetting.OnBeforeInsertHandler.ThrowIfNull("genericBEDefinitionSetting.OnBeforeInsertHandler");

            return genericBEDefinitionSetting.OnBeforeInsertHandler.TryGetInfoByType(new GenericBEOnBeforeInsertHandlerInfoByTypeContext
            {
                InfoType = "SerialNumberPartDefinitions",
                DefinitionSettings = genericBEDefinitionSetting
            }) as IEnumerable<GenericBESerialNumberPartInfo>;
        }
        public long GetSerialNumberPartInitialSequence(Guid businessEntityDefinitionId, string infoType)
        {
            var genericBEDefinitionSetting = new GenericBusinessEntityDefinitionManager().GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            genericBEDefinitionSetting.ThrowIfNull("genericBEDefinitionSetting", businessEntityDefinitionId);
            genericBEDefinitionSetting.OnBeforeInsertHandler.ThrowIfNull("genericBEDefinitionSetting.OnBeforeInsertHandler");

            return (long) genericBEDefinitionSetting.OnBeforeInsertHandler.TryGetInfoByType(new GenericBEOnBeforeInsertHandlerInfoByTypeContext
            {
                InfoType = infoType,
                DefinitionSettings = genericBEDefinitionSetting
            });
        }
    }
}
