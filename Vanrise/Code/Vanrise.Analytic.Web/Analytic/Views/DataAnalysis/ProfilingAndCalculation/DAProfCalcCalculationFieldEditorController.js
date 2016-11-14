(function (appControllers) {

    'use strict';

    DAProfCalcCalculationFieldController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function DAProfCalcCalculationFieldController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var calculationFieldEntity;
        var context;

        var fieldTypeSelectiveAPI;
        var fieldTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                calculationFieldEntity = parameters.daProfCalcCalculationField;
                context = parameters.context;
            }

            isEditMode = (calculationFieldEntity != undefined);
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
                    UtilsService.buildTitleForUpdateEditor((calculationFieldEntity != undefined) ? calculationFieldEntity.FieldName : null, 'Profiling and Calculation Calculation Field') :
                    UtilsService.buildTitleForAddEditor('Profiling and Calculation Calculation Field');
            }
            function loadStaticData() {
                if (calculationFieldEntity == undefined)
                    return;

                $scope.scopeModel.fieldName = calculationFieldEntity.FieldName;
                $scope.scopeModel.expression = calculationFieldEntity.Expression;
            }
            function loadFieldTypeSelective() {
                var fieldTypeSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                fieldTypeSelectiveReadyDeferred.promise.then(function () {
                    var fieldTypeSelectivePayload;

                    if (calculationFieldEntity != undefined && calculationFieldEntity.FieldType) {
                        fieldTypeSelectivePayload = calculationFieldEntity.FieldType;
                    }

                    VRUIUtilsService.callDirectiveLoad(fieldTypeSelectiveAPI, fieldTypeSelectivePayload, fieldTypeSelectiveLoadDeferred);
                });

                return fieldTypeSelectiveLoadDeferred.promise;
            }
        }

        function insert() {
            var calculationFieldObject = buildCalculationFieldObjectFromScope();

            if ($scope.onDAProfCalcCalculationFieldAdded != undefined && typeof ($scope.onDAProfCalcCalculationFieldAdded) == 'function') {
                $scope.onDAProfCalcCalculationFieldAdded(calculationFieldObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var calculationFieldObject = buildCalculationFieldObjectFromScope();

            if ($scope.onDAProfCalcCalculationFieldUpdated != undefined && typeof ($scope.onDAProfCalcCalculationFieldUpdated) == 'function') {
                $scope.onDAProfCalcCalculationFieldUpdated(calculationFieldObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildCalculationFieldObjectFromScope() {

            return {
                FieldName: $scope.scopeModel.fieldName,
                FieldType: fieldTypeSelectiveAPI.getData(),
                Expression: $scope.scopeModel.expression
            };
        }
    }

    appControllers.controller('VR_Analytic_DAProfCalcCalculationFieldController', DAProfCalcCalculationFieldController);

})(appControllers);