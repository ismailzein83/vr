using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Common
{
    public static class InventoryMockDataGenerator
    {
        #region Constants

        #endregion

        #region CPT Numbers

        public static List<CPTNumberDetail> GetAllCPTNumbers()
        {
            return new List<CPTNumberDetail>
            {
                new CPTNumberDetail
                {
                    Number = "1515"
                },
                new CPTNumberDetail
                {
                    Number = "1517"
                },
                new CPTNumberDetail
                {
                    Number = "1518"
                },
                new CPTNumberDetail
                {
                    Number = "1212"
                },
                new CPTNumberDetail
                {
                    Number = "1240"
                }
            };
        }

        #endregion
    }
}
