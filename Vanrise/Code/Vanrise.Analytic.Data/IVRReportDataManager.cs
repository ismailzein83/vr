using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data
{
    public interface IVRReportDataManager : IDataManager
    {
        List<VRReport> GetVRReportDefinitions();

        bool AreVRReportUpdated(ref object updateHandle);

        bool Insert(VRReport dataAnalysisDefinitionItem);

        bool Update(VRReport dataAnalysisDefinitionItem);
    }
}
