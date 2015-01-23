using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class UpdateZoneMatchInDBInput
    {
        public bool IsFuture { get; set; }
        public char FirstDigit { get; set; }
    }

    #endregion

    public sealed class UpdateZoneMatchInDB : BaseAsyncActivity<UpdateZoneMatchInDBInput>
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<char> FirstDigit { get; set; }

        protected override UpdateZoneMatchInDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new UpdateZoneMatchInDBInput
            {
                IsFuture = this.IsFuture.Get(context),
                FirstDigit = this.FirstDigit.Get(context)
            };
        }
        protected override void DoWork(UpdateZoneMatchInDBInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime start = DateTime.Now;
            IZoneMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneMatchDataManager>();
            dataManager.UpdateAll(inputArgument.IsFuture);
            Console.WriteLine("{0}: UpdateZoneMatchInDB is done in {1}", DateTime.Now, (DateTime.Now - start));
        }
    }
}
