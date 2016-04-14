
app.service('VRCommon_LogEntryService', ['VRCommon_MasterLogService',
    function (VRCommon_MasterLogService) {
        var drillDownDefinitions = [];
        return ({           
            registerLogToMaster: registerLogToMaster
        });

        function registerLogToMaster() {
                var tabDefinition = {
                    title: "Log Entry",
                    directive: "vr-log-entry-search",
                    hide: true,
                    loadDirective: function (directiveAPI) {
                        return directiveAPI.load();
                    }
                }

                VRCommon_MasterLogService.addTabDefinition(tabDefinition);
        }

 }]);
