
app.service('VRCommon_DataSourceImportedBatchService', ['VRCommon_MasterLogService',
    function (VRCommon_MasterLogService) {
        var drillDownDefinitions = [];
        return ({           
            registerLogToMaster: registerLogToMaster
        });
       

        function registerLogToMaster() {
            var tabDefinition = {
                title: "Imported Batch",
                hide:true,
                directive: "vr-integration-importedbatch-search",
                loadDirective: function (directiveAPI) {
                    return directiveAPI.load();
                }

            };

            VRCommon_MasterLogService.addTabDefinition(tabDefinition);
        }

 }]);
