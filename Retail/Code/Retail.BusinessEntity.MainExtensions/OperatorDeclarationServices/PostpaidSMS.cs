using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.OperatorDeclarationServices
{
    public class PostpaidSMS : OperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("0F35BD74-81D4-4CF3-950D-98DE8CDAD7D9"); }
        }

        public TrafficType TrafficType { get; set; }

        public TrafficDirection TrafficDirection { get; set; }

        public long NumberOfSMSs { get; set; }
    }
}
