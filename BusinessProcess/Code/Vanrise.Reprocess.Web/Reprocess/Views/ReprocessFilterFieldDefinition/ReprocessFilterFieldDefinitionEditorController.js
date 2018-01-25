(function (appControllers) {

    'use strict';

    ReprocessFilterFieldDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ReprocessFilterFieldDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var reprocessFilterFieldDefinition;

        var fieldTypeSelectiveAPI;
        var fieldTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                reprocessFilterFieldDefinition = parameters.reprocessFilterFieldDefinition;
            }

            isEditMode = (reprocessFilterFieldDefinition != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onFieldTypeSelectiveReady = function (api) {
                fieldTypeSelectiveAPI = api;
                fieldTypeSelectiveReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFieldTypeSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((reprocessFilterFieldDefinition != undefined) ? reprocessFilterFieldDefinition.FieldName : null, 'Reprocess Filter Field') :
                    UtilsService.buildTitleForAddEditor('Reprocess Filter Field');
            }

            function loadStaticData() {
                if (reprocessFilterFieldDefinition == undefined)
                    return;

                $scope.scopeModel.fieldName = reprocessFilterFieldDefinition.FieldName;
                $scope.scopeModel.fieldTitle = reprocessFilterFieldDefinition.FieldTitle;
            }

            function loadFieldTypeSelective() {
                var fieldTypeSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                fieldTypeSelectiveReadyDeferred.promise.then(function () {
                    var fieldTypeSelectivePayload;

                    if (reprocessFilterFieldDefinition != undefined && reprocessFilterFieldDefinition.FieldType) {
                        fieldTypeSelectivePayload = reprocessFilterFieldDefinition.FieldType;
                    }

                    VRUIUtilsService.callDirectiveLoad(fieldTypeSelectiveAPI, fieldTypeSelectivePayload, fieldTypeSelectiveLoadDeferred);
                });

                return fieldTypeSelectiveLoadDeferred.promise;
            }
        }

        function insert() {
            var calculationFieldObject = buildCalculationFieldObjectFromScope();

            if ($scope.onReprocessFilterFieldDefinitionAdded != undefined && typeof ($scope.onReprocessFilterFieldDefinitionAdded) == 'function') {
                $scope.onReprocessFilterFieldDefinitionAdded(calculationFieldObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var calculationFieldObject = buildCalculationFieldObjectFromScope();

            if ($scope.onReprocessFilterFieldDefinitionUpdated != undefined && typeof ($scope.onReprocessFilterFieldDefinitionUpdated) == 'function') {
                $scope.onReprocessFilterFieldDefinitionUpdated(calculationFieldObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildCalculationFieldObjectFromScope() {

            return {
                FieldName: $scope.scopeModel.fieldName,
                FieldTitle: $scope.scopeModel.fieldTitle,
                FieldType: fieldTypeSelectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Reprocess_ReprocessFilterFieldDefinitionEditorController', ReprocessFilterFieldDefinitionEditorController);

})(appControllers);