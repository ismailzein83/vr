
app.service('VRCommon_DataSourceLogService', ['VRCommon_MasterLogService',
    function (VRCommon_MasterLogService) {
        var drillDownDefinitions = [];
        return ({           
            registerLogToMaster: registerLogToMaster
        });
       

        function registerLogToMaster() {
            var tabDefinition = {
                title: "Data Source",
                position: 2,
                directive: "vr-integration-log-search",
                loadDirective: function (directiveAPI) {
                    return directiveAPI.load();
                }

            };

            VRCommon_MasterLogService.addTabDefinition(tabDefinition);
        }

 }]);
