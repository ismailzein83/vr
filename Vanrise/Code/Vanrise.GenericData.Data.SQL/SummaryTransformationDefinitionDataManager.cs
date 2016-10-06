using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class SummaryTransformationDefinitionDataManager : BaseSQLDataManager, ISummaryTransformationDefinitionDataManager
    {
        public SummaryTransformationDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods
        public List<SummaryTransformationDefinition> GetSummaryTransformationDefinitions()
        {
            return GetItemsSP("genericdata.sp_SummaryTransformationDefinition_GetAll", SummaryTransformationDefinitionMapper);
        }

        public bool AreSummaryTransformationDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.SummaryTransformationDefinition", ref updateHandle);
        }

        public bool UpdateSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            string serializedObj = null;
            serializedObj = Vanrise.Common.Serializer.Serialize(summaryTransformationDefinition);
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_SummaryTransformationDefinition_Update", summaryTransformationDefinition.SummaryTransformationDefinitionId, summaryTransformationDefinition.Name, serializedObj);
            return (recordesEffected > 0);
        }

        public bool AddSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            string serializedObj = null;
            serializedObj = Vanrise.Common.Serializer.Serialize(summaryTransformationDefinition);
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_SummaryTransformationDefinition_Insert", summaryTransformationDefinition.SummaryTransformationDefinitionId, summaryTransformationDefinition.Name, serializedObj);

            return (recordesEffected > 0);
        }

        #endregion

        #region Mappers
        private SummaryTransformationDefinition SummaryTransformationDefinitionMapper(IDataReader reader)
        {

            SummaryTransformationDefinition summaryTransformationDefinition = new SummaryTransformationDefinition();
            string details = reader["Details"] as string;
            if (details != null && details != string.Empty)
                summaryTransformationDefinition = Vanrise.Common.Serializer.Deserialize<SummaryTransformationDefinition>(details);

            summaryTransformationDefinition.SummaryTransformationDefinitionId = GetReaderValue<Guid>(reader,"ID");

            return summaryTransformationDefinition;
        }
        #endregion
    }
}
