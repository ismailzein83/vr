﻿using System;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public class AdapterImportDataContext : IAdapterImportDataContext
    {
        DataSource _dataSource;
        Func<IImportedData, ImportedBatchProcessingOutput> _onDataReceivedAction;

        DataSourceStateManager _dsStateManager = new DataSourceStateManager();
        DataSourceRuntimeInstanceManager _dsRuntimeInstanceManager = new DataSourceRuntimeInstanceManager();

        public AdapterImportDataContext(DataSource dataSource, Func<IImportedData, ImportedBatchProcessingOutput> onDataReceivedAction)
        {
            _dataSource = dataSource;
            _onDataReceivedAction = onDataReceivedAction;
        }

        public Guid DataSourceId
        {
            get { return _dataSource.DataSourceId; }
        }

        public BaseAdapterArgument AdapterArgument
        {
            get { return _dataSource.Settings.AdapterArgument; }
        }

        public void GetStateWithLock(Func<BaseAdapterState, BaseAdapterState> onStateReady)
        {
            _dsStateManager.GetStateWithLock(this.DataSourceId, onStateReady);
        }

        public ImportedBatchProcessingOutput OnDataReceived(IImportedData data)
        {
            return _onDataReceivedAction(data);
        }

        public void StartNewInstanceIfAllowed()
        {
            _dsRuntimeInstanceManager.TryAddNewInstance(this.DataSourceId, this.AdapterArgument.MaxParallelRuntimeInstances);
        }

    }
}
