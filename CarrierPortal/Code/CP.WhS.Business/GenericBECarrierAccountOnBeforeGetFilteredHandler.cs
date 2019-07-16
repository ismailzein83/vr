﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace CP.WhS.Business
{
    class GenericBECarrierAccountOnBeforeGetFilteredHandler : GenericBEOnBeforeGetFilteredHandler
    {
        public override Guid ConfigId { get { return new Guid("35FCFE27-A471-449F-AD29-F26296FD513F"); } }

        public string FieldName { get; set; }

        public override void onBeforeAdd(IGenericBEOnBeforeAddHandlerContext context)
        {}

        public override void onBeforeUpdate(IGenericBEOnBeforeUpdateHandlerContext context)
        {}

        public override void PrepareQuery(IGenericBEOnBeforeGetFilteredHandlerPrepareQueryContext context)
        {
            //if(context != null && context.Query!= null)
            //{
            //    int userId = SecurityContext.Current.GetLoggedInUserId();
            //    WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
            //    var accountsInfo = whSCarrierAccountBEManager.GetCachedClientWhSAccountsInfo(userId);
            //    accountsInfo.ThrowIfNull("accountInfo", userId);

            //    List<object> accountIds = new List<object>();
            //    foreach(var accountInfo in accountsInfo)
            //    {
            //        accountIds.Add(accountInfo.Value.AccountId);
            //    }
               
            //    if (context.Query.Filters == null)
            //        context.Query.Filters = new List<GenericBusinessEntityFilter>();

            //    context.Query.Filters.Add(new GenericBusinessEntityFilter()
            //    {
            //        FieldName = this.FieldName,
            //        FilterValues = accountIds
            //    });
            //}
        }
    }
}
