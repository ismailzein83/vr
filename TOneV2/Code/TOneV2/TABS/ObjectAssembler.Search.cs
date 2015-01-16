using System;
using System.Collections.Generic;
using NHibernate.Criterion;

namespace TABS
{
    public partial class ObjectAssembler
    {

        public static IList<SpecialRequest> GetSpecialRequests(CarrierAccount customer, Zone zone, string code, CarrierAccount supplier, SpecialRequestType? specialRequestType, DateTime when)
        {
            return GetSpecialRequests(customer, zone, code, supplier, specialRequestType, when);
        }

        public static IList<SpecialRequest> GetSpecialRequests(CarrierAccount customer, Zone zone, string code, CarrierAccount supplier, SpecialRequestType? specialRequestType, DateTime? when)
        {
            return GetSpecialRequests(customer, zone, code, supplier, specialRequestType, when.HasValue ? (DateTime)when : DateTime.MinValue, false);
        }

        public static IList<SpecialRequest> GetSpecialRequests(CarrierAccount customer, Zone zone, string code, CarrierAccount supplier, SpecialRequestType? specialRequestType, DateTime? when, int pageIndex, int pageSize, out int recordCount)
        {
            return GetSpecialRequests(customer, zone, code, supplier, specialRequestType, when.HasValue ? (DateTime)when : DateTime.MinValue, false, pageIndex, pageSize, out recordCount);
        }

        public static IList<SpecialRequest> GetSpecialRequests(CarrierAccount customer, Zone zone, string code, CarrierAccount supplier, SpecialRequestType? specialRequestType, DateTime when, bool forRouteManager, int pageIndex, int pageSize, out int recordCount)
        {
            IList<SpecialRequest> results = null;

            NHibernate.ICriteria criteria = CurrentSession.CreateCriteria(typeof(SpecialRequest));

            NHibernate.ICriteria criteriaCount = CurrentSession.CreateCriteria(typeof(SpecialRequest));

            if (when != DateTime.MinValue)
                if (!forRouteManager)
                {
                    criteria
                            .Add(Expression.Le("BeginEffectiveDate", when))
                            .Add(Expression.Or(
                                 Expression.Gt("EndEffectiveDate", when),
                                new NullExpression("EndEffectiveDate"))
                                )
                            .AddOrder(new Order("BeginEffectiveDate", false));

                    criteriaCount
                            .Add(Expression.Le("BeginEffectiveDate", when))
                            .Add(Expression.Or(
                                 Expression.Gt("EndEffectiveDate", when),
                                new NullExpression("EndEffectiveDate"))
                                );
                }
                else
                {
                    criteria
                            .Add(Expression.Or(
                                       Expression.And(
                                         Expression.Le("BeginEffectiveDate", when),
                                          Expression.Or(
                                            Expression.Gt("EndEffectiveDate", when),
                                             new NullExpression("EndEffectiveDate"))),
                                    Expression.Ge("BeginEffectiveDate", when)
                                    ))
                                .AddOrder(new Order("BeginEffectiveDate", false));

                    criteriaCount
                            .Add(Expression.Or(
                                      Expression.And(
                                         Expression.Le("BeginEffectiveDate", when),
                                         Expression.Or(
                                             Expression.Gt("EndEffectiveDate", when),
                                             new NullExpression("EndEffectiveDate"))),
                                    Expression.Ge("BeginEffectiveDate", when)
                                    ));
                }
            // If looking for an account that can be either...
            if (customer != null && customer == supplier)
            {
                criteria.Add(Expression.Or(
                        Expression.Eq("Customer", customer),
                        Expression.Eq("Supplier", supplier)
                        ));

                criteriaCount.Add(Expression.Or(
                        Expression.Eq("Customer", customer),
                        Expression.Eq("Supplier", supplier)
                        ));
            }
            else
            {
                if (customer != null)
                {
                    criteria.Add(Expression.Eq("Customer", customer));
                    criteriaCount.Add(Expression.Eq("Customer", customer));
                }
                if (supplier != null)
                {
                    criteria.Add(Expression.Eq("Supplier", supplier));
                    criteriaCount.Add(Expression.Eq("Supplier", supplier));
                }
            }

            if (zone != null)
            {
                criteria.Add(Expression.Eq("Zone", zone));
                criteriaCount.Add(Expression.Eq("Zone", zone));
            }
            if (code != null)
            {
                criteria.Add(Expression.Like("Code", code));
                criteriaCount.Add(Expression.Like("Code", code));
            }
            if (specialRequestType != null)
            {
                criteria.Add(Expression.Eq("SpecialRequestType", specialRequestType));
                criteriaCount.Add(Expression.Eq("SpecialRequestType", specialRequestType));
            }

            results = criteria.SetFirstResult(pageSize * (pageIndex - 1)).SetMaxResults(pageSize).List<SpecialRequest>();

            recordCount = criteriaCount.SetProjection(NHibernate.Criterion.Projections.RowCount()).UniqueResult<int>();

            return results;
        }


