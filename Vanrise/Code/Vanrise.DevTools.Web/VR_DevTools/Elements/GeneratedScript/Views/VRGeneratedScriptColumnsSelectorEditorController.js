(appControllers); (function (appControllers) {
    "use strict";
    generatedScriptColumnsSelectorEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Devtools_GeneratedScriptService','VR_Devtools_GeneratedScriptColumnsSelectionTypeEnum'];

    function generatedScriptColumnsSelectorEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Devtools_GeneratedScriptService, VR_Devtools_GeneratedScriptColumnsSelectionTypeEnum) {

        var payload;
        var columnNames;
        var columnsSelectionTypeDirectiveApi;
        var columnsSelectionTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var bulkActionDraftInstance;
        var columnsDirectiveApi;
        var columnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                payload = parameters.payload;
            }
        }

        function defineScope() {

            $scope.scopeModel.saveGeneratedScriptVariable = function () {

                if (isEditMode)
                    return updateGeneratedScriptVariable();
                else
                    return insertGeneratedScriptVariable();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onColumnsSelectionTypeDirectiveReady = function (api) {
                columnsSelectionTypeDirectiveApi = api;
                columnsSelectionTypeReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onColumnsDirectiveReady = function (api) {
                columnsDirectiveApi = api;
                columnsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.isColumnsSelection = function () {
                return columnsSelectionTypeDirectiveApi.getSelectedIds() == VR_Devtools_GeneratedScriptColumnsSelectionTypeEnum.ChooseColumns.value;
            };

            $scope.scopeModel.generateSelectedTableData=function() {
                $scope.scopeModel.isLoading = true;
                payload.filteredColumnNames = columnsSelectionTypeDirectiveApi.getSelectedIds() == VR_Devtools_GeneratedScriptColumnsSelectionTypeEnum.AllColumns.value ? undefined : columnsDirectiveApi.getSelectedIds();
                $scope.generateSelectedTableDataGrid(payload).then(function () {
                    $scope.deselectAllItems();
                    payload.context.compareTables();
                    $scope.modalContext.closeModal();
                    $scope.scopeModel.isLoading = false;
                });
            };
            $scope.scopeModel.onColumnsChanged = function () { };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

                loadAllControls();
        }

        function loadAllControls() {
            function loadColumnsSelectionTypeDirective() {
                var columnsSelectionTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                columnsSelectionTypeReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(columnsSelectionTypeDirectiveApi, { selectedIds: VR_Devtools_GeneratedScriptColumnsSelectionTypeEnum.AllColumns.value}, columnsSelectionTypeLoadPromiseDeferred);
                });
                return columnsSelectionTypeLoadPromiseDeferred.promise;
            }

            function loadColumnsDirective() {

                var columnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                columnsReadyPromiseDeferred.promise.then(function (response) {
                    var filter = payload.Query;
                    filter.ColumnNames = $scope.columnNames();
                    VRUIUtilsService.callDirectiveLoad(columnsDirectiveApi, { filter: filter }, columnsDirectiveLoadDeferred);
                });

                return columnsDirectiveLoadDeferred.promise;
            }

     
            function setTitle() {
                    $scope.title ="Columns Selector";
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadColumnsSelectionTypeDirective, loadColumnsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

    }
    appControllers.controller('VR_Devtools_GeneratedScriptColumnsSelectorEditorController', generatedScriptColumnsSelectorEditorController);
})(appControllers);