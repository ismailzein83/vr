using CDRComparison.Data;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.BP.Activities
{
    public class CreateCDRTables : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public OutArgument<string> TableKey { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            string tableKey = Guid.NewGuid().ToString().Replace("-", "");

            ICDRDataManager cdrDataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRDataManager>();
            cdrDataManager.TableNameKey = tableKey;

            IMissingCDRDataManager missingCDRDataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            missingCDRDataManager.TableNameKey = tableKey;

            IDisputeCDRDataManager disputeCDRDataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            disputeCDRDataManager.TableNameKey = tableKey;

            IPartialMatchCDRDataManager partialMatchCDRDataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            partialMatchCDRDataManager.TableNameKey = tableKey;

            IInvalidCDRDataManager invalidCDRDataManager = CDRComparisonDataManagerFactory.GetDataManager<IInvalidCDRDataManager>();
            invalidCDRDataManager.TableNameKey = tableKey;

            cdrDataManager.CreateCDRTempTable();
            missingCDRDataManager.CreateMissingCDRTempTable();
            disputeCDRDataManager.CreateDisputeCDRTempTable();
            partialMatchCDRDataManager.CreatePartialMatchCDRTempTable();
            invalidCDRDataManager.CreateInvalidCDRTempTable();

            this.TableKey.Set(context, tableKey);
        }
    }
}
