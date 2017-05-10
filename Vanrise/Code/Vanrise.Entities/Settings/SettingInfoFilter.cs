using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
   public class SettingInfoFilter
    {
        public List<ISettingFilter> Filters { get; set; }

        public List<Guid> ExcludedIds { get; set; }
    }
   public interface ISettingFilter
   {
       bool IsMatched(ISettingFilterContext context);
   }

   public interface ISettingFilterContext
   {
       Setting setting { get; }
   }

   public class SettingFilterContext : ISettingFilterContext
   {
       public Setting setting { get; set; }
   }
}
