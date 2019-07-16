using System;
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
            context.ThrowIfNull("context");
            context.Query.ThrowIfNull("context.Query");

            int userId = SecurityContext.Current.GetLoggedInUserId();
            WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
            var accountsInfo = whSCarrierAccountBEManager.GetCachedClientWhSAccountsInfo(userId);
            accountsInfo.ThrowIfNull("accountInfo", userId);

            List<object> accountIds = new List<object>();
            foreach (var accountInfo in accountsInfo)
            {
                accountIds.Add(accountInfo.Value.AccountId);
            }

            bool hasAccountFilter = false;
            if (context.Query.Filters != null)
            {
                foreach(var filter in context.Query.Filters)
                {
                    if(filter.FieldName == FieldName)
                    {
                        if (filter.FilterValues != null)
                        {
                            foreach (var filterValue in filter.FilterValues)
                            {
                                if (filterValue != null)
                                {
                                    if (!accountIds.Contains(Convert.ToInt32(filterValue.ToString())))
                                        throw new NotSupportedException($"accountId not valid { filterValue }");
                                    hasAccountFilter = true;
                                }
                            }
                        }
                    }
                }
            }
            if (CheckIfFieldNameExist(context.Query.FilterGroup))
                hasAccountFilter = true;
           if (!hasAccountFilter)
            {
                if (context.Query.Filters == null)
                    context.Query.Filters = new List<GenericBusinessEntityFilter>();

                context.Query.Filters.Add(new GenericBusinessEntityFilter()
                {
                    FieldName = this.FieldName,
                    FilterValues = accountIds
                });
            }
        }


        private bool CheckIfFieldNameExist(RecordFilterGroup filterGroup)
        {
            if(filterGroup == null)
                return false;
            if (filterGroup.FieldName == FieldName)
                return true;
            if (filterGroup.Filters == null)
                return false;

            foreach (var filter in filterGroup.Filters)
            {
                if (filter.FieldName == FieldName)
                    return true;

                if (filter is RecordFilterGroup)
                    if (CheckIfFieldNameExist(filter as RecordFilterGroup))
                        return true;
            }
            return false;
        }
        
    }
}
