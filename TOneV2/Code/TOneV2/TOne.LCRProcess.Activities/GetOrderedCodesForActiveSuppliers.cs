﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.Entities;
using TOne.Data;

namespace TOne.LCRProcess.Activities
{
    #region Arguments Classes

    public class GetOrderedCodesForActiveSuppliersInput
    {
        public bool IsFuture { get; set; }
    }

    public class GetOrderedCodesForActiveSuppliersOutput
    {
        public Dictionary<string, Dictionary<string, LCRCode>> SuppliersOrderedCodes { get; set; }
    }

    #endregion

    public sealed class GetOrderedCodesForActiveSuppliers : BaseAsyncActivity<GetOrderedCodesForActiveSuppliersInput, GetOrderedCodesForActiveSuppliersOutput>
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<string, Dictionary<string, LCRCode>>> SuppliersOrderedCodes { get; set; }

        protected override GetOrderedCodesForActiveSuppliersInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetOrderedCodesForActiveSuppliersInput
            {
                IsFuture=this.IsFuture.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetOrderedCodesForActiveSuppliersOutput result)
        {
            this.SuppliersOrderedCodes.Set(context, result.SuppliersOrderedCodes);
        }

        protected override GetOrderedCodesForActiveSuppliersOutput DoWork(GetOrderedCodesForActiveSuppliersInput inputArgument)
        {
            ICodeDataManager dataManager = DataManagerFactory.GetDataManager<ICodeDataManager>();
            var suppliersOrderedCodes = dataManager.GetOrderedCodesForActiveSuppliers(inputArgument.IsFuture);
            return new GetOrderedCodesForActiveSuppliersOutput
            {
                SuppliersOrderedCodes = suppliersOrderedCodes
            };
        }
    }
}
