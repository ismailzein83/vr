using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TABS;
using TOne.CDR.Entities;

namespace TOne.CDR.Business
{
    public class CDRGenerator
    {
        public BillingCDRBase GetBillingCDRBase(Billing_CDR_Base cdrBase)
        {
            BillingCDRBase cdr = new BillingCDRBase();

            cdr.Alert = cdrBase.Alert;
            cdr.Attempt = cdrBase.Attempt;
            cdr.CDPN = cdrBase.CDPN;
            cdr.CDPNOut = cdrBase.CDPNOut;
            cdr.CGPN = cdrBase.CGPN;
            cdr.Connect = cdrBase.Connect;
            cdr.CustomerID = cdrBase.CustomerID;
            cdr.Disconnect = cdrBase.Disconnect;
            cdr.DurationInSeconds = cdrBase.DurationInSeconds;
            cdr.Extra_Fields = cdrBase.Extra_Fields;
            cdr.ID = cdrBase.ID;
            cdr.IsRerouted = cdrBase.IsRerouted;
            cdr.OriginatingZoneID = cdrBase.OriginatingZone == null ? -1 : cdrBase.OriginatingZone.ZoneID;
            cdr.OurCode = cdrBase.OurCode;
            cdr.OurZoneID = cdrBase.OurZone == null ? -1 : cdrBase.OurZone.ZoneID;
            cdr.Port_IN = cdrBase.Port_IN;
            cdr.Port_OUT = cdrBase.Port_OUT;
            cdr.ReleaseCode = cdrBase.ReleaseCode;
            cdr.ReleaseSource = cdrBase.ReleaseSource;
            cdr.SIP = cdrBase.SIP;
            cdr.SubscriberID = cdrBase.SubscriberID;
            cdr.SupplierCode = cdrBase.SupplierCode;
            cdr.SupplierID = cdrBase.SupplierID;
            cdr.SupplierZoneID = cdrBase.SupplierZone == null ? -1 : cdrBase.SupplierZone.ZoneID;
            cdr.SwitchCdrID = cdrBase.SwitchCdrID;
            cdr.SwitchID = cdrBase.Switch == null ? -1 : cdrBase.Switch.SwitchID;
            cdr.Tag = cdrBase.Tag;
            cdr.IsValid = cdrBase.IsValid;

            return cdr;
        }

        public Billing_CDR_Base GenerateBillingCdr(TOne.Business.ProtCodeMap codeMap, TABS.CDR cdr)
        {
            Billing_CDR_Base billingCDR = null;

            if (cdr.DurationInSeconds > 0)
            {
                billingCDR = new Billing_CDR_Main();
            }
            else
                billingCDR = new Billing_CDR_Invalid();

            bool valid = cdr.Switch.SwitchManager.FillCDRInfo(cdr.Switch, cdr, billingCDR);

            GenerateZones(codeMap, billingCDR);

            // If there is a duration and missing supplier (zone) or Customer (zone) info
            // then it is considered invalid
            if (billingCDR is Billing_CDR_Main)
                if (!valid
                    || billingCDR.Customer.RepresentsASwitch
                    || billingCDR.Supplier.RepresentsASwitch
                    || billingCDR.CustomerID == null
                    || billingCDR.SupplierID == null
                    || billingCDR.OurZone == null
                    || billingCDR.SupplierZone == null
                    || billingCDR.Customer.ActivationStatus == ActivationStatus.Inactive
                    || billingCDR.Supplier.ActivationStatus == ActivationStatus.Inactive)
                {
                    billingCDR = new Billing_CDR_Invalid(billingCDR);
                }
            return billingCDR;
        }

        private void GenerateZones(TOne.Business.ProtCodeMap codeMap, Billing_CDR_Base cdr)
        {
            // Our Zone
            Code ourCurrentCode = codeMap.Find(cdr.CDPN, CarrierAccount.SYSTEM, cdr.Attempt);
            if (ourCurrentCode != null)
            {
                cdr.OurZone = ourCurrentCode.Zone;
                cdr.OurCode = ourCurrentCode.Value;
            }

            // Originating Zone
            if (cdr.CustomerID != null && CarrierAccount.All.ContainsKey(cdr.CustomerID))
            {
                CarrierAccount customer = CarrierAccount.All[cdr.CustomerID];
                if (customer.IsOriginatingZonesEnabled)
                {
                    if (cdr.CGPN != null && cdr.CGPN.Trim().Length > 0)
                    {
                        string orginatingCode = _invalidCGPNDigits.Replace(cdr.CGPN, "");
                        Code originatingCode = codeMap.Find(orginatingCode, CarrierAccount.SYSTEM, cdr.Attempt);
                        if (originatingCode != null)
                            cdr.OriginatingZone = originatingCode.Zone;
                    }
                }
            }

            // Supplier Zone
            if (cdr.SupplierID != null && CarrierAccount.All.ContainsKey(cdr.SupplierID))
            {
                CarrierAccount supplier = CarrierAccount.All[cdr.SupplierID];
                Code supplierCode = null;

                if (TABS.SystemParameter.AllowCostZoneCalculationFromCDPNOut.BooleanValue.Value)
                {
                    if (string.IsNullOrEmpty(cdr.CDPNOut))
                        supplierCode = codeMap.Find(cdr.CDPN, supplier, cdr.Attempt);
                    else
                        supplierCode = codeMap.Find(cdr.CDPNOut, supplier, cdr.Attempt);
                }
                else
                    supplierCode = codeMap.Find(cdr.CDPN, supplier, cdr.Attempt);

                if (supplierCode != null)
                {
                    cdr.SupplierZone = supplierCode.Zone;
                    cdr.SupplierCode = supplierCode.Value;
                }
            }
        }

        private System.Text.RegularExpressions.Regex _invalidCGPNDigits = new System.Text.RegularExpressions.Regex("[^0-9]", System.Text.RegularExpressions.RegexOptions.Compiled);

    }
}