        public static IList<SpecialRequest> GetSpecialRequests(CarrierAccount customer, Zone zone, string code, CarrierAccount supplier, SpecialRequestType? specialRequestType, DateTime when, bool forRouteManager)
        {
            IList<SpecialRequest> results = null;

            NHibernate.ICriteria criteria = CurrentSession.CreateCriteria(typeof(SpecialRequest));

            if (when != DateTime.MinValue)
                if (!forRouteManager)
                {
                    criteria
                            .Add(Expression.Le("BeginEffectiveDate", when))
                            .Add(Expression.Or(
                                Expression.Gt("EndEffectiveDate", when),
                                new NullExpression("EndEffectiveDate"))
                                )
                            .AddOrder(new Order("BeginEffectiveDate", false));
                }
                else
                {
                    criteria
                            .Add(Expression.Or(
                                      Expression.And(
                                         Expression.Le("BeginEffectiveDate", when),
                                         Expression.Or(
                                             Expression.Gt("EndEffectiveDate", when),
                                             new NullExpression("EndEffectiveDate"))),
                                    Expression.Ge("BeginEffectiveDate", when)
                                    ))
                                .AddOrder(new Order("BeginEffectiveDate", false));
                }
            // If looking for an account that can be either...
            if (customer != null && customer == supplier)
            {
                criteria.Add(Expression.Or(
                        Expression.Eq("Customer", customer),
                        Expression.Eq("Supplier", supplier)
                        ));
            }
            else
            {
                if (customer != null) criteria.Add(Expression.Eq("Customer", customer));
                if (supplier != null) criteria.Add(Expression.Eq("Supplier", supplier));
            }

            if (zone != null) criteria.Add(Expression.Eq("Zone", zone));
            if (code != null) criteria.Add(Expression.Like("Code", code));
            if (specialRequestType != null) criteria.Add(Expression.Eq("SpecialRequestType", specialRequestType));
            results = criteria.List<SpecialRequest>();

            return results;
        }

        public static IList<RouteBlock> GetRouteBlocks(CarrierAccount customer, Zone zone, string code, CarrierAccount supplier, RouteBlockType? routeBlockType, DateTime when)
        {
            return GetRouteBlocks(customer, zone, code, supplier, routeBlockType, when, false);
        }
        public static IList<RouteBlock> GetRouteBlocks(CarrierAccount customer, Zone zone, string code, CarrierAccount supplier, RouteBlockType? routeBlockType, DateTime when, bool forRouteManager)
        {
            IList<RouteBlock> results = null;

            NHibernate.ICriteria criteria;

            if (!forRouteManager)
            {
                criteria = CurrentSession.CreateCriteria(typeof(RouteBlock))
                        .Add(Expression.Le("BeginEffectiveDate", when))
                        .Add(Expression.Or(
                            Expression.Gt("EndEffectiveDate", when),
                            new NullExpression("EndEffectiveDate"))
                            )
                        .AddOrder(new Order("BeginEffectiveDate", false));

            }
            else
            {
                criteria = CurrentSession.CreateCriteria(typeof(RouteBlock))
                     .Add(Expression.Or(
                          Expression.And(
                             Expression.Le("BeginEffectiveDate", when),
                             Expression.Or(
                                 Expression.Gt("EndEffectiveDate", when),
                                 new NullExpression("EndEffectiveDate"))),
                        Expression.Ge("BeginEffectiveDate", when)
                        ))
                    .AddOrder(new Order("BeginEffectiveDate", false));
            }
            // If looking for an account that can be either...
            if (customer != null && customer == supplier)
            {
                criteria.Add(Expression.Or(
                       Expression.Eq("Customer", customer),
                        Expression.Eq("Supplier", supplier)
                        ));
            }
            else
            {
                if (customer != null) criteria.Add(Expression.Eq("Customer", customer));
                if (supplier != null) criteria.Add(Expression.Eq("Supplier", supplier));
            }

            if (zone != null) criteria.Add(Expression.Eq("Zone", zone));
            if (code != null) criteria.Add(Expression.Like("Code", code));
            if (routeBlockType != null) criteria.Add(Expression.Eq("BlockType", routeBlockType));
            results = criteria.List<RouteBlock>();

            return results;
        }

    }
}
