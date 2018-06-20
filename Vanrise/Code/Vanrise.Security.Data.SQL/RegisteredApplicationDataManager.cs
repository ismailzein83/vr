using System;
using System.Data;
using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class RegisteredApplicationDataManager : BaseSQLDataManager, IRegisteredApplicationDataManager
    {
        #region ctor
        public RegisteredApplicationDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<RegisteredApplication> GetRegisteredApplications()
        {
            return GetItemsSP("sec.sp_RegisteredApplication_GetAll", RegisteredApplicationrMapper);
        }

        public bool AddRegisteredApplication(RegisteredApplication registeredApplication)
        {
            return ExecuteNonQuerySP("sec.sp_RegisteredApplication_Insert", registeredApplication.ApplicationId, registeredApplication.Name, registeredApplication.URL) > 0;
        }


        public bool AreRegisteredApplicationsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.[RegisteredApplication]", ref updateHandle);
        }

        #endregion

        #region Mappers

        private RegisteredApplication RegisteredApplicationrMapper(IDataReader reader)
        {
            return new Entities.RegisteredApplication
            {
                ApplicationId = GetReaderValue<Guid>(reader, "Id"),
                Name = reader["Name"] as string,
                URL = reader["URL"] as string
            };
        }

        #endregion
    }
}