using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TABS;
using TOne.Business;
using TOne.Entities;
using Vanrise.BusinessProcess;

namespace TOne.CDRProcess.Activities
{
    #region Arguments Classes

    public class SaveMainCDRsToDBInput
    {
        public TOneQueue<CDRMainBatch> InputQueue { get; set; }

    }

    #endregion

    public sealed class SaveMainCDRsToDB : DependentAsyncActivity<SaveMainCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument <TOneQueue<CDRMainBatch>> InputQueue { get; set; }

        protected override void DoWork(SaveMainCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((mainCDR) =>
                    {

                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override SaveMainCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new SaveMainCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }




      //  void SaveMainCdrs(List<Billing_CDR_Base> billingCDRs)
      //  {
      //      if (billingCDRs != null && billingCDRs.Count > 0)
      //      {

      //          CDRManager cdrManager = new CDRManager();
      //          var mainRecords = billingCDRs.Where(c => c.IsValid);
      //          //var invalidRecords = billingCDRs.Where(c => !c.IsValid);

      //          long mainId = cdrManager.ReserveRePricingMainCDRIDs(mainRecords.Count());
      //          //long invalidId = cdrManager.ReserveRePricingInvalidCDRIDs(invalidRecords.Count());


      //          DataTable dtMain = GetBillingCdrTable(mainRecords, BulkManager.MAIN_TABLE_NAME, mainId);

      //          //DataTable dtInvalid = GetBillingCdrTable(invalidRecords, BulkManager.INVALID_TABLE_NAME, invalidId);

      //          DataTable dtCost;
      //          DataTable dtSale;
      //          GetPricingTables(mainRecords, out dtSale, out dtCost);
      //          BulkManager.Instance.Write(dtMain.TableName, dtMain);
      //      }
      //  }

      //  void GetPricingTables(IEnumerable<Billing_CDR_Base> billingMainData, out DataTable saleTable, out DataTable costTable)
      //  {
      //      //log.DebugFormat("Bulk Building Data table: {0}", tableName);
      //      //saleTable = PricingTable.Clone();
      //      saleTable.TableName = BulkManager.SALE_TABLE_NAME;
      //      saleTable.BeginLoadData();

      //      //costTable = PricingTable.Clone();
      //      costTable.TableName = BulkManager.COST_TABLE_NAME;
      //      costTable.BeginLoadData();

      //      Action<Billing_CDR_Pricing_Base, DataRow> fillRow = (pricing, row) =>
      //      {
      //          int index = -1;
      //          index++; row[index] = pricing.Billing_CDR_Main.ID;
      //          index++; row[index] = pricing.Zone.ZoneID;
      //          index++; row[index] = pricing.Net;
      //          index++; row[index] = pricing.Currency.Symbol;
      //          index++; row[index] = pricing.RateValue;
      //          index++; row[index] = pricing.Rate.ID;
      //          index++; row[index] = pricing.Discount.HasValue ? (object)pricing.Discount.Value : DBNull.Value;
      //          index++; row[index] = pricing.RateType;
      //          index++; row[index] = pricing.ToDConsideration == null ? DBNull.Value : (object)pricing.ToDConsideration.ToDConsiderationID;
      //          index++; row[index] = pricing.FirstPeriod.HasValue ? (object)pricing.FirstPeriod.Value : DBNull.Value;
      //          index++; row[index] = pricing.RepeatFirstperiod.HasValue ? (object)pricing.RepeatFirstperiod.Value : DBNull.Value;
      //          index++; row[index] = pricing.FractionUnit.HasValue ? (object)pricing.FractionUnit.Value : DBNull.Value;
      //          index++; row[index] = pricing.Tariff == null ? DBNull.Value : (object)pricing.Tariff.TariffID;
      //          index++; row[index] = pricing.CommissionValue;
      //          index++; row[index] = pricing.Commission == null ? DBNull.Value : (object)pricing.Commission.CommissionID;
      //          index++; row[index] = pricing.ExtraChargeValue;
      //          index++; row[index] = pricing.ExtraCharge == null ? DBNull.Value : (object)pricing.ExtraCharge.CommissionID;
      //          index++; row[index] = pricing.Updated;
      //          index++; row[index] = pricing.DurationInSeconds;
      //          index++; row[index] = pricing.Code == null ? DBNull.Value : (object)pricing.Code;
      //          index++; row[index] = pricing.Attempt = pricing.Billing_CDR_Main.Attempt;
      //      };


      //      foreach (Billing_CDR_Main pricing in billingMainData)
      //      {
      //          if (pricing.Billing_CDR_Sale != null)
      //          {
      //              DataRow row = saleTable.NewRow();
      //              fillRow(pricing.Billing_CDR_Sale, row);
      //              saleTable.Rows.Add(row);
      //          }

      //          if (pricing.Billing_CDR_Cost != null)
      //          {
      //              DataRow row = costTable.NewRow();
      //              fillRow(pricing.Billing_CDR_Cost, row);
      //              costTable.Rows.Add(row);
      //          }

      //      }
      //      saleTable.EndLoadData();
      //      costTable.EndLoadData();
      //  }

      //private  DataTable GetBillingCdrTable(IEnumerable<Billing_CDR_Base> cdrData, string tableName, long startingID)
      //  {
      //      //log.DebugFormat("Bulk Building Data table: {0}", tableName);
      //      DataTable dt = BillingCdrTable.Clone();
      //      bool includeRerouted = tableName.ToLower().EndsWith("invalid");
      //      if (!includeRerouted) dt.Columns.Remove("IsRerouted");
      //      dt.TableName = tableName;
      //      dt.BeginLoadData();

      //      long billingCDRID = startingID;
      //      foreach (var cdr in cdrData)
      //      {
      //          cdr.ID = billingCDRID;
      //          billingCDRID--;
      //          DataRow row = dt.NewRow();
      //          int index = -1;
      //          index++; row[index] = cdr.ID;
      //          index++; row[index] = cdr.Attempt;
      //          index++; row[index] = cdr.Alert.HasValue && cdr.Alert.Value != DateTime.MinValue ? (object)cdr.Alert : DBNull.Value;
      //          index++; row[index] = cdr.Connect.HasValue && cdr.Connect.Value != DateTime.MinValue ? (object)cdr.Connect : DBNull.Value;
      //          index++; row[index] = cdr.Disconnect.HasValue && cdr.Disconnect.Value != DateTime.MinValue ? (object)cdr.Disconnect : DBNull.Value;
      //          index++; row[index] = cdr.DurationInSeconds;
      //          index++; row[index] = cdr.CustomerID;
      //          index++; row[index] = cdr.OurZone == null ? DBNull.Value : (object)cdr.OurZone.ZoneID;
      //          index++; row[index] = cdr.OriginatingZone == null ? DBNull.Value : (object)cdr.OriginatingZone.ZoneID;
      //          index++; row[index] = cdr.SupplierID;
      //          index++; row[index] = cdr.SupplierZone == null ? DBNull.Value : (object)cdr.SupplierZone.ZoneID;
      //          index++; row[index] = cdr.CDPN;
      //          index++; row[index] = cdr.CGPN;
      //          index++; row[index] = cdr.CDPNOut;
      //          index++; row[index] = cdr.ReleaseCode;
      //          index++; row[index] = cdr.ReleaseSource;
      //          index++; row[index] = (byte)cdr.Switch.SwitchID;
      //          index++; row[index] = cdr.SwitchCdrID;
      //          index++; row[index] = cdr.Tag;
      //          index++; row[index] = cdr.Extra_Fields;
      //          index++; row[index] = cdr.Port_IN;
      //          index++; row[index] = cdr.Port_OUT;
      //          index++; row[index] = cdr.OurCode != null ? (object)cdr.OurCode : DBNull.Value;
      //          index++; row[index] = cdr.SupplierCode != null ? (object)cdr.SupplierCode : DBNull.Value;

      //          if (includeRerouted) { index++; row[index] = cdr.IsRerouted ? "Y" : "N"; }
      //          dt.Rows.Add(row);
      //      }
      //      dt.EndLoadData();
      //      return dt;
      //  }
    }
}
