using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            SwitchMappingRulesMigrator.Migrate(CBCurrentSwitches.SelectedValue.ToString(), "Teless",
               dateTimePicker.Value);
        }

    }
}
