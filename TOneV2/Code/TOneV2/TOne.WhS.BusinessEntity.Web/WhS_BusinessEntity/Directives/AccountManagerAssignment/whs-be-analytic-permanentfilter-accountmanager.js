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
            var timeDimensionSelectorAPI;
            var timeDimensionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                $scope.scopeModel.onTimeDimensionSelectorDirectiveReady = function (api) {
                    timeDimensionSelectorAPI = api;
                    timeDimensionSelectorReadyPromiseDeferred.resolve();
                };
                defineAPI();

            }
            function loadCustomerDimensionSelector(payload) {
                var loadCustomerDimensionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                customerDimensionSelectorReadyPromiseDeferred.promise.then(function () {
                    var customerDimensionPayload = {
                        filter: { TableIds: [payload.analyticTableId] },
                        selectedIds: payload.settings != undefined ? payload.settings.CustomerDimension : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(customerDimensionSelectorAPI, customerDimensionPayload, loadCustomerDimensionSelectorPromiseDeferred);
                });
                return loadCustomerDimensionSelectorPromiseDeferred.promise;
            }
            function loadSupplierDimensionSelector(payload) {
                var loadSupplierDimensionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                supplierDimensionSelectorReadyPromiseDeferred.promise.then(function () {

                    var supplierDimensionPayload = {
                        filter: { TableIds: [payload.analyticTableId] },
                        selectedIds: payload.settings != undefined ? payload.settings.SupplierDimension : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierDimensionSelectorAPI, supplierDimensionPayload, loadSupplierDimensionSelectorPromiseDeferred);
                });
                return loadSupplierDimensionSelectorPromiseDeferred.promise;
            }
            function loadTimeDimensionSelector(payload) {
                var loadTimeDimensionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                timeDimensionSelectorReadyPromiseDeferred.promise.then(function () {

                    var timeDimensionPayload = {
                        filter: { TableIds: [payload.analyticTableId] },
                        selectedIds: payload.settings != undefined ? payload.settings.TimeDimension : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(timeDimensionSelectorAPI, timeDimensionPayload, loadTimeDimensionSelectorPromiseDeferred);
                });
                return loadTimeDimensionSelectorPromiseDeferred.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        promises.push(loadCustomerDimensionSelector(payload));
                        promises.push(loadSupplierDimensionSelector(payload));
                        promises.push(loadTimeDimensionSelector(payload));
                    }
                    return UtilsService.waitPromiseNode({
                        promises: promises
                    });
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Business.AccountManagerAnalyticPermanentFilter,TOne.WhS.BusinessEntity.Business",
                        TimeDimension: timeDimensionSelectorAPI.getSelectedIds(),
                        CustomerDimension: customerDimensionSelectorAPI.getSelectedIds(),
                        SupplierDimension: supplierDimensionSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }


        }
    }

    app.directive('whsBeAnalyticPermanentfilterAccountmanager', AnalyticPermanentfilterAccountManager);

})(app);