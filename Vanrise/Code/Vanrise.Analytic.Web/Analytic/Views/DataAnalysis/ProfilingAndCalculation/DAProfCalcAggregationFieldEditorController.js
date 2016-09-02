(function (appControllers) {

    'use strict';

    DAProfCalcAggregationFieldController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function DAProfCalcAggregationFieldController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var aggregationFieldEntity;
        var context;

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            console.log(parameters);

            if (parameters != undefined) {
                aggregationFieldEntity = parameters.daProfCalcAggregationField;
                context = parameters.context;
            }
            isEditMode = (aggregationFieldEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
            };

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

            //if (isEditMode) {
            //    setPropertyEntityFromParameters().then(function () {
            //        loadAllControls()
            //    });
            //}
            //else {
            //    loadAllControls();
            //}
        }

        function setPropertyEntityFromParameters() {
            propertyEntity = UtilsService.getItemByVal(properties, propertyName, 'Name');

            var deferred = UtilsService.createPromiseDeferred();
            deferred.resolve();
            return deferred.promise;
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadRecordFilterDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((propertyEntity != undefined) ? propertyEntity.Name : null, 'Profiling and Calculation Aggregation Field') :
                    UtilsService.buildTitleForAddEditor('Profiling and Calculation Aggregation Field');
            }
            function loadStaticData() {
                if (aggregationFieldEntity == undefined)
                    return;

                $scope.scopeModel.fieldName = aggregationFieldEntity.FieldName;
            } 
            function loadRecordFilterDirective() {
                var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                recordFilterDirectiveReadyDeferred.promise.then(function () {
                    var recordFilterDirectivePayload = {};
                    recordFilterDirectivePayload.context = context;
                    if (aggregationFieldEntity != undefined && aggregationFieldEntity.RecordFilter != undefined) {
                        recordFilterDirectivePayload.FilterGroup = aggregationFieldEntity.RecordFilter;
                    }

                    console.log(recordFilterDirectivePayload);

                    VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                })

                return recordFilterDirectiveLoadDeferred.promise;
            }
        }

        function insert() {
            var aggregationFieldObject = buildAggregationFieldObjectFromScope();

            if ($scope.onDAProfCalcAggregationFieldAdded != undefined && typeof ($scope.onDAProfCalcAggregationFieldAdded) == 'function') {
                $scope.onDAProfCalcAggregationFieldAdded(aggregationFieldObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var aggregationFieldObject = buildAggregationFieldObjectFromScope();

            if ($scope.onDAProfCalcAggregationFieldUpdated != undefined && typeof ($scope.onDAProfCalcAggregationFieldUpdated) == 'function') {
                $scope.onDAProfCalcAggregationFieldUpdated(aggregationFieldObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildAggregationFieldObjectFromScope() {

            return {
                FieldName: $scope.scopeModel.fieldName,
                RecordFilter: recordFilterDirectiveAPI.getData().filterObj,
                TimeRangeFilter: null,
                RecordAggregate: null
            };
        }
    }

    appControllers.controller('VR_Analytic_DAProfCalcAggregationFieldController', DAProfCalcAggregationFieldController);

})(appControllers);