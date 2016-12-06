(function (appControllers) {

    'use strict';

    AggregateGroupingItemEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function AggregateGroupingItemEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var aggregates = [];
        var aggregateEntity;
        var isEditMode;

        var fieldTypeAPI;
        var fieldTypeReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                aggregateEntity = parameters.aggregateEntity;
            }
            isEditMode = (aggregateEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onFieldTypeSelectiveReady = function (api) {
                fieldTypeAPI = api;
                fieldTypeReadyDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateAggregate() : addAggregate();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            function builAggregateObjFromScope() {
                return {
                    AggregateItemFieldId: aggregateEntity != undefined ? aggregateEntity.AggregateItemFieldId : UtilsService.guid(),
                    FieldDescription: $scope.scopeModel.fieldDescription,
                    FieldName: $scope.scopeModel.fieldName,
                    FieldType: fieldTypeAPI.getData()
                };
            }

            function addAggregate() {
                var aggregateObj = builAggregateObjFromScope();
                if ($scope.onAggregateGroupingItemAdded != undefined) {
                    $scope.onAggregateGroupingItemAdded(aggregateObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateAggregate() {
                var aggregateObj = builAggregateObjFromScope();
                if ($scope.onAggregateGroupingItemUpdated != undefined) {
                    $scope.onAggregateGroupingItemUpdated(aggregateObj);
                }
                $scope.modalContext.closeModal();
            }

        }
        function load() {

            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {


                function setTitle() {
                    if (isEditMode && aggregateEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(aggregateEntity.FieldDescription, 'Aggregate');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Aggregate');
                }
                function loadStaticData() {
                    if (aggregateEntity != undefined) {
                        $scope.scopeModel.fieldDescription = aggregateEntity.FieldDescription;
                        $scope.scopeModel.fieldName = aggregateEntity.FieldName;
                    }
                }
                function loadFieldTypeDirective() {
                    var fieldTypeLoadDeferred = UtilsService.createPromiseDeferred();
                    fieldTypeReadyDeferred.promise.then(function () {
                        var fieldTypePayload = aggregateEntity != undefined ? aggregateEntity.FieldType : undefined;
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
    appControllers.controller('VR_Invoice_AggregateGroupingItemEditorController', AggregateGroupingItemEditorController);

})(appControllers);