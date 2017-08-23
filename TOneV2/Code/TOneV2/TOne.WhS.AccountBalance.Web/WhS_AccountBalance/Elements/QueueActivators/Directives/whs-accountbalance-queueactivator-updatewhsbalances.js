(function (app) {

    'use strict';

    QueueActivatorUpdateWhSBalances.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorUpdateWhSBalances(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new QueueActivatorUpdateWhSBalancesCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_AccountBalance/Elements/QueueActivators/Directives/Templates/QueueActivatorUpdateWhSBalancesTemplate.html';
            }
        };

        function QueueActivatorUpdateWhSBalancesCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var updateAccountBalanceSettingsDirectiveAPI;
            var updateAccountBalanceSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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
                $scope.scopeModel.onUpdateAccountBalanceSettingsDirectiveReady = function (api) {
                    updateAccountBalanceSettingsDirectiveAPI = api;
                    updateAccountBalanceSettingsDirectiveReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([supplierUsageTransactionTypePromiseDeferred.promise, customerUsageTransactionTypePromiseDeferred.promise, updateAccountBalanceSettingsDirectiveReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
               
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var updateAccountBalanceSettings;
                    var queueActivator;
                    if (payload != undefined && payload.QueueActivator != undefined) {
                        updateAccountBalanceSettings = payload.QueueActivator.UpdateAccountBalanceSettings;
                        queueActivator = payload.QueueActivator;
                    }

                    promises.push(getUpdateAccountBalanceSettingsDirectiveLoadPromise());

                    function getUpdateAccountBalanceSettingsDirectiveLoadPromise() {
                        var payload;
                        if (updateAccountBalanceSettings != undefined) {
                            payload = {
                                updateAccountBalanceSettings: updateAccountBalanceSettings
                            };
                        }
                        return   updateAccountBalanceSettingsDirectiveAPI.load(payload);
                    }
                    promises.push(supplierUsageTransactionTypeLoadPromise());

                    function supplierUsageTransactionTypeLoadPromise() {
                        var supplierUsageTransactionPayload;
                        if (queueActivator != undefined) {
                            supplierUsageTransactionPayload = {
                                selectedIds: queueActivator.SupplierUsageTransactionTypeId
                            };
                        }
                        return supplierUsageTransactionTypeApi.load(supplierUsageTransactionPayload);
                    }

                    promises.push(customerUsageTransactionTypeLoadPromise());

                    function customerUsageTransactionTypeLoadPromise() {
                        var customerUsageTransactionPayload;
                        if (queueActivator != undefined) {
                            customerUsageTransactionPayload = {
                                selectedIds: queueActivator.CustomerUsageTransactionTypeId
                            };
                        }
                        return customerUsageTransactionTypeApi.load(customerUsageTransactionPayload);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: 'TOne.WhS.AccountBalance.MainExtensions.QueueActivators.UpdateWhSBalancesQueueActivator, TOne.WhS.AccountBalance.MainExtensions',
                        UpdateAccountBalanceSettings: updateAccountBalanceSettingsDirectiveAPI.getData(),
                        SupplierUsageTransactionTypeId: supplierUsageTransactionTypeApi.getSelectedIds(),
                        CustomerUsageTransactionTypeId: customerUsageTransactionTypeApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('whsAccountbalanceQueueactivatorUpdatewhsbalances', QueueActivatorUpdateWhSBalances);

})(app);