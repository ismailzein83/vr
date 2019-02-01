using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class FiguresTileQuery
    {
        public Guid FiguresTileQueryId { get; set; }
        public string Name { get; set; }
        public FiguresTileQuerySettings Settings { get; set; }
    }
    public abstract class FiguresTileQuerySettings
    {
        public abstract Guid ConfigId { get; }
        public abstract List<FigureItemSchema> GetSchema(IFiguresTileQueryGetSchemaContext context);
        public abstract List<FigureItemValue> Execute(IFiguresTileQueryExecuteContext context);
    }
    public interface IFiguresTileQueryGetSchemaContext
    {
       
    }
    public class FiguresTileQueryGetSchemaContext : IFiguresTileQueryGetSchemaContext
    {

    }
    public interface IFiguresTileQueryExecuteContext
    {
        List<string> ItemsToDisplayNames { get; set; }
    }
    public class FiguresTileQueryExecuteContext: IFiguresTileQueryExecuteContext
    {
        public List<string> ItemsToDisplayNames { get; set; }
    }
    public class FigureItemValue
    {
        public string Name { get; set; }
        public Object Value { get; set; }
    }
    public class FigureItemSchema
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
