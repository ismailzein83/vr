
app.service('BusinessProcess_BusinessProcessLogService', ['VRCommon_MasterLogService', 'BusinessProcess_BPInstanceAPIService',
    function (VRCommon_MasterLogService, BusinessProcess_BPInstanceAPIService) {
        return ({           
            registerLogToMaster: registerLogToMaster
        });

        function registerLogToMaster() {
            
            var tabDefinition = {
                title: "Business Process",
                rank: 4,
                directive: "bp-instance-log-search",
                hasPermission: function () {
                    return BusinessProcess_BPInstanceAPIService.HasViewFilteredBPInstancesPermission()

                },
                loadDirective: function (directiveAPI) {
                    return directiveAPI.load();
                }
            };
            VRCommon_MasterLogService.addTabDefinition(tabDefinition);
        }

 }]);
