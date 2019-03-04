using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.DataParser.Business;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using System.Collections;
using Mediation.Runtime.DataParser;

namespace Mediation.Runtime.Tasks
{
    public class FatimaAlHeckTask : ITask
    {
        public void Execute()
        {
            #region DataParserTesterTask
            DataParserTesterTask.DataParserTesterTask_Main();
            #endregion
        }
    }
}