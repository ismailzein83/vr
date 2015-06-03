using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.Data.SQL
{
    public class ReportDefinitionDataManager : BaseTOneDataManager, IReportDefinitionDataManager
    {

        public RDLCReportDefinition GetRDLCReportDefinition(int ReportDefinitionId)
        {
            return GetItemSP("[main].[sp_ReportDefinition_GetByReportDefinitionId]", ReportDefinitionMapper, ReportDefinitionId);
        }

        #region Private Methods

        RDLCReportDefinition ReportDefinitionMapper(IDataReader reader)
        {
            RDLCReportDefinition reportDefinition = Vanrise.Common.Serializer.Deserialize<RDLCReportDefinition>(reader["Content"] as string);
            reportDefinition.ReportDefinitionId = (int)reader["ReportDefinitionId"];
            reportDefinition.Name = reader["ReportName"] as string;
            return reportDefinition;
        }

        #endregion
    }
}
