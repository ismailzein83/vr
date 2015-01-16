﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.Entities;
using System.Collections.Concurrent;
using TOne.Data;
using TABS;

namespace TOne.LCRProcess.Activities
{
    #region Arguments Classes

    public class LoadCodesForActiveSuppliersInput
    {
        public bool IsFuture { get; set; }

        public char FirstDigit { get; set; }
        public DateTime EffectiveOn { get; set; }
        public bool GetChangedGroupsOnly { get; set; }

        public ConcurrentQueue<Tuple<string, List<LCRCode>>> QueueLoadedCodes { get; set; }
    }

    #endregion

    public sealed class LoadCodesForActiveSuppliers : BaseAsyncActivity<LoadCodesForActiveSuppliersInput>
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<char> FirstDigit { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> GetChangedGroupsOnly { get; set; }

        [RequiredArgument]
        public InArgument<ConcurrentQueue<Tuple<string, List<LCRCode>>>> QueueLoadedCodes { get; set; }

        protected override LoadCodesForActiveSuppliersInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCodesForActiveSuppliersInput
            {
                IsFuture = this.IsFuture.Get(context),
                FirstDigit = this.FirstDigit.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                GetChangedGroupsOnly = this.GetChangedGroupsOnly.Get(context),
                QueueLoadedCodes = this.QueueLoadedCodes.Get(context)
            };
        }
        protected override void DoWork(LoadCodesForActiveSuppliersInput inputArgument)
        {
            ICodeDataManager dataManager = DataManagerFactory.GetDataManager<ICodeDataManager>();
            DateTime start = DateTime.Now;
            dataManager.LoadCodesForActiveSuppliers(inputArgument.IsFuture, inputArgument.EffectiveOn, inputArgument.GetChangedGroupsOnly, (supplierId, supplierCodes) =>
                {
                    //Console.WriteLine("{0}: {1} codes loaded for supplier {2}", DateTime.Now, supplierCodes.Count, supplierId);
                    inputArgument.QueueLoadedCodes.Enqueue(new Tuple<string, List<LCRCode>>(supplierId, supplierCodes));
                });
            Console.WriteLine("{0}: LoadCodesForActiveSuppliers is done in {1}", DateTime.Now, (DateTime.Now - start));
        }
    }
}
