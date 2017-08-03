(function (appControllers) {

    'use strict';

    FilterConditionItemEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function FilterConditionItemEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var filterConditionItemEntity;

        var filterConditionSelectiveAPI;
        var filterConditionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                filterConditionItemEntity = parameters.filterConditionItemEntity;
                context = parameters.context;
            }
            isEditMode = (filterConditionItemEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onFilterConditionSelectiveReady = function (api) {
                filterConditionSelectiveAPI = api;
                filterConditionSelectiveReadyDeferred.resolve();
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

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((filterConditionItemEntity != undefined) ? filterConditionItemEntity.Name : null, 'Filter Condition') :
                    UtilsService.buildTitleForAddEditor('Filter Condition');
            }
            function loadStaticData() {
                if (filterConditionItemEntity == undefined)
                    return;
                $scope.scopeModel.name = filterConditionItemEntity.Name;

            }
            function loadFilterConditionSelective() {
                var filterConditionSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                filterConditionSelectiveReadyDeferred.promise.then(function () {
                    var filterConditionSelectivePayload = {
                        context: getContext()
                    };
                    if (filterConditionItemEntity != undefined) {
                        filterConditionSelectivePayload.invoiceFilterConditionEntity = filterConditionItemEntity.FilterCondition;
                    }
                    VRUIUtilsService.callDirectiveLoad(filterConditionSelectiveAPI, filterConditionSelectivePayload, filterConditionSelectiveLoadDeferred);
                });

                return filterConditionSelectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFilterConditionSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insert() {
            var filterConditionItemObject = buildFilterConditionItemObjectFromScope();

            if ($scope.onFilterConditionItemAdded != undefined && typeof ($scope.onFilterConditionItemAdded) == 'function') {
                $scope.onFilterConditionItemAdded(filterConditionItemObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var filterConditionItemObject = buildFilterConditionItemObjectFromScope();

            if ($scope.onFilterConditionItemUpdated != undefined && typeof ($scope.onFilterConditionItemUpdated) == 'function') {
                $scope.onFilterConditionItemUpdated(filterConditionItemObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildFilterConditionItemObjectFromScope() {
            return {
                Name: $scope.scopeModel.name,
                FilterCondition: filterConditionSelectiveAPI.getData(),
            };
        }

        function getContext()
        {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }

    appControllers.controller('VR_Invoice_FilterConditionItemEditorController', FilterConditionItemEditorController);

})(appControllers);