using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRNamespaceDataManager : BaseSQLDataManager, IVRNamespaceDataManager
    {
        #region ctor/Local Variables
        public VRNamespaceDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<VRNamespace> GetVRNamespaces()
        {
            return GetItemsSP("common.sp_VRNamespace_GetAll", VRNamespaceMapper);
        }

        public bool AreVRNamespaceUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.VRNamespace", ref updateHandle);
        }

        public bool Insert(VRNamespace vrNamespace)
        {
            return ExecuteNonQuerySP("common.sp_VRNamespace_Insert", vrNamespace.VRNamespaceId, vrNamespace.Name) > 0;
        }

        public bool Update(VRNamespace vrNamespace)
        {
            return ExecuteNonQuerySP("common.sp_VRNamespace_Update", vrNamespace.VRNamespaceId, vrNamespace.Name) > 0;
        }

        #endregion

        #region Mappers

        VRNamespace VRNamespaceMapper(IDataReader reader)
        {
            VRNamespace vrNamespace = new VRNamespace
            {
                VRNamespaceId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
            };
            return vrNamespace;
        }

        #endregion
    }
}