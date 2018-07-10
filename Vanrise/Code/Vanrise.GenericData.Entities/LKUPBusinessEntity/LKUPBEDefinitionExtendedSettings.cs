using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class LKUPBEDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract Dictionary<string, LKUPBusinessEntityItem> GetAllLKUPBusinessEntityItems(ILKUPBusinessEntityExtendedSettingsContext context);
        public abstract bool IsCacheExpired(ref DateTime? lastCheckTime);
    }

    public interface ILKUPBusinessEntityExtendedSettingsContext
    {
        BusinessEntityDefinitionSettings BEDefinitionSettings { get; }
    }
    public class LKUPBusinessEntityItem
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public string LKUPBusinessEntityItemId { get; set; }
        public string Name { get; set; }
    }
}
