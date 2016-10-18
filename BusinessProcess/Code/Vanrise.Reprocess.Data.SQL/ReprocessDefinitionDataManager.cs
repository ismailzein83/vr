using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Reprocess.Entities;
using Vanrise.Reprocess.Data;
using Vanrise.Common;

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
            return GetItemsSP("reprocess.sp_ReprocessDefinition_GetAll", ReprocessDefinitionMapper);
        }

        public bool AreReprocessDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("reprocess.ReprocessDefinition", ref updateHandle);
        }

        public bool Insert(ReprocessDefinition reprocessDefinitionItem)
        {
            string serializedSettings = reprocessDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(reprocessDefinitionItem.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("reprocess.sp_ReprocessDefinition_Insert", reprocessDefinitionItem.ReprocessDefinitionId, reprocessDefinitionItem.Name, serializedSettings);

            return (affectedRecords > 0);
        }

        public bool Update(ReprocessDefinition ReprocessDefinitionItem)
        {
            string serializedSettings = ReprocessDefinitionItem.Settings != null ? Vanrise.Common.Serializer.Serialize(ReprocessDefinitionItem.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("reprocess.sp_ReprocessDefinition_Update", ReprocessDefinitionItem.ReprocessDefinitionId, ReprocessDefinitionItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion


        #region Mappers

        ReprocessDefinition ReprocessDefinitionMapper(IDataReader reader)
        {
            ReprocessDefinition ReprocessDefinition = new ReprocessDefinition
            {
                ReprocessDefinitionId =GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<ReprocessDefinitionSettings>(reader["Settings"] as string)
            };
            return ReprocessDefinition;
        }

        #endregion

    }

}
