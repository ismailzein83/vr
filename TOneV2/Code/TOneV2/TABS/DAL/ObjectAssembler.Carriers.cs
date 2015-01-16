using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using NHibernate.Criterion;

namespace TABS.DAL
{
    public partial class ObjectAssembler
    {
        public static IList<CarrierAccount> GetCarrierAccountsBySuffix(CarrierAccount account)
        {
            return QueryBuilder.GetCarrierAccountsBySuffixQuery(account).List<CarrierAccount>();
        }

        public static DataTable GetCarrierAccountIDs()
        {
            return TABS.DataHelper.GetDataTable(QueryBuilder.GetCarrierAccountIDsQuery());
        }

        public static IList<TABS.Zone> GetZones(TABS.CarrierAccount supplier, DateTime when)
        {
            IList<TABS.Zone> list = QueryBuilder.GetZonesQuery(supplier, when)
                    .List<TABS.Zone>();
            return list;
        }

        public static IList<TABS.Commission> GetCommissions(CarrierAccount customer, CarrierAccount supplier, Zone zone, DateTime? effectiveDate)
        {
            NHibernate.ICriteria criteria = QueryBuilder.GetCommissionsQuery(customer, supplier, zone, effectiveDate);
            return criteria.List<TABS.Commission>().Where(t => !t.Supplier.IsDeleted).ToList();
        }
        //For Custome Paging
        public static IList<TABS.Commission> GetCommissions(CarrierAccount customer, CarrierAccount supplier, Zone zone, DateTime? effectiveDate, int CurrentPageIndex, int PageSize, out int RecordCount)
        {
            NHibernate.ICriteria criteriaCount = QueryBuilder.GetCommissionsQuery(customer, supplier, zone, effectiveDate);
            criteriaCount.List<TABS.Commission>().Where(t => !t.Supplier.IsDeleted).ToList();
            RecordCount=criteriaCount.SetProjection(Projections.RowCount()).UniqueResult<int>();

            NHibernate.ICriteria criteria = QueryBuilder.GetCommissionsQuery(customer, supplier, zone, effectiveDate);
            criteria.List<TABS.Commission>().Where(t => !t.Supplier.IsDeleted).ToList();
            
            return criteria.SetFirstResult(PageSize * (CurrentPageIndex - 1)).SetMaxResults(PageSize).List<TABS.Commission>();
        }
        public static IList<TABS.ToDConsideration> GetToDs(TABS.CarrierAccount supplier, TABS.CarrierAccount customer, TABS.Zone zone, DateTime? effectiveDate)
        {
            NHibernate.ICriteria criteria = QueryBuilder.GetToDsQuery(supplier, customer, zone, effectiveDate);
            return criteria.List<TABS.ToDConsideration>().Where(t => !t.Supplier.IsDeleted).ToList();
        }
    }
}
