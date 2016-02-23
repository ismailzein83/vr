using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class OperatorConfiguration
    {
        public int OperatorConfigurationId { get; set; }

        public int OperatorId { get; set; }

        public int Volume { get; set; }

        public int AmountType { get; set; }

        public CDRDirection CDRDirection { get; set; }

        public double? Percentage { get; set; }

        public double? Amount { get; set; }

        public int? Currency { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string Notes { get; set; }

        public ServiceSubType ServiceSubTypeSettings { get; set; }

    }

}
