(function (appControllers) {
    "use strict";
    bulkAddRuntimEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService','VR_GenericData_GenericBusinessEntityAPIService'];

    function bulkAddRuntimEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService) {

        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();
        $scope.scopeModel = {};

        var dataRecordTypeId;
        var parentFieldValues;
        var customAction;
        var customActionSettings;
        var businessEntityDefinitionId;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope); 
            if (parameters != undefined && parameters != null) {
                dataRecordTypeId = parameters.dataRecordTypeId;
                parentFieldValues = parameters.parentFieldValues;
                customAction = parameters.customAction;
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
            }
            customActionSettings = customAction.Settings != undefined ? customAction.Settings : {}; 
        
        }

        function defineScope() {
            $scope.scopeModel.execute = function () {
                $scope.scopeModel.isLoading = true;
               
                VR_GenericData_GenericBusinessEntityAPIService.ExecuteRangeGenericEditorProcess(buildExecuteProcessInputFromScope()).then(function (response) {
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                        $scope.modalContext.closeModal();
                });
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
            };
        }
        function loadEditorRuntimeDirective() {
            var loadEditorRuntimeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            runtimeEditorReadyDeferred.promise.then(function () {
                var runtimeEditorPayload = {
                    parentFieldValues: parentFieldValues,
                    dataRecordTypeId: dataRecordTypeId,
                    definitionSettings: customActionSettings.Settings,
                    runtimeEditor:customActionSettings.Settings != undefined ? customActionSettings.Settings.RuntimeEditor : undefined,
                    isEditMode:false
                };
                VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, loadEditorRuntimeDirectivePromiseDeferred);
            });
            return loadEditorRuntimeDirectivePromiseDeferred.promise;
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls();
        }

        function loadAllControls() {

                $scope.title = "Add Range";

            return UtilsService.waitPromiseNode({ promises: [loadEditorRuntimeDirective()]}).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildExecuteProcessInputFromScope() {
            var fieldValues = {};
            runtimeEditorAPI.setData(fieldValues); 
          
            for (var parentFieldValue in parentFieldValues) {
                fieldValues[parentFieldValue] = parentFieldValues[parentFieldValue].value;
            }
            var rangeVariableName = customActionSettings.RangeVariableName;
            if (rangeVariableName == undefined || rangeVariableName=='')
                rangeVariableName = "GeneratedRangeInfo";

            var numberRangeFieldValue = fieldValues[rangeVariableName];
            delete fieldValues[rangeVariableName];
            return {
                BusinessEntityDefinitionId: businessEntityDefinitionId,
                RangeFieldName: customActionSettings.RangeFieldName,
                FieldValues: fieldValues,
                Settings: numberRangeFieldValue
            };
        }

    }
    appControllers.controller('VR_GenericData_BulkAddRuntimeEditorController', bulkAddRuntimEditorController);
})(appControllers);