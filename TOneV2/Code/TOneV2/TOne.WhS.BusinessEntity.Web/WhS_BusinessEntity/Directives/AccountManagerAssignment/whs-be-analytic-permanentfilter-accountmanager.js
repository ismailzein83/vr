(function (app) {

    'use strict';

    AnalyticPermanentfilterAccountManager.$inject = ['VRUIUtilsService', 'UtilsService', 'WhS_BE_AccountManagerDataTypeEnum'];

    function AnalyticPermanentfilterAccountManager(VRUIUtilsService, UtilsService, WhS_BE_AccountManagerDataTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var analyticPermanentFilterAccountManager = new AnalyticPermanentFilterAccountManager($scope, ctrl, $attrs);
                analyticPermanentFilterAccountManager.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/AccountManagerAssignment/Templates/AnalyticPermanentFilterAccountManager.html"
        };

        function AnalyticPermanentFilterAccountManager($scope, ctrl, $attrs) {
          
            var customerDimensionSelectorAPI;
            var customerDimensionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierDimensionSelectorAPI;
            var supplierDimensionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var dataTypeSelectorAPI;
            var dataTypeSelectorReadyPromiseDefereed = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataTypes = [];

                $scope.scopeModel.onCustomerDimensionSelectorDirectiveReady = function (api) {
                    customerDimensionSelectorAPI = api;
                    customerDimensionSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierDimensionSelectorDirectiveReady = function (api) {
                    supplierDimensionSelectorAPI = api;
                    supplierDimensionSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onDataTypeSelectorReady = function (api) {
                    dataTypeSelectorAPI = api;
                    dataTypeSelectorReadyPromiseDefereed.resolve();
                };

                UtilsService.waitMultiplePromises([customerDimensionSelectorReadyPromiseDeferred.promise, supplierDimensionSelectorReadyPromiseDeferred.promise, dataTypeSelectorReadyPromiseDefereed.promise]).then(function () {
                    defineAPI();
                });
            }
            
            function defineAPI() {
                var api = {};
                 
                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        promises.push(loadCustomerAccountManagerDimensionSelector(payload));
                        promises.push(loadSupplierAccountManagerDimensionSelector(payload));

                        $scope.scopeModel.dataTypes = UtilsService.getArrayEnum(WhS_BE_AccountManagerDataTypeEnum);

                        if (payload.settings != undefined) {
                            $scope.scopeModel.selectedDataType = UtilsService.getItemByVal($scope.scopeModel.dataTypes, payload.settings.DataType, 'value');
                        }
                    }
                    return UtilsService.waitPromiseNode({
                        promises: promises
                    });
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Business.AccountManagerAnalyticPermanentFilter,TOne.WhS.BusinessEntity.Business",
                        CustomerAccountManagerDimension: customerDimensionSelectorAPI.getSelectedIds(),
                        SupplierAccountManagerDimension: supplierDimensionSelectorAPI.getSelectedIds(),
                        DataType: $scope.scopeModel.selectedDataType.value
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadCustomerAccountManagerDimensionSelector(payload) {
                var loadCustomerDimensionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                customerDimensionSelectorReadyPromiseDeferred.promise.then(function () {
                    var customerDimensionPayload = {
                        filter: { TableIds: [payload.analyticTableId] },
                        selectedIds: payload.settings != undefined ? payload.settings.CustomerAccountManagerDimension : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(customerDimensionSelectorAPI, customerDimensionPayload, loadCustomerDimensionSelectorPromiseDeferred);
                });
                return loadCustomerDimensionSelectorPromiseDeferred.promise;
            }

            function loadSupplierAccountManagerDimensionSelector(payload) {
                var loadSupplierDimensionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                supplierDimensionSelectorReadyPromiseDeferred.promise.then(function () {

                    var supplierDimensionPayload = {
                        filter: { TableIds: [payload.analyticTableId] },
                        selectedIds: payload.settings != undefined ? payload.settings.SupplierAccountManagerDimension : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierDimensionSelectorAPI, supplierDimensionPayload, loadSupplierDimensionSelectorPromiseDeferred);
                });
                return loadSupplierDimensionSelectorPromiseDeferred.promise;
            }
        }
    }

    app.directive('whsBeAnalyticPermanentfilterAccountmanager', AnalyticPermanentfilterAccountManager);

})(app);