
app.service('VRCommon_DataSourceImportedBatchService', ['VRCommon_MasterLogService','VR_Integration_DataSourceImportedBatchAPIService',
    function (VRCommon_MasterLogService, VR_Integration_DataSourceImportedBatchAPIService) {
        var drillDownDefinitions = [];
        return ({           
            registerLogToMaster: registerLogToMaster
        });
       

        function registerLogToMaster() {
            var tabDefinition = {
                title: "Imported Batch",
                rank: 3,
                hide:true,
                directive: "vr-integration-importedbatch-search",
                hasPermission: function (){
                    return VR_Integration_DataSourceImportedBatchAPIService.HasViewImportedBatchesPermission();
                },
                loadDirective: function (directiveAPI) {
                    return directiveAPI.load();
                }

            };

            VRCommon_MasterLogService.addTabDefinition(tabDefinition);
        }

 }]);
