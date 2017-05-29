using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRObjectTypeDefinitionDataManager : BaseSQLDataManager, IVRObjectTypeDefinitionDataManager
    {
        #region ctor/Local Variables
        public VRObjectTypeDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion


        #region Public Methods

        public List<VRObjectTypeDefinition> GetVRObjectTypeDefinitions()
        {
            return GetItemsSP("common.sp_VRObjectTypeDefinition_GetAll", VRObjectTypeDefinitionMapper);
        }

        public bool AreVRObjectTypeDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.VRObjectTypeDefinition", ref updateHandle);
        }

        public bool Insert(VRObjectTypeDefinition vrObjectTypeDefinitionItem)
        {
            string serializedSettings = vrObjectTypeDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrObjectTypeDefinitionItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_VRObjectTypeDefinition_Insert", vrObjectTypeDefinitionItem.VRObjectTypeDefinitionId, vrObjectTypeDefinitionItem.Name, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(VRObjectTypeDefinition vrObjectTypeDefinitionItem)
        {
            string serializedSettings = vrObjectTypeDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrObjectTypeDefinitionItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_VRObjectTypeDefinition_Update", vrObjectTypeDefinitionItem.VRObjectTypeDefinitionId, vrObjectTypeDefinitionItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }
        public void GenerateScript(List<VRObjectTypeDefinition> objTypeDefs, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var objTypeDef in objTypeDefs)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", objTypeDef.VRObjectTypeDefinitionId, objTypeDef.Name, Serializer.Serialize(objTypeDef.Settings));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[VRObjectTypeDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);", scriptBuilder);
            addEntityScript("[common].[VRObjectTypeDefinition]", script);
        }
        #endregion


        #region Mappers

        VRObjectTypeDefinition VRObjectTypeDefinitionMapper(IDataReader reader)
        {
            VRObjectTypeDefinition vrObjectTypeDefinition = new VRObjectTypeDefinition
            {
                VRObjectTypeDefinitionId = (Guid) reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VRObjectTypeDefinitionSettings>(reader["Settings"] as string) 
            };
            return vrObjectTypeDefinition;
        }

        #endregion
    }
}
