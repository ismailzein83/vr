(function (appControllers) {

    'use strict';

    DAProfCalcCalculationFieldController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function DAProfCalcCalculationFieldController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var calculationFieldEntity;
        var context;

        var dataRecordTypeFieldsSelectorAPI;
        var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                dataRecordTypeFieldsSelectorAPI = api;
                dataRecordTypeFieldsSelectorReadyDeferred.resolve();
            }

            $scope.scopeModel.onValidateProperties = function () {

                if (propertyEntity != undefined && propertyEntity.Name == $scope.scopeModel.propertyName)
                    return null;

                for (var i = 0; i < properties.length; i++) {
                    var property = properties[i];
                    if ($scope.scopeModel.propertyName.toLowerCase() == property.Name.toLowerCase()) {
                        return 'Same Property Name Exists';
                    }
                }
                return null;
            }

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeFieldsSelector]).catch(function (error) {
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
            function loadDataRecordTypeFieldsSelector() {
                var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {
                    var dataRecordTypeFieldsSelectorPayload = {};

                    if (calculationFieldEntity != undefined && calculationFieldEntity.FieldType) {
                        dataRecordTypeFieldsSelectorPayload.selectedIds = calculationFieldEntity.FieldType;
                    }

                    //check if dataRecordType is selected
                    var _dataRecordTypeId = context.getDataRecordTypeId();
                    if (_dataRecordTypeId) {
                        dataRecordTypeFieldsSelectorPayload.dataRecordTypeId = _dataRecordTypeId;
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);
                    }
                    else {
                        dataRecordTypeFieldsSelectorLoadDeferred.resolve();
                    }

                });

                return dataRecordTypeFieldsSelectorLoadDeferred.promise;
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
                FieldType: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                Expression: $scope.scopeModel.expression
            };
        }
    }

    appControllers.controller('VR_Analytic_DAProfCalcCalculationFieldController', DAProfCalcCalculationFieldController);

})(appControllers);