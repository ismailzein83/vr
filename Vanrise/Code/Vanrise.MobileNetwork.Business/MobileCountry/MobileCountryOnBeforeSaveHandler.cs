using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.GenericData.Business;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.MobileNetwork.Entities;

namespace Vanrise.MobileNetwork.Business
{
    public class MobileCountryOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("0d1caab7-6ded-4f00-8df4-261ab4a18210"); } }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            MobileCountryManager mobileCountryManager = new MobileCountryManager();
            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            var countryId = context.GenericBusinessEntity.FieldValues.GetRecord("Country");
            countryId.ThrowIfNull("country");
            var code = context.GenericBusinessEntity.FieldValues.GetRecord("Code");
            code.ThrowIfNull("code");
            var id = context.GenericBusinessEntity.FieldValues.GetRecord("ID");
            int? mobileCountryId = (int?)id;
           var mobileCountryByCode = mobileCountryManager.GetMobileCountryByCode((string)code);
            if(mobileCountryByCode != null && mobileCountryId != mobileCountryByCode.Id)
            {
                context.OutputResult.Result = false;
                context.OutputResult.Messages.Add(string.Format("The code '{0}' is already used by another Mobile Country",code));
            }
            var mobileCountryByCountryId = mobileCountryManager.GetMobileCountryByCountryId(Convert.ToInt32(countryId));
            if (mobileCountryByCountryId != null && mobileCountryId != mobileCountryByCountryId.Id)
            {
                context.OutputResult.Result = false;
                context.OutputResult.Messages.Add(string.Format("The country added already exists"));
            }
        }
    }
}
