using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.Data
{
    public interface IApplicationParameterDataManager : IDataManager
    {
        ApplicationParameter GetApplicationParameterById(int parameterId);
        bool UpdateApplicationParameter(ApplicationParameter appParamObj);
    }
}
