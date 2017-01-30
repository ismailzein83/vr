(function (appControllers) {

    'use strict';

    VisibilityGridColumnController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VisibilityGridColumnController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var visibilityGridColumnEntity;
        var columnDefinitions;

        var accountGenericFieldDefinitionSelectorAPI;
        var accountGenericFieldDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            console.log(parameters);

            if (parameters != undefined) {
                visibilityGridColumnEntity = parameters.visibilityGridColumn;
                columnDefinitions = parameters.columnDefinitions;
            }

            isEditMode = (visibilityGridColumnEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.datasource = [];

            $scope.scopeModel.onAccountGenericFieldDefinitionSelectorReady = function (api) {
                accountGenericFieldDefinitionSelectorAPI = api;
                accountGenericFieldDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadGridColumnsSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((visibilityGridColumnEntity != undefined) ? visibilityGridColumnEntity.FieldName : null, 'Grid Column') :
                UtilsService.buildTitleForAddEditor('Grid Column');
        }
        function loadStaticData() {
            if (visibilityGridColumnEntity == undefined)
                return;

            $scope.scopeModel.title = visibilityGridColumnEntity.Title;
        }
        function loadGridColumnsSelector() {
            if (columnDefinitions != undefined) {
                for (var i = 0; i < columnDefinitions.length; i++) {
                    $scope.scopeModel.datasource.push(columnDefinitions[i]);
                }
                if (visibilityGridColumnEntity != undefined) {
                    $scope.scopeModel.selectedvalue = UtilsService.getItemByVal($scope.scopeModel.datasource, visibilityGridColumnEntity.FieldName, 'FieldName');
                }
            }

        }

        function insert() {
            var visibilityGridColumnObject = buildVisibilityGridColumnFromScope();
            if ($scope.onVisibilityGridColumnAdded != undefined && typeof ($scope.onVisibilityGridColumnAdded) == 'function') {
                $scope.onVisibilityGridColumnAdded(visibilityGridColumnObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var visibilityGridColumnObject = buildVisibilityGridColumnFromScope();
            if ($scope.onVisibilityGridColumnUpdated != undefined && typeof ($scope.onVisibilityGridColumnUpdated) == 'function') {
                $scope.onVisibilityGridColumnUpdated(visibilityGridColumnObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildVisibilityGridColumnFromScope() {

            var selectedvalue = $scope.scopeModel.selectedvalue;

            console.log(selectedvalue);

            return {
                Title: $scope.scopeModel.title,
                FieldName: selectedvalue.FieldName,
                Header: selectedvalue.Header
            };
        }
    }

    appControllers.controller('Retail_BE_VisibilityGridColumnController', VisibilityGridColumnController);

})(appControllers);