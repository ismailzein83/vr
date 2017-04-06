app.run(['VRCommon_DataSourceLogService', 'VRCommon_DataSourceImportedBatchService', 'VR_Integration_DataSourceService', function (VRCommon_DataSourceLogService, VRCommon_DataSourceImportedBatchService, VR_Integration_DataSourceService) {
    VRCommon_DataSourceLogService.registerLogToMaster();
    VRCommon_DataSourceImportedBatchService.registerLogToMaster();
    VR_Integration_DataSourceService.registerObjectTrackingDrillDownToDataSource();
    VR_Integration_DataSourceService.registerHistoryViewAction();
}]);