using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class LineBlockingFlowManagement
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        const string welcomeStep = "2ED78E9D-7567-41D0-BB49-099A698830EA";
        const string attachmentstep = "AE91CDB4-459B-4059-A238-74FC00587A7F";
        const string completedStep = "F8545214-789B-4624-B528-46FBB4C27542";
        public string GetNextStep(string id, string currentStepId)
        {

            string nextStepId = "";
            switch (currentStepId)
            {
                case welcomeStep: nextStepId = attachmentstep; break;
                case attachmentstep: nextStepId = completedStep; break;
            }
            return nextStepId;
        }
    }
}
