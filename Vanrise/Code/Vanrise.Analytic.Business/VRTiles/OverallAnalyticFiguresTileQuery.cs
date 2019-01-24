using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Business
{
    public class OverallAnalyticFiguresTileQuery : FiguresTileQuerySettings
    {
        public override Guid ConfigId { get { return new Guid("1A67CEF6-7472-4151-8A74-8AF12D245E27"); } }

        public override List<FigureItemValue> Execute(IFiguresTileQueryExecuteContext context)
        {
            throw new NotImplementedException();
        }

        public override List<FigureItemSchema> GetSchema(IFiguresTileQueryGetSchemaContext context)
        {
            throw new NotImplementedException();
        }

        public Guid AnalyticTableId { get; set; }
        public List<Guid> Measures { get; set; }
        public VRTimePeriod TimePeriod { get; set; }
        public RecordFilter RecordFilter { get; set; }

    }
}
