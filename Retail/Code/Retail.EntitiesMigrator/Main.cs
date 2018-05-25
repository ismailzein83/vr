using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Cells;
using Retail.BusinessEntity.Entities;
using Retail.EntitiesMigrator.Entities;
using Retail.EntitiesMigrator.Migrators;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.EntitiesMigrator
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            cboSNP.DisplayMember = "Name";
            cboSNP.ValueMember = "SellingNumberPlanId";
            cboCurrency.DisplayMember = "Symbol";
            cboCurrency.ValueMember = "CurrencyId";
            SellingNumberPlanManager sellingNumberPlanManager = new SellingNumberPlanManager();
            bsSNP.DataSource = sellingNumberPlanManager.GetSellingNumberPlans();
            cboSNP.DataSource = bsSNP;

            Vanrise.Common.Business.CurrencyManager currencyManager = new Vanrise.Common.Business.CurrencyManager();
            bsCurrency.DataSource = currencyManager.GetCachedCurrencies().Values;
            cboCurrency.DataSource = bsCurrency;


            Helper.NumberPlanId = (int)cboSNP.SelectedValue;
            Helper.CurrencyId = (int)cboCurrency.SelectedValue;

            Helper.MobileRuleDefinition.ChargingPolicyId = (int)numMobileChargingPolicy.Value;
            Helper.OffNetRuleDefinition.ChargingPolicyId = (int)numOffNetChargingPolicy.Value;
            Helper.OnNetRuleDefinition.ChargingPolicyId = (int)numOnNetChargingPolicy.Value;
            Helper.IntlRuleDefinition.ChargingPolicyId = (int)numInternationalChargingPolicy.Value;
            Helper.LocalRuleDefinition.ChargingPolicyId = (int)numLocalChargingPolicy.Value;
            Helper.CurrencyId = (int)cboCurrency.SelectedValue;
        }
        private void btnImportOutGoingRates_Click(object sender, EventArgs e)
        {
            btnImportOutGoingRates.Enabled = false;
            List<RuleDefinitionDetails> ruleDefinitions = GetIncomingRuleDefinitions((int)numOnNetChargingPolicy.Value, (int)numOffNetChargingPolicy.Value, (int)numMobileChargingPolicy.Value, (int)numInternationalChargingPolicy.Value);
            IncomingRatesMigrator migrator = new IncomingRatesMigrator();
            migrator.Ececute();
            MessageBox.Show("Importing Outgoing Rates rates is done");
            btnImportOutGoingRates.Enabled = true;

        }
        private void btnImportIntlRates_Click(object sender, EventArgs e)
        {
            Vanrise.Security.Entities.ContextFactory.GetContext().SetContextUserId(-1);
            btnImportIntlRates.Enabled = false;
            InternationalRatesMigrator migrator = new InternationalRatesMigrator(GetZones(), (int)numInternationalChargingPolicy.Value);
            migrator.MigrateInternationalRates();
            MessageBox.Show("Importing International rates is done");
            btnImportIntlRates.Enabled = true;
        }
        private void btnImpOffNetRates_Click(object sender, EventArgs e)
        {
            btnImpOffNetRates.Enabled = false;
            OffNetRatesMigrator migrator = new OffNetRatesMigrator();
            migrator.Execute();
            btnImpOffNetRates.Enabled = true;
            MessageBox.Show("Importing OffNet rates is done");
        }
        private void btnImportOnNet_Click(object sender, EventArgs e)
        {
            btnImportOnNet.Enabled = false;
            OnNetRatesMigrator onNetMigrator = new OnNetRatesMigrator();
            onNetMigrator.Execute();
            LocalRatesMigrator localMigrator = new LocalRatesMigrator();
            localMigrator.Execute();
            MessageBox.Show("Importing OnNet rates is done");
            btnImportOnNet.Enabled = true;
        }
        private void btnMobileRates_Click(object sender, EventArgs e)
        {
            btnMobileRates.Enabled = false;
            MobileRatesMigrator migrator = new MobileRatesMigrator();
            migrator.Execute();
            btnMobileRates.Enabled = true;
            MessageBox.Show("Importing Mobile rates is done");
        }
        private void btnImportMonthlyCharges_Click(object sender, EventArgs e)
        {
            btnImportMonthlyCharges.Enabled = false;
            MonthlyChargesMigrator migrator = new MonthlyChargesMigrator();
            migrator.Execute();
            MessageBox.Show("Importing Monthly Charges is done");
            btnImportMonthlyCharges.Enabled = true;
        }                
        private void cboCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCurrency.SelectedValue != null)
                Helper.CurrencyId = (int)cboCurrency.SelectedValue;
        }
        private void cboSNP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSNP.SelectedValue != null)
                Helper.NumberPlanId = (int)cboSNP.SelectedValue;
        }        
        private void numMobileChargingPolicy_ValueChanged(object sender, EventArgs e)
        {
            Helper.MobileRuleDefinition.ChargingPolicyId = (int)numMobileChargingPolicy.Value;
        }
        private void numOffNetChargingPolicy_ValueChanged(object sender, EventArgs e)
        {
            Helper.OffNetRuleDefinition.ChargingPolicyId = (int)numOffNetChargingPolicy.Value;
        }
        private void numOnNetChargingPolicy_ValueChanged(object sender, EventArgs e)
        {
            Helper.OnNetRuleDefinition.ChargingPolicyId = (int)numOnNetChargingPolicy.Value;
        }
        private void numInternationalChargingPolicy_ValueChanged(object sender, EventArgs e)
        {
            Helper.IntlRuleDefinition.ChargingPolicyId = (int)numInternationalChargingPolicy.Value;
        }        
        private void numPricingUnit_ValueChanged(object sender, EventArgs e)
        {
            Helper.PricingUnit = (int)numPricingUnit.Value;
        }
        Dictionary<string, SaleZone> GetZones()
        {
            SaleZoneManager manager = new SaleZoneManager();
            IEnumerable<SaleZone> zones = manager.GetSaleZonesEffectiveAfter(Helper.NumberPlanId, DateTime.Now);
            Dictionary<string, SaleZone> zoneDic = new Dictionary<string, SaleZone>();
            foreach(var z in zones.OrderByDescending(itm => itm.BED))
            {
                if (!zoneDic.ContainsKey(z.Name))
                    zoneDic.Add(z.Name, z);
            }
            return zoneDic;
        }
        private List<RuleDefinitionDetails> GetIncomingRuleDefinitions(int onNetPolicy, int offNetPolicy, int mobilePolicy, int internationalPolicy)
        {
            List<RuleDefinitionDetails> ruleDefinitionDetails = new List<RuleDefinitionDetails>();
            ruleDefinitionDetails.Add(new RuleDefinitionDetails
            {
                Name = "Mobile",
                RateDefinitionId = new Guid("f2348c97-8a9c-48db-9135-b0f93d2ce664"),
                TariffDefinitionId = new Guid("8d7e87f1-c91c-4a60-b404-ad4b8b85fa14"),
                IsInternational = false,
                ServiceTypeId = Helper.MobileServiceTypeId,
                ChargingPolicyId = mobilePolicy
            });
            ruleDefinitionDetails.Add(new RuleDefinitionDetails
            {
                Name = "On-Net",
                RateDefinitionId = new Guid("c97f4161-0fd2-4d30-acc3-06df03bf6aad"),
                TariffDefinitionId = new Guid("800c58a6-c2b3-4d4d-9777-99907bb2a6f8"),
                IsInternational = false,
                ServiceTypeId = Helper.OnNetServiceTypeId,
                ChargingPolicyId = onNetPolicy
            });
            ruleDefinitionDetails.Add(new RuleDefinitionDetails
            {
                Name = "Off-Net",
                RateDefinitionId = new Guid("ff701206-e2f3-44e2-b941-6d5a848ea123"),
                TariffDefinitionId = new Guid("c795feec-ae04-4ad4-8f6b-e022c78070da"),
                IsInternational = false,
                ServiceTypeId = Helper.OffNetServiceTypeId,
                ChargingPolicyId = offNetPolicy
            });
            ruleDefinitionDetails.Add(new RuleDefinitionDetails
            {
                Name = "International",
                RateDefinitionId = new Guid("d9cd209f-346b-4460-99da-68e8ad37bbc1"),
                TariffDefinitionId = new Guid("ecfbb042-9b68-4c84-9ab9-1e01b50b92ac"),
                IsInternational = true,
                ServiceTypeId = Helper.InternationalServiceTypeId,
                ChargingPolicyId = internationalPolicy
            });

            return ruleDefinitionDetails;

        }

    }
}
