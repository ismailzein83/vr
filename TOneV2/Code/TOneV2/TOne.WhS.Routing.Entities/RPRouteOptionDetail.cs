﻿using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionDetail
    {
        public long SaleZoneId { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierZoneName { get; set; }
        public HashSet<int> SupplierServicesIds { get; set; }
        public string SupplierServicesNames { get; set; }
        public Decimal? SupplierRate { get; set; }
        public int? Percentage { get; set; }
        public bool SupplierZoneMatchHasClosedRate { get; set; }
        public Decimal? ConvertedSupplierRate { get; set; }
        public string CurrencySymbol { get; set; }
        public int OptionOrder { get; set; }
        public decimal? ACD { get; set; }
        public decimal? ASR { get; set; }
        public decimal? Duration { get; set; }
        public int? ExecutedRuleId { get; set; }
        public SupplierStatus SupplierStatus { get; set; }
        public bool IsForced { get; set; }
        public RouteOptionEvaluatedStatus? EvaluatedStatus { get; set; }
    }

    public class BaseRPRouteOptionByCodeDetail
    {
        public int SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public Decimal SupplierRate { get; set; }
        public long SupplierZoneId { get; set; }
        public string SupplierZoneName { get; set; }
        public decimal ConvertedSupplierRate { get; set; }
        public string CurrencySymbol { get; set; }
        public int? ExecutedRuleId { get; set; }
        public RouteOptionEvaluatedStatus? EvaluatedStatus { get; set; }
        public bool SupplierZoneMatchHasClosedRate { get; set; }
    }
    public class RPRouteOptionByCodeDetail : BaseRPRouteOptionByCodeDetail
    {
        public string ExactSupplierServiceSymbols { get; set; }
        public int OptionOrder { get; set; }
        public int? Percentage { get; set; }
        public List<RPRouteBackupOptionByCodeDetail> Backups { get; set; }
    }

    public class RPRouteBackupOptionByCodeDetail : BaseRPRouteOptionByCodeDetail
    {

    }
}