using CDRComparison.Data;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.BP.Activities
{
    public class DeleteCDRTables : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<string> TableKey { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            ICDRDataManager cdrDataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRDataManager>();
            cdrDataManager.TableNameKey = this.TableKey.Get(context);
            cdrDataManager.DeleteCDRTable();
            IMissingCDRDataManager missingCDRDataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            missingCDRDataManager.TableNameKey = this.TableKey.Get(context);
            missingCDRDataManager.DeleteMissingCDRTable();
            IDisputeCDRDataManager disputeCDRDataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            disputeCDRDataManager.TableNameKey = this.TableKey.Get(context);
            disputeCDRDataManager.DeleteDisputeCDRTable();
            IPartialMatchCDRDataManager partialMatchCDRDataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            partialMatchCDRDataManager.TableNameKey = this.TableKey.Get(context);
            partialMatchCDRDataManager.DeletePartialMatchTable();


        }
    }
}
