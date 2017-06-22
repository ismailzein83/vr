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
        }

        private void btnImportOutGoingRates_Click(object sender, EventArgs e)
        {
            List<RuleDefinitionDetails> ruleDefinitions = GetIncomingRuleDefinitions((int)numOnNetChargingPolicy.Value, (int)numOffNetChargingPolicy.Value, (int)numMobileChargingPolicy.Value, (int)numInternationalChargingPolicy.Value);
            IncomingRatesMigrator migrator = new IncomingRatesMigrator(ruleDefinitions);
            migrator.MigrateIncomingRates(new Guid("9a427357-cf55-4f33-99f7-745206dee7cd"), new Guid("5ff96aee-cdf0-4415-a643-6b275f47e791"));
        }

        Dictionary<string, SaleZone> GetZones()
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZonesEffectiveAfter(Helper.NumberPlanId, DateTime.Now).ToDictionary(z => z.Name, z => z);
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

        private List<RuleDefinitionDetails> GetInternationalRuleDefinitions(int internationalPolicy)
        {
            List<RuleDefinitionDetails> ruleDefinitionDetails = new List<RuleDefinitionDetails>();

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

        private void btnImportIntlRates_Click(object sender, EventArgs e)
        {
            var ruleDefinitionDetails = GetInternationalRuleDefinitions((int)numInternationalChargingPolicy.Value);
            InternationalRatesMigrator migrator = new InternationalRatesMigrator(GetZones(), ruleDefinitionDetails);
            migrator.MigrateInternationalRates();
        }

        private void cboCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            Helper.CurrencyId = (int)cboCurrency.SelectedValue;
        }

        private void cboSNP_SelectedIndexChanged(object sender, EventArgs e)
        {
            Helper.NumberPlanId = (int)cboSNP.SelectedValue;
        }
    }
}
