using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class ProcessStateManager
    {
        #region Public Methods

        public T GetProcessStateSetting<T>(string processStateUniqueName) where T : ProcessStateSettings
        {
            if (string.IsNullOrEmpty(processStateUniqueName))
                throw new ArgumentNullException("processStateUniqueName");

            var processState = GetProcessStates().GetRecord(processStateUniqueName);
            if (processState == null)
                return null;

            return processState.Settings.CastWithValidate<T>("processState.Settings", processStateUniqueName);
        }

        public bool InsertOrUpdate(string processStateUniqueName, ProcessStateSettings processState)
        {
            if (string.IsNullOrEmpty(processStateUniqueName))
                throw new ArgumentNullException("processStateUniqueName");

            IProcessStateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IProcessStateDataManager>();
            return dataManager.InsertOrUpdate(processStateUniqueName, processState);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, ProcessState> GetProcessStates()
        {
            IProcessStateDataManager dataManager = CommonDataManagerFactory.GetDataManager<IProcessStateDataManager>();
            IEnumerable<ProcessState> processStates = dataManager.GetProcessStates();
            return processStates.ToDictionary(item => item.UniqueName, item => item);
        }
        #endregion
    }
}