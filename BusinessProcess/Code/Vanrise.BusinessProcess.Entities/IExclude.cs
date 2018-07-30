using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public interface IExclude
    {
        bool IsExcluded { get; set; }
        abstract void SetAsExcluded();
    }
}
