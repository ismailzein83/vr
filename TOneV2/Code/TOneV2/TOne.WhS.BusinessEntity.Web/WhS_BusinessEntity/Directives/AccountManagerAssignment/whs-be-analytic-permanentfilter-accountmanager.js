(function (app) {

    'use strict';

    AnalyticPermanentfilterAccountManager.$inject = ['VRUIUtilsService', 'UtilsService'];

    function AnalyticPermanentfilterAccountManager(VRUIUtilsService, UtilsService) {
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

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onCustomerDimensionSelectorDirectiveReady = function (api) {
                    customerDimensionSelectorAPI = api;
                    customerDimensionSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierDimensionSelectorDirectiveReady = function (api) {
                    supplierDimensionSelectorAPI = api;
                    supplierDimensionSelectorReadyPromiseDeferred.resolve();
                };
               
                defineAPI();

            }
            
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        promises.push(loadCustomerAccountManagerDimensionSelector(payload));
                        promises.push(loadSupplierAccountManagerDimensionSelector(payload));
                    }
                    return UtilsService.waitPromiseNode({
                        promises: promises
                    });
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Business.AccountManagerAnalyticPermanentFilter,TOne.WhS.BusinessEntity.Business",
                        CustomerAccountManagerDimension: customerDimensionSelectorAPI.getSelectedIds(),
                        SupplierAccountManagerDimension: supplierDimensionSelectorAPI.getSelectedIds()
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