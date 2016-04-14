app.run(['VRCommon_DataSourceLogService', 'VRCommon_DataSourceImportedBatchService', function (VRCommon_DataSourceLogService, VRCommon_DataSourceImportedBatchService) {
    VRCommon_DataSourceLogService.registerLogToMaster();
    VRCommon_DataSourceImportedBatchService.registerLogToMaster();
}]);