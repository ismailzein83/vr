(function (app) {

    'use strict';

    DatarecordstoragePermanentfilterAccountManager.$inject = ['VRUIUtilsService', 'UtilsService'];

    function DatarecordstoragePermanentfilterAccountManager(VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordStoragePermanentFilterAccountManager = new DataRecordStoragePermanentFilterAccountManager($scope, ctrl, $attrs);
                dataRecordStoragePermanentFilterAccountManager.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/AccountManagerAssignment/Templates/DataRecordStoragePermanentFilterAccountManager.html"
        };

        function DataRecordStoragePermanentFilterAccountManager($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var customerFieldSelectorAPI;
            var customerFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierFieldSelectorAPI;
            var supplierFieldSelectorReadyPromiseDeferred= UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCustomerFieldSelectorDirectiveReady = function (api) {
                    customerFieldSelectorAPI = api;
                    customerFieldSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierFieldSelectorDirectiveReady = function (api) {
                    supplierFieldSelectorAPI = api;
                    supplierFieldSelectorReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function loadCustomerFieldSelector(payload) {
                var loadCustomerFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                customerFieldSelectorReadyPromiseDeferred.promise.then(function () {
                    var customerFieldPayload = {
                        dataRecordTypeId: payload.dataRecordTypeId,
                        selectedIds: payload.settings != undefined ? payload.settings.CustomerField : undefined,
                    };
                    VRUIUtilsService.callDirectiveLoad(customerFieldSelectorAPI, customerFieldPayload, loadCustomerFieldSelectorPromiseDeferred);
                });
                return loadCustomerFieldSelectorPromiseDeferred.promise;
            }

            function loadSupplierFieldSelector(payload) {
                var loadSupplierFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                supplierFieldSelectorReadyPromiseDeferred.promise.then(function () {
                    var supplierFieldPayload = {
                        dataRecordTypeId: payload.dataRecordTypeId,
                        selectedIds: payload.settings != undefined ? payload.settings.SupplierField : undefined,
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierFieldSelectorAPI, supplierFieldPayload, loadSupplierFieldSelectorPromiseDeferred);
                });
                return loadSupplierFieldSelectorPromiseDeferred.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        promises.push(loadCustomerFieldSelector(payload));
                        promises.push(loadSupplierFieldSelector(payload));
                    }
                    return UtilsService.waitPromiseNode({
                        promises: promises
                    });
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Business.AccountManagerDataRecordStoragePermanentFilter,TOne.WhS.BusinessEntity.Business",
                        CustomerField: customerFieldSelectorAPI.getSelectedIds(),
                        SupplierField: supplierFieldSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }


        }
    }

    app.directive('whsBeDatarecordstoragePermanentfilterAccountmanager', DatarecordstoragePermanentfilterAccountManager);

})(app); 