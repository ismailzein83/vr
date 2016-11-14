(function (appControllers) {

    'use strict';

    DAProfCalcGroupingFieldController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function DAProfCalcGroupingFieldController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var groupingFieldEntity;
        var context;

        var dataRecordTypeFieldsSelectorAPI;
        var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var fieldTypeSelectiveAPI;
        var fieldTypeSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                groupingFieldEntity = parameters.daProfCalcGroupingField;
                context = parameters.context;
            }

            isEditMode = (groupingFieldEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                dataRecordTypeFieldsSelectorAPI = api;
                dataRecordTypeFieldsSelectorReadyDeferred.resolve();
            };

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordTypeFieldsSelector, loadFieldTypeSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((groupingFieldEntity != undefined) ? groupingFieldEntity.FieldName : null, 'Grouping Field') :
                    UtilsService.buildTitleForAddEditor('Grouping Fields');
            }
            function loadStaticData() {
                if (groupingFieldEntity == undefined)
                    return;

                $scope.scopeModel.fieldName = groupingFieldEntity.FieldName;
            }
            function loadDataRecordTypeFieldsSelector() {

                var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {
                    var dataRecordTypeFieldsSelectorPayload = {};
                    if (context != undefined) {
                        dataRecordTypeFieldsSelectorPayload.dataRecordTypeId = context.getDataRecordTypeId();
                    }
                    if (groupingFieldEntity != undefined) {
                        dataRecordTypeFieldsSelectorPayload.selectedIds = groupingFieldEntity.FieldName;
                    }
                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);
                });

                return dataRecordTypeFieldsSelectorLoadDeferred.promise;
            }
            function loadFieldTypeSelective() {
                var fieldTypeSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                fieldTypeSelectiveReadyDeferred.promise.then(function () {
                    var fieldTypeSelectivePayload;

                    if (groupingFieldEntity != undefined && groupingFieldEntity.FieldType) {
                        fieldTypeSelectivePayload = groupingFieldEntity.FieldType;
                    }

                    VRUIUtilsService.callDirectiveLoad(fieldTypeSelectiveAPI, fieldTypeSelectivePayload, fieldTypeSelectiveLoadDeferred);
                });

                return fieldTypeSelectiveLoadDeferred.promise;
            }
        }

        function insert() {
            var calculationFieldObject = buildCalculationFieldObjectFromScope();

            if ($scope.onDAProfCalcGroupingFieldAdded != undefined && typeof ($scope.onDAProfCalcGroupingFieldAdded) == 'function') {
                $scope.onDAProfCalcGroupingFieldAdded(calculationFieldObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var calculationFieldObject = buildCalculationFieldObjectFromScope();

            if ($scope.onDAProfCalcGroupingFieldUpdated != undefined && typeof ($scope.onDAProfCalcGroupingFieldUpdated) == 'function') {
                $scope.onDAProfCalcGroupingFieldUpdated(calculationFieldObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildCalculationFieldObjectFromScope() {

            return {
                FieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                FieldType: fieldTypeSelectiveAPI.getData(),
            };
        }
    }

    appControllers.controller('VR_Analytic_DAProfCalcGroupingFieldController', DAProfCalcGroupingFieldController);

})(appControllers);