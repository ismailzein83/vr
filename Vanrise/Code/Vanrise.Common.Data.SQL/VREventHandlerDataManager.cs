using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VREventHandlerDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IVREventHandlerDataManager
    {
        #region Constructors / Fields
        public VREventHandlerDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<VREventHandler> GetVREventHandlers()
        {
            return GetItemsSP("common.sp_VREventHandler_GetAll", VREventHandlerMapper);
        }

        public bool AreVREventHandlerUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.VREventHandler", ref updateHandle);
        }

        public bool Insert(VREventHandler vREventHandler)
        {
            string serializedSettings = vREventHandler.Settings != null ? Vanrise.Common.Serializer.Serialize(vREventHandler.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_VREventHandler_Insert", vREventHandler.VREventHandlerId, vREventHandler.Name, serializedSettings, vREventHandler.BED, vREventHandler.EED);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(VREventHandler vREventHandler)
        {
            string serializedSettings = vREventHandler.Settings != null ? Vanrise.Common.Serializer.Serialize(vREventHandler.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_VREventHandler_Update", vREventHandler.VREventHandlerId, vREventHandler.Name, serializedSettings, vREventHandler.BED, vREventHandler.EED);
            return (affectedRecords > 0);
        }

        #endregion

        #region Mappers

        VREventHandler VREventHandlerMapper(IDataReader reader)
        {
            VREventHandler vREventHandler = new VREventHandler
            {
                VREventHandlerId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VREventHandlerSettings>(reader["Settings"] as string),
                BED= GetReaderValue<DateTime>(reader,"BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
            };
            return vREventHandler;
        }

        #endregion
    }
}
