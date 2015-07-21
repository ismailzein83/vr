using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.Data.SQL
{
    public class ApplicationParameterDataManager : BaseTOneDataManager, IApplicationParameterDataManager
    {
        public ApplicationParameter GetApplicationParameterById(int parameterId)
        {
            return GetItemSP("dbo.sp_ApplicationParameter_GetById", ApplicationParameterMapper, parameterId);
        }

        public bool UpdateApplicationParameter(ApplicationParameter appParamObj)
        {
            int recordesEffected = ExecuteNonQuerySP("dbo.sp_ApplicationParameter_Update", appParamObj.Id, appParamObj.Value);
            return (recordesEffected > 0);
        }

        ApplicationParameter ApplicationParameterMapper(IDataReader reader)
        {
            ApplicationParameter appParameter = new ApplicationParameter();
            appParameter.Value = (int)reader["Value"];
            return appParameter;
        }
    }
}
