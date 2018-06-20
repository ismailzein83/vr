using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class RDLCReportDefinition
    {
        public int ReportDefinitionId { get; set; }

        public string Name { get; set; }

        public string ReportURL { get; set; }

        public List<ReportDefinitionRDLCFile> ReportDefinitionRDLCFiles { get; set; }
        public string Description { get; set; }
        public ReportParameterSettings ParameterSettings { get; set; }
        public string ReportGeneratorFQTN { get; set; }

        IReportGenerator _reportGenerator;
        public IReportGenerator GetReportGenerator()
        {
            if (_reportGenerator == null)
            {
                if (String.IsNullOrEmpty(this.ReportGeneratorFQTN))
                    throw new ArgumentNullException("ReportGeneratorFQTN");

                Type t = Type.GetType(this.ReportGeneratorFQTN);
                if (t == null)
                    throw new Exception(String.Format("ReportGeneratorFQTN '{0}' is not a valid Type", this.ReportGeneratorFQTN));
                _reportGenerator = Activator.CreateInstance(t) as IReportGenerator;
                if (_reportGenerator == null)
                    throw new Exception(String.Format("ReportGeneratorFQTN '{0}' is not of type IReportGenerator", this.ReportGeneratorFQTN));
            }
            return _reportGenerator;
        }
    }

    public class ReportDefinitionRDLCFile
    {
        public int ReportDefinitionRDLCFileId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string RDLCURL { get; set; }
    }
}