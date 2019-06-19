"use strict";
app.directive("vrInvoiceGenericinvoicesettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericInvoiceSettings = new GenericInvoiceSettings($scope, ctrl, $attrs);
                genericInvoiceSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Invoice/Directives/Extensions/GenericData/Templates/InvoiceBusinessObjectDataProviderSettingsTemplate.html'
        };


        function GenericInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var configuration;
            var invoiceTransactionTypeId;
            var usageToOverrideTransactionTypeIds;

            var genericFinancialAccountDirectiveAPI;
            var genericFinancialAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var transactionTypeSelectorAPI;
            var transactionTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var usageToOverrideTransactionTypeSelectorAPI;
            var usageToOverrideTransactionTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGenricFinancialAccountReady = function (api) {
                    genericFinancialAccountDirectiveAPI = api;
                    genericFinancialAccountReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onUsageToOverrideTransactionTypeSelectorReady = function (api) {
                    usageToOverrideTransactionTypeSelectorAPI = api;
                    usageToOverrideTransactionTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onTransactionTypeSelectorReady = function (api) {
                    transactionTypeSelectorAPI = api;
                    transactionTypeSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    if (payload != undefined) {
                        configuration = payload.Configuration;
                        invoiceTransactionTypeId = payload.InvoiceTransactionTypeId;
                        usageToOverrideTransactionTypeIds = payload.UsageToOverrideTransactionTypeIds;
                    };

                    promises.push(loadGenericFinancialAccountDirective());
                    promises.push(loadTransactionTypeSelector());
                    promises.push(loadUsageToOverrideTransactionTypeSelector());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (object) {
                    object.Configuration = genericFinancialAccountDirectiveAPI.getData();
                    object.UsageToOverrideTransactionTypeIds = usageToOverrideTransactionTypeSelectorAPI.getSelectedIds();
                    object.InvoiceTransactionTypeId = transactionTypeSelectorAPI.getSelectedIds();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                function loadUsageToOverrideTransactionTypeSelector() {
                    var usageToOverrideTransactionTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    usageToOverrideTransactionTypeSelectorReadyPromiseDeferred.promise.then(function () {

                        var usageToOverrideTransactionTypePayload;
                        if (usageToOverrideTransactionTypeIds != undefined)
                            usageToOverrideTransactionTypePayload = {
                                selectedIds: usageToOverrideTransactionTypeIds
                            };
                        VRUIUtilsService.callDirectiveLoad(usageToOverrideTransactionTypeSelectorAPI, usageToOverrideTransactionTypePayload, usageToOverrideTransactionTypeSelectorLoadPromiseDeferred);
                    });
                    return usageToOverrideTransactionTypeSelectorLoadPromiseDeferred.promise;
                }

                function loadTransactionTypeSelector() {
                    var transactionTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    transactionTypeSelectorReadyPromiseDeferred.promise.then(function () {

                        var transactionTypePayload;
                        if (invoiceTransactionTypeId != undefined)
                            transactionTypePayload = {
                                selectedIds: invoiceTransactionTypeId
                            };
                        VRUIUtilsService.callDirectiveLoad(transactionTypeSelectorAPI, transactionTypePayload, transactionTypeSelectorLoadPromiseDeferred);
                    });
                    return transactionTypeSelectorLoadPromiseDeferred.promise;
                }

                function loadGenericFinancialAccountDirective() {
                    var genericFinancialAccountDirectiverLoadDeferred = UtilsService.createPromiseDeferred();

                    genericFinancialAccountReadyPromiseDeferred.promise.then(function () {
                        var genericFinancialAccountPayload;

                        if (configuration != undefined) {
                            genericFinancialAccountPayload = configuration;
                        }
                        VRUIUtilsService.callDirectiveLoad(eedFieldSelectorAPI, genericFinancialAccountPayload, genericFinancialAccountDirectiverLoadDeferred);
                    });

                    return eedFieldSelectorLoadDeferred.promise;
                }
            }
        }

        return directiveDefinitionObject;
    }
]);