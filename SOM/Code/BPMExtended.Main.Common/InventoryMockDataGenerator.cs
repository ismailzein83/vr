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

        public static List<CPTNumberDetail> GetCPTNumbersByStatus(CPTNumberStatus status)
        {
            return GetAllCPTNumbers().FindAll(x => x.Status == status).ToList();
        }

        private static List<CPTNumberDetail> GetAllCPTNumbers()
        {
            return new List<CPTNumberDetail>
            {
                new CPTNumberDetail
                {
                    Id = "1",
                    Number = "1515",
                    Status = CPTNumberStatus.Free
                },
                new CPTNumberDetail
                {
                    Id = "2",
                    Number = "1517",
                    Status = CPTNumberStatus.Free
                },
                new CPTNumberDetail
                {
                    Id = "3",
                    Number = "1518",
                    Status = CPTNumberStatus.Free
                },
                new CPTNumberDetail
                {
                    Id = "4",
                    Number = "1212",
                    Status = CPTNumberStatus.Reserved
                },
                new CPTNumberDetail
                {
                    Id = "5",
                    Number = "1240",
                    Status = CPTNumberStatus.Reserved
                }
            };
        }

        #endregion
    }
}
