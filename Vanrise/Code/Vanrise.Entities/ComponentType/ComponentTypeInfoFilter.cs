using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
   public class ComponentTypeInfoFilter
    {
        public List<IComponentTypeFilter> Filters { get; set; }

        public List<Guid> ExcludedIds { get; set; }
    }

   public interface IComponentTypeFilter
   {
       bool IsMatched(IComponentTypeFilterContext context);
   }

   public interface IComponentTypeFilterContext
   {
       VRComponentType componentType { get; }
   }

   public class ComponentTypeFilterContext : IComponentTypeFilterContext
   {
       public VRComponentType componentType { get; set; }
   }

}
