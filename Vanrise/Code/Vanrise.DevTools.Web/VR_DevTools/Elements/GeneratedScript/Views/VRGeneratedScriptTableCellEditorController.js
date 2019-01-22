(appControllers); (function (appControllers) {
    "use strict";
    generatedScriptTableCellEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService','VR_Devtools_GeneratedScriptFieldValueTypeEnum',];

    function generatedScriptTableCellEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Devtools_GeneratedScriptFieldValueTypeEnum) {

        $scope.scopeModel = {};
        $scope.scopeModel.variables = [];
        var cellValue;
        var fieldValueTypeDirectiveAPI;
        var fieldValueTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var fieldValueTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
        var variablesDirectiveAPI;
        var variablesReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                cellValue = parameters.cellValue;
            }
        }

        function defineScope() {
            

            $scope.scopeModel.saveTableCell = function () {
                if ($scope.scopeModel.isFieldTypeText())
                    $scope.modifySelectedTableData($scope.scopeModel.cellValue);
                else $scope.modifySelectedTableData({
                    $type:"Vanrise.DevTools.Entities.GeneratedScriptVariableData,Vanrise.DevTools.Entities",
                    VariableId: $scope.scopeModel.selectedVariable.Id,
                    IsVariable:true
                });
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onFieldValueTypeSelectorReady = function (api) {
                fieldValueTypeDirectiveAPI = api;
                fieldValueTypeReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.isFieldTypeText = function () {
                if (fieldValueTypeDirectiveAPI != undefined && fieldValueTypeDirectiveAPI.getSelectedIds() == VR_Devtools_GeneratedScriptFieldValueTypeEnum.Text.value) 
                    return true;
                return false;
            };
            $scope.scopeModel.onVariablesSelectorReady = function (api) {
                variablesReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onFieldValueTypeChanged = function (value) {
                if (fieldValueTypeDirectiveAPI) {
                    if (value) {
                        if (fieldValueTypeSelectedPromiseDeferred != undefined) {
                            fieldValueTypeSelectedPromiseDeferred.resolve();
                            fieldValueTypeSelectedPromiseDeferred = undefined;
                        }
                        else {
                            if (!$scope.scopeModel.isFieldTypeText()) {
                                $scope.scopeModel.variables = $scope.getVariables();
                                $scope.scopeModel.cellValue = undefined;
                            }
                            else $scope.scopeModel.selectedVariable=undefined;
                        }
                    }
                }
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            
            loadAllControls().finally(function () {
            });

        }
       
        function loadAllControls() {
            function loadStaticData() {
                if (cellValue != undefined && typeof (cellValue) == "object") {
                    if (cellValue.IsVariable) {
                        variablesReadyPromiseDeferred.promise.then(function () {
                            $scope.scopeModel.variables = $scope.getVariables();
                            for (var i = 0; i < $scope.scopeModel.variables.length; i++) {
                                var variable = $scope.scopeModel.variables[i];
                                if (cellValue.VariableId == variable.Id) {
                                    $scope.scopeModel.selectedVariable = variable; break;
                                }
                            }
                        });
                    }
                    else $scope.scopeModel.cellValue = JSON.stringify(cellValue);
                }
                else 
                    $scope.scopeModel.cellValue = cellValue;
            }

            function loadFieldValueTypeDirective() {
                var fieldValueTypeLoadDeferred = UtilsService.createPromiseDeferred();
                fieldValueTypeReadyPromiseDeferred.promise.then(function (response) {
                    var payload = {
                        selectedIds: (cellValue != undefined && typeof (cellValue) == "object" && cellValue.IsVariable) ? VR_Devtools_GeneratedScriptFieldValueTypeEnum.Variable.value
                            : VR_Devtools_GeneratedScriptFieldValueTypeEnum.Text.value
                    };
                    VRUIUtilsService.callDirectiveLoad(fieldValueTypeDirectiveAPI, payload, fieldValueTypeLoadDeferred);
                });
                return fieldValueTypeLoadDeferred.promise;
            }
            function setTitle() {
                $scope.title = "Edit Table Cell";

            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFieldValueTypeDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }


    }
    appControllers.controller('VR_Devtools_GeneratedScriptTableCellEditorController', generatedScriptTableCellEditorController);
})(appControllers);