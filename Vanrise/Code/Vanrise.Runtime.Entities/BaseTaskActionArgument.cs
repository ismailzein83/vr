using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class BaseTaskActionArgument
    {

        public virtual void OnAfterSaveAction(IBaseTaskActionArgumentOnAfterSaveActionContext context)
        {

        }
        public Dictionary<string, object> RawExpressions { get; set; }
    }
    public interface IBaseTaskActionArgumentOnAfterSaveActionContext
    {
        Guid TaskId { get; }
    }
}
