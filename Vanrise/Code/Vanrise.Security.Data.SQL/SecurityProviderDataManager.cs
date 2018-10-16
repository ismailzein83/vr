using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class SecurityProviderDataManager : BaseSQLDataManager, ISecurityProviderDataManager
    {
        public SecurityProviderDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }
        #region Public Methods
        public bool SetDefaultSecurityProvider(Guid securityProviderId)
        {
            int recordsEffected = ExecuteNonQuerySP("sec.sp_SecurityProvider_SetDefault", securityProviderId);
            return (recordsEffected > 0);
        }

        public SecurityProvider GetDefaultSecurityProvider()
        {
            return GetItemSP("sec.sp_SecurityProvider_GetDefault", SecurityProviderMapper);
        }
        #endregion
        #region Mappers
        public SecurityProvider SecurityProviderMapper(IDataReader reader)
        {
            return new SecurityProvider()
            {
                IsEnabled = GetReaderValue<bool>(reader, "IsEnabled"),
                Name = reader["Name"] as string,
                SecurityProviderId = GetReaderValue<Guid>(reader,"ID"),
                Settings = Vanrise.Common.Serializer.Deserialize<SecurityProviderSettings>(reader["Settings"] as string)
            };
        }
        #endregion
    }
}
