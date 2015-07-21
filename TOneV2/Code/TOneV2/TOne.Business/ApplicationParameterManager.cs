using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TABS;
using TOne.Data;
using TOne.Entities;

namespace TOne.Business
{
    public class ApplicationParameterManager
    {
        public ApplicationParameter GetApplicationParameterById(int parameterId)
        {
            IApplicationParameterDataManager dataManager = DataManagerFactory.GetDataManager<IApplicationParameterDataManager>();
            return dataManager.GetApplicationParameterById(parameterId);
        }

        public TOne.Entities.UpdateOperationOutput<ApplicationParameter> UpdateApplicationParameter(ApplicationParameter appParamObj)
        {
            IApplicationParameterDataManager dataManager = DataManagerFactory.GetDataManager<IApplicationParameterDataManager>();
            bool updateActionSucc = dataManager.UpdateApplicationParameter(appParamObj);

            UpdateOperationOutput<ApplicationParameter> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<ApplicationParameter>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = appParamObj;
            }

            return updateOperationOutput;
        }
    }
}
