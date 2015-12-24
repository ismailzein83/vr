using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPValidationMessageDataManager : IDataManager
    {
        void Insert(IEnumerable<ValidationMessage> messages);
    }
}
