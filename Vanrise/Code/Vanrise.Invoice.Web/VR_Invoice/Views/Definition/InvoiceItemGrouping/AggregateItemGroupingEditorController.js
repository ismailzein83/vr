(function (appControllers) {

    'use strict';

    AggregateItemGroupingEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function AggregateItemGroupingEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var aggregates = [];
        var aggregateEntity;
        var isEditMode;

        var fieldTypeAPI;
        var fieldTypeReadyDeferred = UtilsService.createPromiseDeferred();

        var aggregateTypeAPI;
        var aggregateTypeReadyDeferred = UtilsService.createPromiseDeferred();
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
            $scope.scopeModel.onAggregateTypeSelectorReady = function (api) {
                aggregateTypeAPI = api;
                aggregateTypeReadyDeferred.resolve();
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
                    FieldType: fieldTypeAPI.getData(),
                    AggregateType: aggregateTypeAPI.getSelectedIds()
                };
            }

            function addAggregate() {
                var aggregateObj = builAggregateObjFromScope();
                if ($scope.onAggregateItemGroupingAdded != undefined) {
                    $scope.onAggregateItemGroupingAdded(aggregateObj);
                }
                $scope.modalContext.closeModal();
            }
            function updateAggregate() {
                var aggregateObj = builAggregateObjFromScope();
                if ($scope.onAggregateItemGroupingUpdated != undefined) {
                    $scope.onAggregateItemGroupingUpdated(aggregateObj);
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
                function loadAggregateTypeDirective() {
                    var aggregateTypeLoadDeferred = UtilsService.createPromiseDeferred();
                    aggregateTypeReadyDeferred.promise.then(function () {
                        var aggregateTypePayload = aggregateEntity != undefined ? { selectedIds: aggregateEntity.AggregateType } : undefined;
                        VRUIUtilsService.callDirectiveLoad(aggregateTypeAPI, aggregateTypePayload, aggregateTypeLoadDeferred);
                    });
                    return aggregateTypeLoadDeferred.promise;
                }
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFieldTypeDirective, loadAggregateTypeDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

        }

    }
    appControllers.controller('VR_Invoice_AggregateItemGroupingEditorController', AggregateItemGroupingEditorController);

})(appControllers);