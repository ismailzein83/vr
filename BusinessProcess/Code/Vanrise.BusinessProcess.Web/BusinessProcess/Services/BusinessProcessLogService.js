
app.service('BusinessProcess_BusinessProcessLogService', ['VRCommon_MasterLogService', 'BusinessProcess_BPInstanceAPIService',
    function (VRCommon_MasterLogService, BusinessProcess_BPInstanceAPIService) {
        return ({           
            registerLogToMaster: registerLogToMaster
        });

        function registerLogToMaster() {
            var promise =  BusinessProcess_BPInstanceAPIService.HasViewFilteredBPInstancesPermission().then(function (response) {
                if (response == true) {
                    var tabDefinition = {
                        title: "Business Process",
                        directive: "bp-instance-log-search",
                        hide: true,
                        loadDirective: function (directiveAPI) {
                            return directiveAPI.load();
                        }
                    }
                    VRCommon_MasterLogService.addTabDefinition(tabDefinition);
                }

            });

            VRCommon_MasterLogService.addTabPromise(promise);
               
        }

 }]);
