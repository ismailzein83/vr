(function (appControllers) {

    'use strict';

    GroupingItemEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function GroupingItemEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var groupingItemEntity;

        var isEditMode;

        var dimensionsAPI;
        var dimensionsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var aggregatesAPI;
        var aggregatesReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                groupingItemEntity = parameters.groupingItemEntity;
            }
            isEditMode = (groupingItemEntity != undefined);
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
                return (isEditMode) ? updateGroupingItem() : addGroupingItem();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            function builGroupingItemObjFromScope() {
                return {
                    GroupingItemId: groupingItemEntity != undefined ? groupingItemEntity.GroupingItemId : UtilsService.guid(),
                    ItemSetName: $scope.scopeModel.groupingItemName,
                    DimensionItemFields: dimensionsAPI.getData(),
                    AggregateItemFields: aggregatesAPI.getData()
                };
            }

            function addGroupingItem() {
                var groupingItemObj = builGroupingItemObjFromScope();
                if ($scope.onGroupingItemAdded != undefined) {
                    $scope.onGroupingItemAdded(groupingItemObj);
                }
                $scope.modalContext.closeModal();
            }

            function updateGroupingItem() {
                var groupingItemObj = builGroupingItemObjFromScope();
                if ($scope.onGroupingItemUpdated != undefined) {
                    $scope.onGroupingItemUpdated(groupingItemObj);
                }
                $scope.modalContext.closeModal();
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {
                function setTitle() {
                    if (isEditMode && groupingItemEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(groupingItemEntity.ItemSetName, 'Grouping Item');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Grouping Item');
                }
                function loadStaticData() {
                    if (groupingItemEntity != undefined) {
                        $scope.scopeModel.groupingItemName = groupingItemEntity.ItemSetName;
                    }
                }
                function loadDimensionsDirective() {
                    var dimensionsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    dimensionsReadyPromiseDeferred.promise.then(function () {
                        var dimensionPayload = groupingItemEntity != undefined ? { dimensionGroupingItems: groupingItemEntity.DimensionItemFields } : undefined;
                        VRUIUtilsService.callDirectiveLoad(dimensionsAPI, dimensionPayload, dimensionsLoadPromiseDeferred);
                    });
                    return dimensionsLoadPromiseDeferred.promise;
                }
                function loadAggregatesDirective() {
                    var aggregatesLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    aggregatesReadyPromiseDeferred.promise.then(function () {
                        var aggregatesPayload = groupingItemEntity != undefined ? { aggregateGroupingItems: groupingItemEntity.AggregateItemFields } : undefined;
                        VRUIUtilsService.callDirectiveLoad(aggregatesAPI, aggregatesPayload, aggregatesLoadPromiseDeferred);
                    });
                    return aggregatesLoadPromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDimensionsDirective, loadAggregatesDirective]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
            }
        }

    }
    appControllers.controller('VR_Invoice_GroupingItemEditorController', GroupingItemEditorController);

})(appControllers);