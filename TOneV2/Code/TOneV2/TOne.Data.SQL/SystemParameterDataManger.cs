using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;
using Vanrise.Data.SQL;

namespace TOne.Data.SQL
{
    public class SystemParameterDataManger : BaseTOneDataManager, ISystemParameterDataManger
    {

        public void UpdateSystemParameter(Entities.SystemParameter systemParameter, string value)
        {
            ExecuteNonQuerySP("MainModule.sp_SystemParameter_Update", value);
        }

        public T GetSystemParameterValue<T>(Entities.SystemParameter systemParameter)
        {
            return (T)Convert.ChangeType(ExecuteScalarSP("MainModule.sp_SystemParameter_GetSystemparameterValue", Enum.GetName(typeof(SystemParameter), systemParameter)), typeof(T));
        }
    }
}
