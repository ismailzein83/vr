using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Reprocess.Entities;
using Vanrise.Reprocess.Data;

namespace Vanrise.Reprocess.Data.SQL
{
    public class ReprocessDefinitionDataManager : BaseSQLDataManager, IReprocessDefinitionDataManager
    {

        #region ctor/Local Variables
        public ReprocessDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        #endregion


        #region Public Methods

        public List<ReprocessDefinition> GetReprocessDefinition()
        {
            return GetItemsSP("common.sp_ReprocessDefinition_GetAll", ReprocessDefinitionMapper);
        }

        public bool AreReprocessDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.ReprocessDefinition", ref updateHandle);
        }

        public bool Insert(ReprocessDefinition ReprocessDefinitionItem)
        {
            //string serializedSettings = ReprocessDefinitionItem.ReprocessDefinitionSettings != null ? Vanrise.Common.Serializer.Serialize(ReprocessDefinitionItem.ReprocessDefinitionSettings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_ReprocessDefinition_Insert", ReprocessDefinitionItem.ReprocessDefinitionId, ReprocessDefinitionItem.Name, null);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(ReprocessDefinition ReprocessDefinitionItem)
        {
            //string serializedSettings = ReprocessDefinitionItem.ReprocessDefinitionSettings != null ? Vanrise.Common.Serializer.Serialize(ReprocessDefinitionItem.ReprocessDefinitionSettings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_ReprocessDefinition_Update", ReprocessDefinitionItem.ReprocessDefinitionId, ReprocessDefinitionItem.Name, null);
            return (affectedRecords > 0);
        }

        #endregion


        #region Mappers

        ReprocessDefinition ReprocessDefinitionMapper(IDataReader reader)
        {
            ReprocessDefinition ReprocessDefinition = new ReprocessDefinition
            {
                ReprocessDefinitionId = (int)reader["ID"],
                Name = reader["Name"] as string,
                //ReprocessDefinitionSettings = Vanrise.Common.Serializer.Deserialize<ReprocessDefinitionSettings>(reader["Settings"] as string)
            };
            return ReprocessDefinition;
        }

        #endregion
    }

}
