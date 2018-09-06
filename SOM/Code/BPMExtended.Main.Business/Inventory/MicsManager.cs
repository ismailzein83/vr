using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.Business
{
    public class MicsManager
    {
        public List<MIC> GetMics(string phoneNumber)
        {
            //TODO: get mics list
            return MicsMockDataGenerator.GetAllMics();
        }
        public MIC GetNextMic(string SwitchName)
        {
            //TODO: get next mic
            MIC currentmic = MicsMockDataGenerator.GetAllMics().Where(x => x.SwitchName == SwitchName).FirstOrDefault();
            MIC nextmic = MicsMockDataGenerator.GetAllMics()[currentmic.MicNumber];
            return nextmic== null ? new MIC() : nextmic ;
        }
    }
}
