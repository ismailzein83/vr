﻿using PartnerPortal.CustomerAccess.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Business;
using Vanrise.GenericData.Entities;

namespace PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIRecordQueryInterceptor
{
    public class RetailAccountVRRestAPIRecordQueryInterceptor : Vanrise.GenericData.Entities.VRRestAPIRecordQueryInterceptor
    {
        public override Guid ConfigId { get { return new Guid("B3A94A20-92ED-47BF-86D6-1034B720BE73"); } }

        public override void PrepareQuery(Vanrise.GenericData.Entities.IVRRestAPIRecordQueryInterceptorContext context)
        {
            var vrRestAPIRecordQueryInterceptorContext = context as Vanrise.GenericData.Business.VRRestAPIRecordQueryInterceptorContext;
            if (vrRestAPIRecordQueryInterceptorContext == null)
                throw new Exception("vrRestAPIRecordQueryInterceptorContext is not of type VRRestAPIRecordQueryInterceptorContext.");

            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();

            NumberRecordFilter numberRecordFilter = new NumberRecordFilter()
            {
                FieldName = "FinancialAccountId",
                CompareOperator = NumberRecordFilterOperator.Equals,
                Value = manager.GetRetailAccountId(userId)
            };

            if (vrRestAPIRecordQueryInterceptorContext.Query == null)
                vrRestAPIRecordQueryInterceptorContext.Query = new DataRecordQuery();

            if (vrRestAPIRecordQueryInterceptorContext.Query.FilterGroup == null)
            {
                vrRestAPIRecordQueryInterceptorContext.Query.FilterGroup = new RecordFilterGroup() { Filters = new List<RecordFilter>() { numberRecordFilter }, LogicalOperator = RecordQueryLogicalOperator.And };
            }
            else
            {
                var existingFilterGroup = vrRestAPIRecordQueryInterceptorContext.Query.FilterGroup;
                vrRestAPIRecordQueryInterceptorContext.Query.FilterGroup = new RecordFilterGroup() { Filters = new List<RecordFilter>(), LogicalOperator = RecordQueryLogicalOperator.And };
                vrRestAPIRecordQueryInterceptorContext.Query.FilterGroup.Filters.Add(existingFilterGroup);
                vrRestAPIRecordQueryInterceptorContext.Query.FilterGroup.Filters.Add(numberRecordFilter);
            }
        }
    }
}
