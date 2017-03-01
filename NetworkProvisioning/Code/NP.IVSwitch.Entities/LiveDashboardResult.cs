using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class LiveDashboardResult
    {
        public TopCustomersResult TopCustomersResult { get; set; }
        public TopSuppliersResult TopSuppliersResult { get; set; }
        public TopZonesResult TopZonesResult { get; set; }
        public LastDistributionResult LastDistributionResult { get; set; }


        public TotalDurationResult TotalDurationResult { get; set; }
        public LiveSummaryResult LiveSummaryResult { get; set; }
    }
    public class TopZonesResult
    {
        public List<ZoneResult> ZoneResults { get; set; }
    }
    public class ZoneResult
    {
        public string ZoneName { get; set; }
        public int ZoneId { get; set; }
        public int Attempts { get; set; }
        public int CountConnected { get; set; }
        public decimal PercConnected { get; set; }
        public decimal ACD { get; set; }
        public decimal PDDInSec { get; set; }
        public decimal TotalDuration { get; set; }
    }
    public class TopCustomersResult
    {
        public List<CustomerResult> CustomerResults { get; set; }
    }
    public class CustomerResult
    {
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public int Attempts { get; set; }
        public int CountConnected { get; set; }
        public decimal PercConnected { get; set; }
        public decimal ACD { get; set; }
        public decimal PDDInSec { get; set; }
        public decimal TotalDuration { get; set; }
    }
    public class TopSuppliersResult
    {
        public List<SupplierResult> SupplierResults { get; set; }

    }
    public class SupplierResult
    {
        public string SupplierName { get; set; }
        public int SupplierId { get; set; }
        public int Attempts { get; set; }
        public int CountConnected { get; set; }
        public decimal PercConnected { get; set; }
        public decimal ACD { get; set; }
        public decimal PDDInSec { get; set; }
        public decimal TotalDuration { get; set; }

    }
    public class LastDistributionResult
    {
        public List<DistributionResult> DistributionResults { get; set; }
    }
    public class DistributionResult
    {
        public string DurationRange { get; set; }
        public int Attempts { get; set; }
        public int CountConnected { get; set; }
        public decimal PercConnected { get; set; }
        public decimal ACD { get; set; }
        public decimal PDDInSec { get; set; }
        public decimal TotalDuration { get; set; }

    }
    public class TotalDurationResult
    {

    }
    public class LiveSummaryResult
    {
        public int Attempts { get; set; }
        public int CountConnected { get; set; }
        public decimal PercConnected { get; set; }
        public decimal ACD { get; set; }
        public decimal PDDInSec { get; set; }
        public decimal TotalDuration { get; set; }
    }

}
