using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public abstract class BaseReceiveAdapter
    {
        public abstract void ImportData(Action<object> receiveData);
    }
}
