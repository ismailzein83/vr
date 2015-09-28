﻿using System;

namespace PSTN.BusinessEntity.Entities
{
    public class SwitchDetail
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int TypeID { get; set; }

        public string TypeName { get; set; }

        public string AreaCode { get; set; }

        public TimeSpan TimeOffset { get; set; }

        public int? DataSourceID { get; set; }
    }
}
