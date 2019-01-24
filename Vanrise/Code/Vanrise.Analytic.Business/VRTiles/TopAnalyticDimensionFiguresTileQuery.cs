using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Business
{
    public class TopAnalyticDimensionFiguresTileQuery : FiguresTileQuerySettings
    {
        public override Guid ConfigId { get { return new Guid("A0833F27-6718-40E1-BB69-344B0BD3CFAF"); } }

        public override List<FigureItemValue> Execute(IFiguresTileQueryExecuteContext context)
        {
            throw new NotImplementedException();
        }

        public override List<FigureItemSchema> GetSchema(IFiguresTileQueryGetSchemaContext context)
        {
            throw new NotImplementedException();
        }
        public Guid AnalyticTableId { get; set; }
        public Guid DimensionId { get; set; }
        public List<Guid> Measures { get; set; }
        public VRTimePeriod TimePeriod { get; set; }
        public RecordFilter RecordFilter { get; set; }
        public OrderType OrderType { get; set; }

    }
    public enum OrderType  { ASC = 1, DESC = 2 }
   
}
