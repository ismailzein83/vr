using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Data;
namespace TOne.LCRProcess.Activities
{

    public sealed class CreateDistinctCodesTable : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<Int32> MaximumCodeID { get; set; }
        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            int maxID = dataManager.CreateandInsertDistinctCodesTable();

            this.MaximumCodeID.Set(context, maxID);
        }
    }
}
