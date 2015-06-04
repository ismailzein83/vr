using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data;
using TOne.Entities;


namespace TOne.Business
{
    public class ReportDefinitionManager
    {
        private readonly IReportDefinitionDataManager _datamanager;

        public ReportDefinitionManager()
        {
            _datamanager = DataManagerFactory.GetDataManager<IReportDefinitionDataManager>();
               
        }
        public RDLCReportDefinition GetRDLCReportDefinition(int ReportDefinitionId)
        {
            return _datamanager.GetRDLCReportDefinition(ReportDefinitionId);
        }
        public List<RDLCReportDefinition> GetAllRDLCReportDefinition()
        {
            return _datamanager.GetAllRDLCReportDefinition();
        }

    }
}
