using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TOne.RuntimeService
{
    public partial class TOneRuntimeService : ServiceBase
    {
        public TOneRuntimeService()
        {
            InitializeComponent();
        }

        TOne.Runtime.MainService _mainService;

        public void Start()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            _mainService = new Runtime.MainService();
            _mainService.Start();
        }

        protected override void OnStop()
        {
            if (_mainService != null)
                _mainService.Stop();
        }
    }
}
