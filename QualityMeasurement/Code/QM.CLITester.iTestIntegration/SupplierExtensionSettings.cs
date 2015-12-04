using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.BusinessEntity.Entities;
using QM.BusinessEntity.Data;
//using Vanrise.Entities;

namespace QM.CLITester.iTestIntegration
{
    public class SupplierExtensionSettings : BusinessEntity.Entities.ExtendedSupplierSetting
    {
        public string Prefix { get; set; }

        public bool IsNew { get; set; }

        public override void Apply(Supplier supplier)
        {
            //throw new NotImplementedException();
        }

        //public InsertOperationOutput<Supplier> AddNewTestCall(Supplier supplier)
        //{
        //    InsertOperationOutput<Supplier> insertOperationOutput = new InsertOperationOutput<TestCallResult>();

        //    insertOperationOutput.Result = InsertOperationResult.Failed;
        //    insertOperationOutput.InsertedObject = null;

        //    int carrierAccountId = -1;

        //    ISupplierDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ISupplierDataManager>();
        //    bool insertActionSucc = dataManager.Insert(supplier, out carrierAccountId);
        //    if (insertActionSucc)
        //    {
        //        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
        //        insertOperationOutput.InsertedObject = supplier;
        //    }

        //    return insertOperationOutput;
        //}

        //public Vanrise.Entities.UpdateOperationOutput<TestCallResult> UpdateTestCallResult(TestCallResult testCallResult)
        //{
        //    ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();

        //    bool updateActionSucc = dataManager.Update(testCallResult);
        //    Vanrise.Entities.UpdateOperationOutput<TestCallResult> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<TestCallResult>();

        //    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
        //    updateOperationOutput.UpdatedObject = null;

        //    if (updateActionSucc)
        //    {
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
        //        updateOperationOutput.UpdatedObject = testCallResult;
        //    }
        //    else
        //    {
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
        //    }

        //    return updateOperationOutput;
        //}


    }
}
