using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.BPInstanceProgressViews
{
    public class SimilarProcessesProgressView : BPInstanceProgressViewSettings
    {
        public override string Editor
        {
            get { throw new NotImplementedException(); }
        }

        public override bool LoadOnDemand
        {
            get { return true; }
        }
    }
}
