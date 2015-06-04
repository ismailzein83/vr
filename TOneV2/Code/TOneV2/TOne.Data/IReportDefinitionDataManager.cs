using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.Data
{
    public interface IReportDefinitionDataManager : IDataManager
    {
        RDLCReportDefinition GetRDLCReportDefinition(int ReportDefinitionId);

        List<RDLCReportDefinition> GetAllRDLCReportDefinition();
    }
}
