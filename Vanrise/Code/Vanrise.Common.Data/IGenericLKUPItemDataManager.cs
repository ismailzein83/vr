using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IGenericLKUPItemDataManager : IDataManager
    {
        List<GenericLKUPItem> GetGenericLKUPItem();

        bool AreGenericLKUPItemUpdated(ref object updateHandle);

        bool Insert(GenericLKUPItem genericLKUPItem);

        bool Update(GenericLKUPItem genericLKUPItem);
    }
}
