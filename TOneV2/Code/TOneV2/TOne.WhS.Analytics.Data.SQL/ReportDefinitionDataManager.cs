using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Data.SQL;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class ReportDefinitionDataManager : BaseTOneDataManager, IReportDefinitionDataManager
    {    
        #region ctor/Local Variables
        public ReportDefinitionDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }
        #endregion

        public RDLCReportDefinition GetRDLCReportDefinition(int ReportDefinitionId)
        {
            return GetItemSP("[TOneWhs_Billing].[sp_ReportDefinition_GetById]", ReportDefinitionMapper, ReportDefinitionId);
        }

        public List<RDLCReportDefinition> GetAllRDLCReportDefinition()
        {
            return GetItemsSP("[TOneWhs_Billing].[sp_ReportDefinition_GetAll]", ReportDefinitionMapper);
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
