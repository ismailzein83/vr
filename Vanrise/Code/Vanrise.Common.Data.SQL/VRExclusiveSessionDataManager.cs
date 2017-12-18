﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRExclusiveSessionDataManager : BaseSQLDataManager, IVRExclusiveSessionDataManager
    {
        public VRExclusiveSessionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        public void InsertIfNotExists(Guid sessionTypeId, string targetId)
        {
            ExecuteNonQuerySP("[common].[sp_VRExclusiveSession_InsertIfNotExists]", sessionTypeId, targetId);
        }

        public void TryTakeSession(Guid sessionTypeId, string targetId, int userId, int timeoutInSeconds, out int takenByUserId)
        {
            var takenByUserIdObject = ExecuteScalarSP("[common].[sp_VRExclusiveSession_TryTake]", sessionTypeId, targetId, userId, timeoutInSeconds);
            takenByUserIdObject.ThrowIfNull("takenByUserIdObject");
            takenByUserId = (int)takenByUserIdObject;
        }

        public void ReleaseSession(Guid sessionTypeId, string targetId, int userId)
        {
            ExecuteNonQuerySP("[common].[sp_VRExclusiveSession_Release]", sessionTypeId, targetId, userId);
        }


        public List<VRExclusiveSession> GetAllVRExclusiveSessions(int timeOutInSeconds)
        {
            return GetItemsSP("[common].[sp_VRExclusiveSession_GetAll]", VRExclusiveSessionMapper);
        }

        VRExclusiveSession VRExclusiveSessionMapper(IDataReader reader)
        {
            VRExclusiveSession vrExclusiveSession = new VRExclusiveSession
            {
                VRExclusiveSessionID = (int)reader["ID"],
                SessionTypeId = (Guid)reader["ID"],
                TargetId =  reader["TargetId"] as string,
                TakenByUserId = GetReaderValue<int>(reader, "TakenByUserId"),
                LastTakenUpdateTime = GetReaderValue<DateTime>(reader, "LastTakenUpdateTime"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
            };

            return vrExclusiveSession;
        }
    }
}
