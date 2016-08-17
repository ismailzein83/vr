
app.service('VRCommon_DataSourceLogService', ['VRCommon_MasterLogService','VR_Integration_DataSourceLogAPIService',
    function (VRCommon_MasterLogService, VR_Integration_DataSourceLogAPIService) {
        var drillDownDefinitions = [];
        return ({           
            registerLogToMaster: registerLogToMaster
        });
       

        function registerLogToMaster() {
            var tabDefinition = {
                title: "Data Source",
                rank: 2,
                directive: "vr-integration-log-search",
                hasPermission: function () {
                    return VR_Integration_DataSourceLogAPIService.HasViewFilteredDataSourcePermission();
                },
                loadDirective: function (directiveAPI) {
                    return directiveAPI.load();
                }

            };

            VRCommon_MasterLogService.addTabDefinition(tabDefinition);
        }

 }]);
