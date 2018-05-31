using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Vanrise.Fzero.Services.DailyReportsSummary
{
    [RunInstaller(true)]
    public partial class DaileReportsSummaryServiceInstaller : System.Configuration.Install.Installer
    {
        public DaileReportsSummaryServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
