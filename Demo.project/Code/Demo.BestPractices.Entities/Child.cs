using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BestPractices.Entities
{
    public class Child
    {
        public long ChildId { get; set; }
        public string Name { get; set; }
        public long ParentId { get; set; }
        public ChildSettings Settings { get; set; }
    }

    public class ChildSettings
    {
        public ChildShape ChildShape { get; set; }
    }
    public abstract class ChildShape
    {
        public abstract Guid ConfigId { get; }
        public abstract string GetChildAreaDescription(IChildShapeDescriptionContext context);
    }
    public interface IChildShapeDescriptionContext
    {
        Child Child { get;}
    }

}
