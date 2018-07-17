using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data
{
    public interface IVRReportGenerationDataManager : IDataManager
    {
        List<VRReportGeneration> GetVRReportGenerations();

        bool AreVRReportGenerationUpdated(ref object updateHandle);

        bool Insert(VRReportGeneration vrReportGeneration);

        bool Update(VRReportGeneration vrReportGeneration);
    }
}
