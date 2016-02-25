using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class OperatorDeclaredInformation
    {
        public int OperatorDeclaredInformationId { get; set; }

        public int OperatorId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? DestinationGroup { get; set; }

        public int Volume { get; set; }

        public int AmountType { get; set; }

        public string Notes { get; set; }

        public long? Attachment { get; set; }

    }
   
}
