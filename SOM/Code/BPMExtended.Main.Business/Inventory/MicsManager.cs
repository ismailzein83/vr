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
        public MIC GetNextMic(string switchName, string phoneNumber)
        {
            //TODO: get next mic
            List<MIC> allmics = GetMics(phoneNumber);
            MIC currentmic = allmics.Where(x => x.SwitchName == switchName).FirstOrDefault();
            MIC nextmic = null;
            try
            {
                nextmic = allmics[currentmic.MicNumber];
            }
            catch {
            }
            return  nextmic ;
        }
        public MIC GetFirstMic(string phoneNumber)
        {
            //TODO: get next mic
            MIC currentmic = null; 
            try
            {
                currentmic = GetMics(phoneNumber).First();
            }
            catch
            {
            }
            return currentmic;
        }
    }
}
