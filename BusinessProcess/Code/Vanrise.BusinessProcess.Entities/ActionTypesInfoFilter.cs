using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
   public class ActionTypesInfoFilter
    {
       public IEnumerable<IActionTypesFilter> Filters { get; set; }
    }
   public interface IActionTypesFilter
   {
       bool IsExcluded(IActionTypeFilterContext context);
   }
   public interface IActionTypeFilterContext
   {
       Guid ActionTypeId { get; }

       object CustomData { get; set; }
   }
   public class ActionTypeFilterContext : IActionTypeFilterContext
   {
       public Guid ActionTypeId { get; set; }

       public object CustomData { get; set; }
   }
}
