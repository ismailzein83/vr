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
    public class ApplyChangedCodesToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ChangedCode>> ChangedCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ChangedCode> codesList = this.ChangedCodes.Get(context);

            long processInstanceID = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            IChangedSaleCodeDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleCodeDataManager>();
            dataManager.Insert(processInstanceID, codesList);
        }
    }
}
