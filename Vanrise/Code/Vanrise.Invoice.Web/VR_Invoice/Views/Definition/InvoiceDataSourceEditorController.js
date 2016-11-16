(function (appControllers) {

    'use strict';

    InvoiceDataSourceEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function InvoiceDataSourceEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var context;
        var dataSourceEntity;

        var isEditMode;

        var dataSourceSettingsAPI;
        var dataSourceSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var itemsFilterAPI;
        var itemsFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                context = parameters.context;
                dataSourceEntity = parameters.dataSourceEntity;
            }
            isEditMode = (dataSourceEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onDataSourceSettingsReady = function (api) {
                dataSourceSettingsAPI = api;
                dataSourceSettingsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.showItemsFilter = false;
            $scope.scopeModel.onItemsFilterReady = function (api) {
                itemsFilterAPI = api;
                itemsFilterReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateDataSource() : addDataSource();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataSourceSettingsDirective, loadItemsFilterDirective]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }
        function setTitle() {
            if (isEditMode && dataSourceEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataSourceEntity.DataSourceName, 'DataSource');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('DataSource');
        }
        function loadStaticData() {
            if (dataSourceEntity != undefined) {
                $scope.scopeModel.dataSourceName = dataSourceEntity.DataSourceName;
            }
        }
        function loadDataSourceSettingsDirective() {
            var dataSourceSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
           dataSourceSettingsReadyPromiseDeferred.promise.then(function () {
               var dataSourcePayload = dataSourceEntity != undefined ? { dataSourceEntity: dataSourceEntity.Settings } : undefined;
               VRUIUtilsService.callDirectiveLoad(dataSourceSettingsAPI, dataSourcePayload, dataSourceSettingsLoadPromiseDeferred);
            });
           return dataSourceSettingsLoadPromiseDeferred.promise;
        }
        function loadItemsFilterDirective() {
            if (context != undefined && context.showItemsFilter != undefined && context.showItemsFilter())
            {
                $scope.scopeModel.showItemsFilter = true;
                var itemsFilterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                itemsFilterReadyPromiseDeferred.promise.then(function () {
                    var itemsFilterPayload = dataSourceEntity != undefined ? { itemsFilterEntity: dataSourceEntity.ItemsFilter } : undefined;
                    VRUIUtilsService.callDirectiveLoad(itemsFilterAPI, itemsFilterPayload, itemsFilterLoadPromiseDeferred);
                });
                return itemsFilterLoadPromiseDeferred.promise;
            }
        }
        function builDataSourceObjFromScope() {
            return {
                DataSourceName: $scope.scopeModel.dataSourceName,
                Settings: dataSourceSettingsAPI.getData(),
                ItemsFilter: itemsFilterAPI != undefined? itemsFilterAPI.getData():undefined
            };
        }

        function addDataSource() {
            var dataSourceObj = builDataSourceObjFromScope();
            if ($scope.onDataSourceAdded != undefined) {
                $scope.onDataSourceAdded(dataSourceObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateDataSource() {
            var dataSourceObj = builDataSourceObjFromScope();
            if ($scope.onDataSourceUpdated != undefined) {
                $scope.onDataSourceUpdated(dataSourceObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    appControllers.controller('VR_Invoice_InvoiceDataSourceEditorController', InvoiceDataSourceEditorController);

})(appControllers);