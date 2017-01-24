(function (appControllers) {

    'use strict';

    DimensionItemGroupingEditorControlle.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function DimensionItemGroupingEditorControlle($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var dimensions = [];
        var dimensionEntity;
        var isEditMode;

        var fieldTypeAPI;
        var fieldTypeReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                dimensionEntity = parameters.dimensionEntity;
            }
            isEditMode = (dimensionEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onFieldTypeSelectiveReady = function (api) {
                fieldTypeAPI = api;
                fieldTypeReadyDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateDimension() : addDimension();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            function builDimensionObjFromScope() {
                return {
                    DimensionItemFieldId: dimensionEntity != undefined ? dimensionEntity.DimensionItemFieldId : UtilsService.guid(),
                    FieldDescription: $scope.scopeModel.fieldDescription,
                    FieldName: $scope.scopeModel.fieldName,
                    FieldType: fieldTypeAPI.getData()
                };
            }

            function addDimension() {
                var dimensionObj = builDimensionObjFromScope();
                if ($scope.onDimensionItemGroupingAdded != undefined) {
                    $scope.onDimensionItemGroupingAdded(dimensionObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateDimension() {
                var dimensionObj = builDimensionObjFromScope();
                if ($scope.onDimensionItemGroupingUpdated != undefined) {
                    $scope.onDimensionItemGroupingUpdated(dimensionObj);
                }
                $scope.modalContext.closeModal();
            }

        }
        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {


                function setTitle() {
                    if (isEditMode && dimensionEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(dimensionEntity.FieldDescription, 'Dimension');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Dimension');
                }
                function loadStaticData() {
                    if (dimensionEntity != undefined) {
                        $scope.scopeModel.fieldDescription = dimensionEntity.FieldDescription;
                        $scope.scopeModel.fieldName = dimensionEntity.FieldName;
                    }
                }
                function loadFieldTypeDirective() {
                    var fieldTypeLoadDeferred = UtilsService.createPromiseDeferred();
                    fieldTypeReadyDeferred.promise.then(function () {
                        var fieldTypePayload = dimensionEntity != undefined ? dimensionEntity.FieldType : undefined;
                        VRUIUtilsService.callDirectiveLoad(fieldTypeAPI, fieldTypePayload, fieldTypeLoadDeferred);
                    });
                    return fieldTypeLoadDeferred.promise;
                }
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFieldTypeDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

        }

    }
    appControllers.controller('VR_Invoice_DimensionItemGroupingEditorController', DimensionItemGroupingEditorControlle);

})(appControllers);