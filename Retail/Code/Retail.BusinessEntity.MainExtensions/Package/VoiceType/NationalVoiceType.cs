using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.Package
{
    public enum NetworkType { OnNet = 0, OffNet = 1 }
    public class NationalVoiceType : VoiceType
    {
        public NetworkType NetworkType { get; set; }
    }
}
