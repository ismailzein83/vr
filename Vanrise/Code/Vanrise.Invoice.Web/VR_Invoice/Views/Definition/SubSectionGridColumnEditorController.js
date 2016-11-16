(function (appControllers) {

    'use strict';

    SubSectionGridColumnEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService','VRCommon_GridWidthFactorEnum'];

    function SubSectionGridColumnEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService, VRCommon_GridWidthFactorEnum) {

        var gridColumns = [];
        var gridColumnEntity;
        var isEditMode;

        var fieldTypeAPI;
        var fieldTypeReadyDeferred = UtilsService.createPromiseDeferred();

        var gridWidthFactorAPI;
        var gridWidthFactorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                gridColumns = parameters.gridColumns;
                gridColumnEntity = parameters.gridColumnEntity;
            }
            isEditMode = (gridColumnEntity != undefined);
        }
        function defineScope() {
            $scope.onFieldTypeSelectiveReady = function (api) {
                fieldTypeAPI = api;
                fieldTypeReadyDeferred.resolve();
            };
            $scope.onGridWidthFactorSelector = function (api) {
                gridWidthFactorAPI = api;
                gridWidthFactorReadyDeferred.resolve();
            };
            $scope.save = function () {
                return (isEditMode) ? updateGridColumn() : addGridColumn();
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {

            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFieldTypeDirective, loadGridWidthFactorDirective]).then(function () {

            }).finally(function () {
                $scope.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function setTitle() {
            if (isEditMode && gridColumnEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(gridColumnEntity.Header, 'Grid Column');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Grid Column');
        }
        function loadStaticData() {
            if (gridColumnEntity != undefined) {
                $scope.header = gridColumnEntity.Header ;
                $scope.fieldName = gridColumnEntity.FieldName;
                $scope.widthFactor = gridColumnEntity.WidthFactor;
            }
        }
        function builGridColumnObjFromScope() {
            return {
                Header: $scope.header,
                FieldName: $scope.fieldName,
                WidthFactor: gridWidthFactorAPI.getSelectedIds(),
                FieldType: fieldTypeAPI.getData()
            };
        }
        function loadFieldTypeDirective() {
            var fieldTypeLoadDeferred = UtilsService.createPromiseDeferred();
            fieldTypeReadyDeferred.promise.then(function () {
                var fieldTypePayload = gridColumnEntity != undefined?gridColumnEntity.FieldType:undefined;
                VRUIUtilsService.callDirectiveLoad(fieldTypeAPI, fieldTypePayload, fieldTypeLoadDeferred);
            });
            return fieldTypeLoadDeferred.promise;
        }
        function loadGridWidthFactorDirective() {
            var gridWidthFactorLoadDeferred = UtilsService.createPromiseDeferred();
            gridWidthFactorReadyDeferred.promise.then(function () {
                var gridWidthFactorPayload = { selectedIds:gridColumnEntity != undefined ? gridColumnEntity.WidthFactor:VRCommon_GridWidthFactorEnum.Normal.value };
                VRUIUtilsService.callDirectiveLoad(gridWidthFactorAPI, gridWidthFactorPayload, gridWidthFactorLoadDeferred);
            });
            return gridWidthFactorLoadDeferred.promise;
        }
        function addGridColumn() {
            var gridColumnObj = builGridColumnObjFromScope();
            if ($scope.onSubSectionGridColumnAdded != undefined) {
                $scope.onSubSectionGridColumnAdded(gridColumnObj);
            }
            $scope.modalContext.closeModal();
        }
        function updateGridColumn() {
            var gridColumnObj = builGridColumnObjFromScope();
            if ($scope.onSubSectionGridColumnUpdated != undefined) {
                $scope.onSubSectionGridColumnUpdated(gridColumnObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('VR_Invoice_SubSectionGridColumnEditorController', SubSectionGridColumnEditorController);

})(appControllers);