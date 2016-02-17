using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class OperatorDeclaredInformation
    {
        public int OperatorDeclaredInformationId { get; set; }

        public int OperatorId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public long? ZoneId { get; set; }

        public int Volume { get; set; }

        public int AmountType { get; set; }
    }
   
}
