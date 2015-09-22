using System;

namespace PSTN.BusinessEntity.Entities
{
    public class Switch
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int TypeID { get; set; }

        public string AreaCode { get; set; }

        public string TimeOffset { get; set; }

        public int DataSourceID { get; set; }
    }
}
