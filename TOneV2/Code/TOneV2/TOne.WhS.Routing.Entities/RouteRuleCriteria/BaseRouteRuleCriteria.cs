using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Entities
{
    public abstract class BaseRouteRuleCriteria
    {
        public abstract Guid ConfigId { get; }
        public Dictionary<string, object> FieldValues { get; set; }

        public virtual RoutingExcludedDestinations GetExcludedDestinations() { return null; } 
        public virtual int? GetRoutingProductId() { return null; }
        public virtual CodeCriteriaGroupSettings GetCodeCriteriaGroupSettings() { return null; }
        public virtual SaleZoneGroupSettings GetSaleZoneGroupSettings() { return null; }
        public virtual CustomerGroupSettings GetCustomerGroupSettings() { return null; }

        protected object GetFieldValue(string fieldName)
        {
            FieldValues.ThrowIfNull("FieldValues");

            object value;
            if (!FieldValues.TryGetValue(fieldName, out value))
                throw new NullReferenceException();

            return value;
        }

        public virtual bool IsVisibleInManagementView()
        {
            return true;
        }
    }
}