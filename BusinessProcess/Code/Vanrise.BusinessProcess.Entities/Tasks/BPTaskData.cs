using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BPTaskData
    {
        public virtual string TaskType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public virtual Guid? TaskTypeId { get; set; }
    }
}
