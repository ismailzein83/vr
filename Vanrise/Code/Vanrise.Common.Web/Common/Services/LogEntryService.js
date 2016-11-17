
app.service('VRCommon_LogEntryService', ['VRCommon_MasterLogService','VRCommon_LogEntryAPIService','LabelColorsEnum',
    function (VRCommon_MasterLogService, VRCommon_LogEntryAPIService, LabelColorsEnum) {
        var drillDownDefinitions = [];
        return ({           
            registerLogToMaster: registerLogToMaster,
            getTypeColor: getTypeColor
        });

        function registerLogToMaster() {
               
            var tabDefinition = {
                title: "General",
                rank: 1,
                directive: "vr-log-entry-search",
                hide: true,
                hasPermission: function () {
                    return VRCommon_LogEntryAPIService.HasViewSystemLogPermission();
                },
                loadDirective: function (directiveAPI) {
                    return directiveAPI.load();
                }
            };
                VRCommon_MasterLogService.addTabDefinition(tabDefinition);
        }

        function getTypeColor(type) {
            if (type === 1 ) return LabelColorsEnum.Error.color;
            if (type === 2) return LabelColorsEnum.Warning.color;
            if (type === 4) return LabelColorsEnum.Info.color;
            if (type === 8) return LabelColorsEnum.Primary.color;
            return LabelColorsEnum.Info.color;
        };

 }]);
