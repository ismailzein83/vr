"use strict";

app.directive("retailBillingInvoicetypesettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new BillingInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/Templates/BillingInvoiceTypeSettingsTemplate.html"

        };

        function BillingInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var vatRuleDefinitionId;

            var genericInvoiceSettingsDirectiveAPI;
            var genericInvoiceSettingsDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            var ruleDefinitionSelectorAPI;
            var ruleDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGenericInvoiceSettingsReady = function (api) {
                    genericInvoiceSettingsDirectiveAPI = api;
                    genericInvoiceSettingsDirectivePromiseDeferred.resolve();
                };

                $scope.scopeModel.onRuleDefinitionSelectorReady = function (api) {
                    ruleDefinitionSelectorAPI = api;
                    ruleDefinitionSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.extendedSettingsEntity != undefined) {
                        vatRuleDefinitionId = payload.extendedSettingsEntity.VatRuleDefinitionId;
                    }

                    promises.push(loadGenericInvoiceSettingsDirective());

                    promises.push(loadRuleDefinitionSelector());

                    function loadGenericInvoiceSettingsDirective() {
                        var genericInvoiceSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        genericInvoiceSettingsDirectivePromiseDeferred.promise.then(function () {
                            var directivePayload;
                            if (payload != undefined && payload.extendedSettingsEntity != undefined) {
                                directivePayload = payload.extendedSettingsEntity;
                            }
                            VRUIUtilsService.callDirectiveLoad(genericInvoiceSettingsDirectiveAPI, directivePayload, genericInvoiceSettingsDirectiveLoadDeferred);
                        });
                        return genericInvoiceSettingsDirectiveLoadDeferred.promise;
                    }

                    function loadRuleDefinitionSelector() {
                        var ruleDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

                        ruleDefinitionSelectorReadyDeferred.promise.then(function () {
                            var ruleDefinitionPayload = {
                                selectedIds: vatRuleDefinitionId
                            };

                            VRUIUtilsService.callDirectiveLoad(ruleDefinitionSelectorAPI, ruleDefinitionPayload, ruleDefinitionLoadDeferred);
                        });
                        return ruleDefinitionLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    var obj = {
                        $type: "Retail.Billing.Business.BillingInvoiceSettings, Retail.Billing.Business",
                        VatRuleDefinitionId: ruleDefinitionSelectorAPI.getSelectedIds()
                    };

                    genericInvoiceSettingsDirectiveAPI.setData(obj);
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);