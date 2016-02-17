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

        public OperatorDeclaredInformationSettings Settings { get; set;  }
    }
    public class OperatorDeclaredInformationSettings
    {
        public List<string> Range { get; set; }

    }
}
