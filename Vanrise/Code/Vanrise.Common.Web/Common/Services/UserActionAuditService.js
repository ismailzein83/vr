
app.service('VRCommon_UserActionAuditService', ['VRCommon_MasterLogService', 'VRCommon_UserActionAuditAPIService', 
    function (VRCommon_MasterLogService, VRCommon_UserActionAuditAPIService) {
        var drillDownDefinitions = [];
        return ({           
            registerLogToMaster: registerLogToMaster
        });

        function registerLogToMaster() {
               
            var tabDefinition = {
                title: "User Action Audit",
                rank: 5,
                directive: "vr-useractionaudit-search",
                hasPermission: function () {
                    return VRCommon_UserActionAuditAPIService.HasViewUserActionAuditsPermission();
                },
                loadDirective: function (directiveAPI) {
                    return directiveAPI.load();
                }
            };
            VRCommon_MasterLogService.addTabDefinition(tabDefinition);
        }
 }]);
