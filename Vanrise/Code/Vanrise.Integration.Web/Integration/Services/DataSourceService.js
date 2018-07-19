(function (appControllers) {

    'use strict';

    DataSourceService.$inject = ['UtilsService', 'VRModalService', 'VR_Integration_ExecutionStatusEnum', 'VR_Integration_MappingResultEnum', 'LabelColorsEnum','VRNotificationService','VR_Integration_DataSourceAPIService','VRCommon_ObjectTrackingService'];

    function DataSourceService(UtilsService, VRModalService, VR_Integration_ExecutionStatusEnum, VR_Integration_MappingResultEnum, LabelColorsEnum, VRNotificationService, VR_Integration_DataSourceAPIService,VRCommon_ObjectTrackingService) {
        var drillDownDefinitions=[];
        return {
            
            getExecutionStatusDescription: getExecutionStatusDescription,
            getMappingResultDescription: getMappingResultDescription,
            getExecutionStatusColor: getExecutionStatusColor,
            editDataSource: editDataSource,
            addDataSource: addDataSource,
            deleteDataSource: deleteDataSource,
            registerObjectTrackingDrillDownToDataSource:registerObjectTrackingDrillDownToDataSource,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction,
            registerLogToDataSource: registerLogToDataSource,
            registerImportedBatchToDataSource: registerImportedBatchToDataSource

        };


        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "VR_Integration_DataSource_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryDataSource(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }

        function viewHistoryDataSource(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('Client/Modules/Integration/Views/DataSource/DataSourceEditor.html', modalParameters, modalSettings);
        };

        function getExecutionStatusDescription(executionStatusValue) {
            return UtilsService.getEnumDescription(VR_Integration_ExecutionStatusEnum, executionStatusValue);
        }

        function getMappingResultDescription(mappingResultValue) {
            return UtilsService.getEnumDescription(VR_Integration_MappingResultEnum, mappingResultValue);
        }

        function getExecutionStatusColor(executionStatusValue) {
            var color = undefined;

            switch (executionStatusValue) {
                case VR_Integration_ExecutionStatusEnum.New.value:
                    color = LabelColorsEnum.New.color;
                    break;
                case VR_Integration_ExecutionStatusEnum.Processing.value:
                    color = LabelColorsEnum.Processing.color;
                    break;
                case VR_Integration_ExecutionStatusEnum.Failed.value:
                    color = LabelColorsEnum.Failed.color;
                    break;
                case VR_Integration_ExecutionStatusEnum.Processed.value:
                    color = LabelColorsEnum.Processed.color;
                    break;
                case VR_Integration_ExecutionStatusEnum.NoBatches.value:
                    color = LabelColorsEnum.Default.color;
                    break;
            }

            return color;
        }

        function editDataSource(dataSourceId, onDataSourceUpdated) {
            var modalSettings = {
            };
            var parameters = {
                dataSourceId: dataSourceId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Edit Data Source";
                modalScope.onDataSourceUpdated = onDataSourceUpdated;
            };
            VRModalService.showModal('Client/Modules/Integration/Views/DataSource/DataSourceEditor.html', parameters, modalSettings);
        }

        function addDataSource(onDataSourceAdded) {
            var modalSettings = {
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Add a Data Source";
                modalScope.onDataSourceAdded = onDataSourceAdded;
            };

            VRModalService.showModal('Client/Modules/Integration/Views/DataSource/DataSourceEditor.html', null, modalSettings);
        }

        function deleteDataSource(scope, dataSourceObj, onDataSourceDeleted) {
            VRNotificationService.showConfirmation().then(function (response) {

                    if (response) {
                        return VR_Integration_DataSourceAPIService.DeleteDataSource(dataSourceObj.DataSourceId, dataSourceObj.TaskId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("DataSource", deletionResponse);
                                onDataSourceDeleted(dataSourceObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }

        function getEntityUniqueName()
        {
            return "VR_Integration_DataSource";
        }

        function registerObjectTrackingDrillDownToDataSource() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";
            

            drillDownDefinition.loadDirective = function (directiveAPI, dataSourceItem) {
                dataSourceItem.objectTrackingGridAPI = directiveAPI;
              
                var query = {
                    ObjectId: dataSourceItem.Entity.DataSourceId,
                    EntityUniqueName: getEntityUniqueName(),
                    
                };
                return dataSourceItem.objectTrackingGridAPI.load(query);
            };
            
            addDrillDownDefinition(drillDownDefinition);
           
        }

        function registerLogToDataSource() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Data Source Log";
            drillDownDefinition.directive = "vr-integration-log-search";


            drillDownDefinition.loadDirective = function (directiveAPI, dataSourceItem) {
                dataSourceItem.logPanelAPI = directiveAPI;

                var payload = {
                    dataSourceId: dataSourceItem.Entity.DataSourceId

                };
                return dataSourceItem.logPanelAPI.load(payload);
            };

            addDrillDownDefinition(drillDownDefinition);

        }

        function registerImportedBatchToDataSource() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Imported Batch";
            drillDownDefinition.directive = "vr-integration-importedbatch-search";


            drillDownDefinition.loadDirective = function (directiveAPI, dataSourceItem) {
                dataSourceItem.importedBatchPanelAPI = directiveAPI;

                var payload = {
                    dataSourceId: dataSourceItem.Entity.DataSourceId

                };
                return dataSourceItem.importedBatchPanelAPI.load(payload);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
          
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
   
    
    };

    appControllers.service('VR_Integration_DataSourceService', DataSourceService);

})(appControllers);
