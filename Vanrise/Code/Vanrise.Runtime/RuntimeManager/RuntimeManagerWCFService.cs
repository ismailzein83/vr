using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime
{
   internal class RuntimeManagerWCFService : IRuntimeManagerWCFService
    {
        public HeartBeatResponse UpdateHeartBeat(HeartBeatRequest request)
        {
            HeartBeatResponse response = new HeartBeatResponse();
            if (ProcessHeartBeatManager.Current.UpdateProcessHB(request.RunningProcessId))
                response.Result = HeartBeatResult.Succeeded;
            else
                response.Result = HeartBeatResult.ProcessNotExists;
            //Console.WriteLine("{0: HH:mm:ss} heartbeat received from process {1}. Result is {2}", DateTime.Now, request.RunningProcessId, response.Result);
            return response;
        }
    }
}
