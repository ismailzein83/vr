"use strict";

app.directive("retailBillingInvoicetypesettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_Interconnect_InvoiceType",
    function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_Interconnect_InvoiceType) {

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

            var genericInvoiceSettingsDirectiveAPI;
            var genericInvoiceSettingsDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGenericInvoiceSettingsReady = function (api) {
                    genericInvoiceSettingsDirectiveAPI = api;
                    genericInvoiceSettingsDirectivePromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    promises.push(getGenericInvoiceSettingsDirectiveLoadPromise());

                    function getGenericInvoiceSettingsDirectiveLoadPromise() {
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

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    var obj = {
                        $type: "Retail.Billing.Business.BillingInvoiceSettings, Retail.Billing.Business",
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