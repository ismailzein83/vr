using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IExtensibleBEItemDataManager : IDataManager
    {
        bool UpdateExtensibleBEItem(ExtensibleBEItem extensibleBEItem);

        bool AddExtensibleBEItem(ExtensibleBEItem extensibleBEItem);
        List<ExtensibleBEItem> GetExtensibleBEItems();
        bool AreExtensibleBEItemUpdated(ref object updateHandle);
    }
}
