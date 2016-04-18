
app.service('VRCommon_LogEntryService', ['VRCommon_MasterLogService','VRCommon_LogEntryAPIService',
    function (VRCommon_MasterLogService, VRCommon_LogEntryAPIService) {
        var drillDownDefinitions = [];
        return ({           
            registerLogToMaster: registerLogToMaster
        });

        function registerLogToMaster() {
            VRCommon_LogEntryAPIService.HasViewSystemLogPermission().then(function (response) {
                if (response == true) {
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

            });
               
        }

 }]);
