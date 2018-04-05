using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface IRuntimeNodeDataManager : IDataManager
    {
        List<RuntimeNode> GetAllNodes();
    }
}
