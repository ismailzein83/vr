(function (appControllers) {

    'use strict';

    ItemGroupingEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function ItemGroupingEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var itemGroupingEntity;

        var isEditMode;

        var dimensionsAPI;
        var dimensionsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var aggregatesAPI;
        var aggregatesReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var orderTypeSelectorAPI;
        var orderTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                itemGroupingEntity = parameters.itemGroupingEntity;
            }
            isEditMode = (itemGroupingEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onDimensionsReady = function (api) {
                dimensionsAPI = api;
                dimensionsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onAggregatesReady = function (api) {
                aggregatesAPI = api;
                aggregatesReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateItemGrouping() : addItemGrouping();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onOrderTypeSelectorReady = function (api) {
                orderTypeSelectorAPI = api;
                orderTypeSelectorReadyDeferred.resolve();
            };

            function builItemGroupingObjFromScope() {
                return {
                    ItemGroupingId: itemGroupingEntity != undefined ? itemGroupingEntity.ItemGroupingId : UtilsService.guid(),
                    ItemSetName: $scope.scopeModel.itemGroupingName,
                    DimensionItemFields: dimensionsAPI.getData(),
                    AggregateItemFields: aggregatesAPI.getData(),
                    OrderType: orderTypeSelectorAPI.getData()

                };
            }

            function addItemGrouping() {
                var itemGroupingObj = builItemGroupingObjFromScope();
                if ($scope.onItemGroupingAdded != undefined) {
                    $scope.onItemGroupingAdded(itemGroupingObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateItemGrouping() {
                var itemGroupingObj = builItemGroupingObjFromScope();
                if ($scope.onItemGroupingUpdated != undefined) {
                    $scope.onItemGroupingUpdated(itemGroupingObj);
                }
                $scope.modalContext.closeModal();
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && itemGroupingEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(itemGroupingEntity.ItemSetName, 'Grouping Item');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Grouping Item');
                }
                function loadStaticData() {
                    if (itemGroupingEntity != undefined) {
                        $scope.scopeModel.itemGroupingName = itemGroupingEntity.ItemSetName;
                    }
                }
                function loadDimensionsDirective() {
                    var dimensionsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    dimensionsReadyPromiseDeferred.promise.then(function () {
                        var dimensionPayload = itemGroupingEntity != undefined ? { dimensionItemGroupings: itemGroupingEntity.DimensionItemFields } : undefined;
                        VRUIUtilsService.callDirectiveLoad(dimensionsAPI, dimensionPayload, dimensionsLoadPromiseDeferred);
                    });
                    return dimensionsLoadPromiseDeferred.promise;
                }
                function loadAggregatesDirective() {
                    var aggregatesLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    aggregatesReadyPromiseDeferred.promise.then(function () {
                        var aggregatesPayload = itemGroupingEntity != undefined ? { aggregateItemGroupings: itemGroupingEntity.AggregateItemFields } : undefined;
                        VRUIUtilsService.callDirectiveLoad(aggregatesAPI, aggregatesPayload, aggregatesLoadPromiseDeferred);
                    });
                    return aggregatesLoadPromiseDeferred.promise;
                }
                function loadOrderTypeSelective() {
                    var orderTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    orderTypeSelectorReadyDeferred.promise.then(function () {
                        var orderTypeSelectorPayload = {};
                      //  var orderTypeSelectorPayload = { tableIds: payload.tableIds };
                         if (itemGroupingEntity != undefined)
                             orderTypeSelectorPayload.orderTypeEntity = { OrderType: itemGroupingEntity.OrderType };
                        VRUIUtilsService.callDirectiveLoad(orderTypeSelectorAPI, orderTypeSelectorPayload, orderTypeSelectorLoadDeferred);
                    });
                    return orderTypeSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDimensionsDirective, loadAggregatesDirective, loadOrderTypeSelective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }
        }

    }
    appControllers.controller('VR_Invoice_ItemGroupingEditorController', ItemGroupingEditorController);

})(appControllers);