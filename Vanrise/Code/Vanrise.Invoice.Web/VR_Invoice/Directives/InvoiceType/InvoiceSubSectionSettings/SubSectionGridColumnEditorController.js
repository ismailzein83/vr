(function (appControllers) {

    'use strict';

    SubSectionGridColumnEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRCommon_GridWidthFactorEnum','VRLocalizationService'];

    function SubSectionGridColumnEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService, VRCommon_GridWidthFactorEnum, VRLocalizationService) {

        var gridColumns = [];
        var gridColumnEntity;
        var isEditMode;

        var fieldTypeAPI;
        var fieldTypeReadyDeferred = UtilsService.createPromiseDeferred();

        var gridWidthFactorAPI;
        var gridWidthFactorReadyDeferred = UtilsService.createPromiseDeferred();

        var localizationTextResourceSelectorAPI;
        var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
            $scope.onLocalizationTextResourceSelectorReady = function (api) {
                localizationTextResourceSelectorAPI = api;
                localizationTextResourceSelectorReadyPromiseDeferred.resolve();
            };
            function builGridColumnObjFromScope() {
                return {
                    Header: $scope.header,
                    FieldName: $scope.fieldName,
                    WidthFactor: gridWidthFactorAPI.getSelectedIds(),
                    FieldType: fieldTypeAPI.getData(),
                    TextResourceKey: localizationTextResourceSelectorAPI != undefined ? localizationTextResourceSelectorAPI.getSelectedValues() : undefined
                };
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
        function load() {

            $scope.isLoading = true;
            loadAllControls();

            function loadAllControls() {


                function setTitle() {
                    if (isEditMode && gridColumnEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(gridColumnEntity.Header, 'Grid Column');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Grid Column');
                }
                function loadStaticData() {
                    if (gridColumnEntity != undefined) {
                        $scope.header = gridColumnEntity.Header;
                        $scope.fieldName = gridColumnEntity.FieldName;
                        $scope.widthFactor = gridColumnEntity.WidthFactor;
                    }
                }
                function loadFieldTypeDirective() {
                    var fieldTypeLoadDeferred = UtilsService.createPromiseDeferred();
                    fieldTypeReadyDeferred.promise.then(function () {
                        var fieldTypePayload = gridColumnEntity != undefined ? gridColumnEntity.FieldType : undefined;
                        VRUIUtilsService.callDirectiveLoad(fieldTypeAPI, fieldTypePayload, fieldTypeLoadDeferred);
                    });
                    return fieldTypeLoadDeferred.promise;
                }
                function loadGridWidthFactorDirective() {
                    var gridWidthFactorLoadDeferred = UtilsService.createPromiseDeferred();
                    gridWidthFactorReadyDeferred.promise.then(function () {
                        var gridWidthFactorPayload = { selectedIds: gridColumnEntity != undefined ? gridColumnEntity.WidthFactor : VRCommon_GridWidthFactorEnum.Normal.value };
                        VRUIUtilsService.callDirectiveLoad(gridWidthFactorAPI, gridWidthFactorPayload, gridWidthFactorLoadDeferred);
                    });
                    return gridWidthFactorLoadDeferred.promise;
                }
                function loadLocalizationTextResourceSelector() {
                    var localizationTextResourceSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    var localizationTextResourcePayload = gridColumnEntity != undefined ? { selectedValue: gridColumnEntity.TextResourceKey } : undefined;

                    localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, localizationTextResourcePayload, localizationTextResourceSelectorLoadPromiseDeferred);
                    });
                    return localizationTextResourceSelectorLoadPromiseDeferred.promise;
                }
                var promises = [setTitle, loadStaticData, loadFieldTypeDirective, loadGridWidthFactorDirective, loadLocalizationTextResourceSelector];
                return UtilsService.waitMultipleAsyncOperations(promises).then(function () {

                }).finally(function () {
                    $scope.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }
    }
    appControllers.controller('VR_Invoice_SubSectionGridColumnEditorController', SubSectionGridColumnEditorController);

})(appControllers);