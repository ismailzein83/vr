using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class ServiceTypeDataManager : BaseSQLDataManager, IServiceTypeDataManager
    {
           
        #region Constructors

        public ServiceTypeDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<ServiceType> GetServiceTypes()
        {
            return GetItemsSP("Retail.sp_ServiceType_GetAll", ServiceTypeMapper);
        }

        public bool Update(Guid serviceTypeId, string title,Guid accountBEDefinitionId, ServiceTypeSettings serviceTypeSettings)
        {
            string serializedSettings = serviceTypeSettings != null ? Vanrise.Common.Serializer.Serialize(serviceTypeSettings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_ServiceType_Update", serviceTypeId, title,accountBEDefinitionId, serializedSettings);
            return (affectedRecords > 0);
        }

        public bool AreServiceTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail.ServiceType", ref updateHandle);
        }

        public void GenerateScript(List<ServiceType> serviceTypes, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var serviceType in serviceTypes)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}')", serviceType.ServiceTypeId, serviceType.Name, serviceType.Title, serviceType.AccountBEDefinitionId, Serializer.Serialize(serviceType.Settings));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[AccountBEDefinitionId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[AccountBEDefinitionId],[Settings]))
merge	[Retail].[ServiceType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[AccountBEDefinitionId] = s.[AccountBEDefinitionId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[AccountBEDefinitionId],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[AccountBEDefinitionId],s.[Settings]);", scriptBuilder);
            addEntityScript("[Retail_BE].[ServiceType]", script);
        }

        #endregion

        #region Mappers

        private ServiceType ServiceTypeMapper(IDataReader reader)
        {
            return new ServiceType()
            {
                ServiceTypeId = GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                AccountBEDefinitionId = GetReaderValue<Guid>(reader, "AccountBEDefinitionId"),
                Settings = Vanrise.Common.Serializer.Deserialize<ServiceTypeSettings>(reader["Settings"] as string),
            };
        }

        #endregion
    }
}
