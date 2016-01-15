using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrierPortal.DevRuntime.Tasks
{
    public class ZeinabTask : ITask
    {
        private readonly UploadPriceList _upload = new UploadPriceList();
        public void Execute()
        {
            _upload.Start(new MainForm());
        }
    }
}
