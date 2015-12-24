using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.BusinessProcess;
namespace TOne.WhS.CodePreparation.BP.Activities
{

    public class ApplyNewCodesToDB : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<AddedCode>> NewCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<AddedCode> codesList = this.NewCodes.Get(context);

            long processInstanceID = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            INewSaleCodeDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleCodeDataManager>();
            dataManager.Insert(processInstanceID, codesList);
        }
    }
}
