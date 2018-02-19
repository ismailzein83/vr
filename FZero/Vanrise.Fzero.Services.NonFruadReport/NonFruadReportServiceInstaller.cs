using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Vanrise.Fzero.Services.NonFruadReport
{
    [RunInstaller(true)]
    public partial class NonFruadReportServiceInstaller : System.Configuration.Install.Installer
    {
        public NonFruadReportServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
