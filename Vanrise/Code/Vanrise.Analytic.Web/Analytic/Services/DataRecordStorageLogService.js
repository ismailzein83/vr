(function (appControllers) {

    "use strict";

    DataRecordStorageLogService.$inject = ['VR_Analytic_DataAnalysisItemDefinitionService', 'VRModalService', 'VRUIUtilsService', 'VRCommon_ObjectTrackingService'];

    function DataRecordStorageLogService(VR_Analytic_DataAnalysisItemDefinitionService, VRModalService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

        function defineDataRecordStorageLogTabs(dataRecordStorageLog, subviewDefinitions, parentSearchQuery, gridAPI) {

            var drillDownTabs = [];

            if (dataRecordStorageLog.details != undefined && dataRecordStorageLog.details.length > 0) {
                buildDetailsDrillDownTab();
            }

            if (subviewDefinitions != undefined) {
                for (var i = 0; i < subviewDefinitions.length; i++) {
                    var subviewDefinition = subviewDefinitions[i];

                    addDrillDownTab(dataRecordStorageLog, subviewDefinition);
                }
            }

            setDrillDownTabs();


            function buildDetailsDrillDownTab() {
                var detailsTab = {};
                detailsTab.title = 'Details';
                detailsTab.directive = 'vr-analytic-datarecordsearchpage-itemdetails';

                detailsTab.loadDirective = function (directiveAPI, dataRecordStorageLog) {
                    dataRecordStorageLog.directiveAPI = directiveAPI;
                    var payload = { dataRecordStorageLog: dataRecordStorageLog };
                    return dataRecordStorageLog.directiveAPI.load(payload);
                };

                drillDownTabs.push(detailsTab);
            }

            function addDrillDownTab(dataRecordStorageLog, subviewDefinition) {
                var drillDownTab = {};
                drillDownTab.title = subviewDefinition.Name;
                drillDownTab.directive = subviewDefinition.Settings.RuntimeEditor;

                drillDownTab.loadDirective = function (directiveAPI, dataRecordStorageLog) {
                    dataRecordStorageLog.dataRecordStorageLogSubviewGridAPI = directiveAPI;

                    var payload = {
                        subviewDefinition: subviewDefinition,
                        parentSearchQuery: parentSearchQuery,
                        dataRecordStorageLog: dataRecordStorageLog
                    };

                    return dataRecordStorageLog.dataRecordStorageLogSubviewGridAPI.load(payload);
                };

                drillDownTabs.push(drillDownTab);
            }

            function setDrillDownTabs() {
                if (drillDownTabs.length == 0)
                    return;

                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
                drillDownManager.setDrillDownExtensionObject(dataRecordStorageLog);
            }
        }


        return {
            defineDataRecordStorageLogTabs: defineDataRecordStorageLogTabs
        };
    }

    appControllers.service('VR_Analytic_DataRecordStorageLogService', DataRecordStorageLogService);

})(appControllers);