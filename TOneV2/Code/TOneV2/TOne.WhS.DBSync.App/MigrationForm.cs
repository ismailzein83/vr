using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TOne.WhS.DBSync.Business;
using TOne.WhS.DBSync.Business.SwitchMigration;

namespace TOne.WhS.DBSync.App
{
    public partial class MigrationForm : Form
    {
        private SwitchMappingRulesMigrator SwitchMappingRulesMigrator { get; set; }

        public MigrationForm()
        {
            InitializeComponent();
            SwitchMappingRulesMigrator = new SwitchMappingRulesMigrator(Program.ConnectionString);
            PopulateSwitchCombo();
            PopulateParserCb();
        }

        private void PopulateParserCb()
        {
            var allImplementation = Vanrise.Common.Utilities.GetAllImplementations<ISwitchParser>();
            CBParser.DataSource = allImplementation;
            CBParser.DisplayMember = "Name";
            CBParser.ValueMember = "FullName";
        }
        private void PopulateSwitchCombo()
        {
            var switches = SwitchMappingRulesMigrator.LoadSwitches();
            CBCurrentSwitches.DisplayMember = "Name";
            CBCurrentSwitches.ValueMember = "Id";
            CBCurrentSwitches.DataSource = switches;
        }
        private void btnMigrate_Click(object sender, EventArgs e)
        {
            SwitchMappingRulesMigrator.Migrate(CBCurrentSwitches.SelectedValue.ToString(), CBParser.SelectedValue.ToString(),
               dateTimePicker.Value.Date);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Inserted {0} new Customer Mapping Rules",
                SwitchMappingRulesMigrator.Logger.InParsedMappingSuccededCount));
            if (SwitchMappingRulesMigrator.Logger.InParsedMappingFailedCount > 0)
                sb.AppendLine((String.Format("Failed to insert {0} Customer Mapping Rules",
                    SwitchMappingRulesMigrator.Logger.InParsedMappingFailedCount)));
            sb.AppendLine(string.Format("Inserted {0} new Supplier Mapping Rules",
                SwitchMappingRulesMigrator.Logger.OutParsedMappingSuccededCount));
            if (SwitchMappingRulesMigrator.Logger.OutParsedMappingFailedCount > 0)
                sb.AppendLine((String.Format("Failed to insert {0} Supplier Mapping Rules",
                    SwitchMappingRulesMigrator.Logger.OutParsedMappingFailedCount)));
            if (!string.IsNullOrEmpty(SwitchMappingRulesMigrator.Logger.WarningMessage.ToString()))
                sb.AppendLine(SwitchMappingRulesMigrator.Logger.WarningMessage.ToString());
            if (!string.IsNullOrEmpty(SwitchMappingRulesMigrator.Logger.InfoMessage.ToString()))
                sb.AppendLine(SwitchMappingRulesMigrator.Logger.InfoMessage.ToString());
            LblInfo.Text = sb.ToString();
        }
    }
}
