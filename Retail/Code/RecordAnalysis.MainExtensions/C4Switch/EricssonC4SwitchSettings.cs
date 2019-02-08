using System;
using RecordAnalysis.Entities;

namespace RecordAnalysis.MainExtensions.C4Switch
{
    public class EricssonC4SwitchSettings : C4SwitchSettings
    {
        public override Guid ConfigId { get { return new Guid("821F06BA-32D8-4B2A-8681-B688CE4FEE93"); } }

        public string Host { get; set; }
    }
}
