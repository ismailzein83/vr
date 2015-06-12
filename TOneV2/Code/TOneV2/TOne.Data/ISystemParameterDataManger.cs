using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.Data
{
    public interface ISystemParameterDataManger : IDataManager
    {
        void UpdateSystemParameter(SystemParameter systemParameter, string value);

        T GetSystemParameterValue<T>(SystemParameter systemParameter);
    }
}
