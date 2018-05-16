"use strict";

app.directive("retailInvoiceInvoicetypeInterconnectinvoicesettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InterconnectInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Interconnect/Directives/Templates/InterconnectInvoiceSettingsTemplate.html"

        };

        function InterconnectInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var singleBillingTransactionTypeSelectorAPI;
            var singleBillingTransactionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var multiBillingTransactionTypeSelectorAPI;
            var multiBillingTransactionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSingleBillingTransactionTypeSelector = function (api) {
                    singleBillingTransactionTypeSelectorAPI = api;
                    singleBillingTransactionTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onMultiBillingTransactionTypeSelector = function (api) {
                    multiBillingTransactionTypeSelectorAPI = api;
                    multiBillingTransactionTypeSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    
                    promises.push(getBusinessEntityDefinitionSelectorLoadPromise());
                    promises.push(loadSingleBillingTransactionTypeSelector());
                    promises.push(loadMultiBillingTransactionTypeSelector());

                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };
                            if (payload != undefined && payload.extendedSettingsEntity != undefined) {
                                selectorPayload.selectedIds = payload.extendedSettingsEntity.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });
                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    function loadSingleBillingTransactionTypeSelector() {
                        var singleBillingTransactionTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        singleBillingTransactionTypeSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload;
                            if (payload != undefined && payload.extendedSettingsEntity != undefined) {
                                selectorPayload = {
                                    selectedIds: payload.extendedSettingsEntity.InvoiceTransactionTypeId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(singleBillingTransactionTypeSelectorAPI, selectorPayload, singleBillingTransactionTypeSelectorLoadDeferred);
                        });
                        return singleBillingTransactionTypeSelectorLoadDeferred.promise;
                    }

                    function loadMultiBillingTransactionTypeSelector() {
                        var multiBillingTransactionTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        multiBillingTransactionTypeSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload;
                            if (payload != undefined && payload.extendedSettingsEntity != undefined) {
                                selectorPayload = {
                                    selectedIds: payload.extendedSettingsEntity.UsageTransactionTypeIds
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(multiBillingTransactionTypeSelectorAPI, selectorPayload, multiBillingTransactionTypeSelectorLoadDeferred);
                        });
                        return multiBillingTransactionTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    return {
                        $type: "Retail.Interconnect.Business.InterconnectInvoiceSettings, Retail.Interconnect.Business",
                        AccountBEDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
                        InvoiceTransactionTypeId: singleBillingTransactionTypeSelectorAPI.getSelectedIds(),
                        UsageTransactionTypeIds: multiBillingTransactionTypeSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);