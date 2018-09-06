using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.Common
{
    public static class MicsMockDataGenerator
    {
        public static List<MIC> GetAllMics()
        {
            return new List<MIC>
            {
                new MIC
                {
                    SwitchName = "Midan001.SW"
                },
                new MIC
                {
                    SwitchName = "Mazzeh001.SW"
                },
                new MIC
                {
                    SwitchName = "Damascus001.SW"
                }
            };
        }
    }
}
