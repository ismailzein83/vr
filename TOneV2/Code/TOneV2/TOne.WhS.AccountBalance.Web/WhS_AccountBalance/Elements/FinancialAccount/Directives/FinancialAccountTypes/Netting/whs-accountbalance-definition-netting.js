"use strict";

app.directive("whsAccountbalanceDefinitionNetting", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CustomerPostPaid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/FinancialAccountTypes/Netting/Templates/NettingDefinitionSettings.html"

        };

        function CustomerPostPaid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var supplierUsageTransactionTypeApi;
            var supplierUsageTransactionTypePromiseDeferred = UtilsService.createPromiseDeferred();

            var customerUsageTransactionTypeApi;
            var customerUsageTransactionTypePromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSupplierUsageTransactionTypeReady = function (api) {
                    supplierUsageTransactionTypeApi = api;
                    supplierUsageTransactionTypePromiseDeferred.resolve();
                };

                $scope.scopeModel.onCustomerUsageTransactionTypeReady = function (api) {
                    customerUsageTransactionTypeApi = api;
                    customerUsageTransactionTypePromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettingsEntity;
                    if (payload != undefined) {
                        extendedSettingsEntity = payload.extendedSettingsEntity;
                    }
                    var promises = [];
                    promises.push(supplierUsageTransactionTypeLoadPromise());

                    promises.push(customerUsageTransactionTypeLoadPromise());

                    function supplierUsageTransactionTypeLoadPromise() {
                        var supplierUsageTransactionTypeLoadDeferred = UtilsService.createPromiseDeferred();
                        supplierUsageTransactionTypePromiseDeferred.promise.then(function () {
                            var supplierUsageTransactionPayload;
                            if (payload != undefined) {
                                supplierUsageTransactionPayload = {
                                    selectedIds: extendedSettingsEntity != undefined ? extendedSettingsEntity.SupplierUsageTransactionTypeId : undefined
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(supplierUsageTransactionTypeApi, supplierUsageTransactionPayload, supplierUsageTransactionTypeLoadDeferred);
                        });
                        return supplierUsageTransactionTypeLoadDeferred.promise;
                    }

                    function customerUsageTransactionTypeLoadPromise() {
                        var customerUsageTransactionTypeLoadDeferred = UtilsService.createPromiseDeferred();
                        customerUsageTransactionTypePromiseDeferred.promise.then(function () {
                            var customerUsageTransactionPayload;
                            if (payload != undefined) {
                                customerUsageTransactionPayload = {
                                    selectedIds: extendedSettingsEntity != undefined ? extendedSettingsEntity.CustomerUsageTransactionTypeId : undefined
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(customerUsageTransactionTypeApi, customerUsageTransactionPayload, customerUsageTransactionTypeLoadDeferred);
                        });
                        return customerUsageTransactionTypeLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.Netting.NettingDefinitionSettings ,TOne.WhS.AccountBalance.MainExtensions",
                        SupplierUsageTransactionTypeId: supplierUsageTransactionTypeApi.getSelectedIds(),
                        CustomerUsageTransactionTypeId: customerUsageTransactionTypeApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);