using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Business
{
    public class BaseTaskActionArgumentOnAfterSaveActionContext : IBaseTaskActionArgumentOnAfterSaveActionContext
    {
        public Guid TaskId { get; set; }
    }
}
