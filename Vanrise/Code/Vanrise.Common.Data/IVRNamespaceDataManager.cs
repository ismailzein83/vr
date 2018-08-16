using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRNamespaceDataManager : IDataManager
    {
        List<VRNamespace> GetVRNamespaces();

        bool AreVRNamespaceUpdated(ref object updateHandle);

        bool Insert(VRNamespace vrNamespaceItem);

        bool Update(VRNamespace vrNamespaceItem);
    }
}
