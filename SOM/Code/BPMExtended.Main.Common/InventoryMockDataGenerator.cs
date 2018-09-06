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

        #region DSLAM Free Ports

        public static List<DSLAMPortInfo> GetDSLAMFreePorts()
        {
            return new List<DSLAMPortInfo>
            {
                new DSLAMPortInfo
                {
                    Id="1001",
                    Name = "P-1515"
                },
                new DSLAMPortInfo
                {
                    Id="1002",
                    Name = "P-1517"
                },
                new DSLAMPortInfo
                {
                    Id="1003",
                    Name = "P-1518"
                },
                new DSLAMPortInfo
                {
                    Id="1004",
                    Name = "P-1212"
                },
                new DSLAMPortInfo
                {
                    Id="1005",
                    Name = "P-1240"
                }
            };
        }

        #endregion
    }
}
