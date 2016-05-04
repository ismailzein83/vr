﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class CloudAuthServerDataManager : BaseSQLDataManager, ICloudAuthServerDataManager
    {
        public CloudAuthServerDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        public Entities.CloudAuthServer GetAuthServer()
        {
            return GetItemSP("sec.sp_CloudAuthServer_GetFirst", CloudAuthServerMapper);
        }

        public bool IsAuthServerUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.[CloudAuthServer]", ref updateHandle);
        }

        #region Mappers

        CloudAuthServer CloudAuthServerMapper(IDataReader reader)
        {
            return new CloudAuthServer
            {
                Settings = Serializer.Deserialize<CloudAuthServerSettings>(reader["Settings"] as string),
                ApplicationIdentification = Serializer.Deserialize<CloudApplicationIdentification>(reader["ApplicationIdentification"] as string)
            };
        }

        #endregion
    }
}
