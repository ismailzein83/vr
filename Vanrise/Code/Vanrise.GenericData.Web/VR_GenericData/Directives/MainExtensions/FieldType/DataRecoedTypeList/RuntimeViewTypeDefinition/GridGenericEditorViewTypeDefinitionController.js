//(function (appControllers) {
//    "use strict";
//    gridViewEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

//    function gridViewEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

//        var isEditMode;

//        var editorDefinitionAPI;
//        var editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//        var columnSettingDirectiveAPI;
//        var columnSettingDirectivePromiseReadyDeferred = UtilsService.createPromiseDeferred();

//        $scope.scopeModel = {};

//        var title;
//        var dataRow;
//        var context;
//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope); 
//            if (parameters != undefined && parameters != null) {
//                context = parameters.context;
//                dataRow = parameters.dataRow;
//            }
//            if (dataRow != undefined) {
//                isEditMode = true;
//                title = dataRow.title;
//            }
//        }

//        function defineScope() {
//            $scope.scopeModel.saveDataRow = function () {

//                if (isEditMode)
//                    return updateDataRow();
//                else
//                    return insertDataRow();
//            };

//            $scope.scopeModel.close = function () {
//                $scope.modalContext.closeModal();
//            };

//            $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
//                editorDefinitionAPI = api;
//                editorDefinitionReadyPromiseDeferred.resolve();
//            };
//            $scope.scopeModel.onColumnSettingDirectiveReady = function (api) {
//                columnSettingDirectiveAPI = api;
//                columnSettingDirectivePromiseReadyDeferred.resolve();
//            };
//        }

//        function loadEditorDefinitionDirective() {
//            var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
//            editorDefinitionReadyPromiseDeferred.promise.then(function () {
//                var editorPayload = {
//                    settings: dataRow != undefined ? dataRow.Settings : undefined,
//                    context: context
//                };
//                VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, editorPayload, loadEditorDefinitionDirectivePromiseDeferred);
//            });
//            return loadEditorDefinitionDirectivePromiseDeferred.promise;
//        }

//        function loadColumnSettingDirective() {
//            var columnSettingDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
//            columnSettingDirectivePromiseReadyDeferred.promise.then(function () {
//                var columnSettingEditorPayload = {
//                    data: dataRow != undefined ? dataRow.GridColumnSettings : undefined
//                };
//                VRUIUtilsService.callDirectiveLoad(columnSettingDirectiveAPI, columnSettingEditorPayload, columnSettingDirectiveLoadPromiseDeferred);
//            });
//            return columnSettingDirectiveLoadPromiseDeferred.promise;
//        }
//        function load() {
//            $scope.scopeModel.isLoading = true;

//            if (isEditMode) {
//                loadAllControls().finally(function () {
//                });
//            }
//            else
//                loadAllControls();
//        }

//        function loadAllControls() {

//            function setTitle() {
//                if (isEditMode)
//                    $scope.title = UtilsService.buildTitleForUpdateEditor(title);
//                else
//                    $scope.title = UtilsService.buildTitleForAddEditor(title);
//            }
//            if (dataRow) {
//                $scope.scopeModel.headerText =  dataRow.HeaderText;
//                $scope.scopeModel.title = dataRow.Title;
//            }
           
//            return UtilsService.waitMultipleAsyncOperations([loadEditorDefinitionDirective,loadColumnSettingDirective, setTitle]).catch(function (error) {
//                VRNotificationService.notifyExceptionWithClose(error, $scope);
//            }).finally(function () {
//                $scope.scopeModel.isLoading = false;
//            });
//        }

//        function buildDataRowFromScope() {
//            return {
//                GridGenericEditorFieldId: dataRow != undefined ? dataRow.GridGenericEditorFieldId: UtilsService.guid(),
//                HeaderText: $scope.scopeModel.headerText,
//                Title: $scope.scopeModel.title,
//                Settings: editorDefinitionAPI.getData(),
//                GridColumnSettings: columnSettingDirectiveAPI.getData()
//            };
//        }

//        function insertDataRow() {
//            $scope.scopeModel.isLoading = true;
//            if ($scope.onRowAdded != undefined) {
//                $scope.onRowAdded(buildDataRowFromScope());
//            }
//            $scope.modalContext.closeModal();
//            $scope.scopeModel.isLoading = true;
//        }

//        function updateDataRow() {
//            $scope.scopeModel.isLoading = true;
//            if ($scope.onRowUpdated != undefined) {
//                $scope.onRowUpdated(buildDataRowFromScope());
//            }
//            $scope.modalContext.closeModal();
//            $scope.scopeModel.isLoading = true;

//        }

//    }
//    appControllers.controller('VR_GenericData_GridGenericEditorViewTypeDefinitionController', gridViewEditorController);
//})(appControllers);