"use strict";

app.directive("retailDemoInvoicetypeSmsinvoicesettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "retail_SMS_InvoiceType",
    function (UtilsService, VRNotificationService, VRUIUtilsService, retail_SMS_InvoiceType) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new NetworkRentalInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Demo/Elements/SMS/Directives/Templates/SMSInvoiceSettingsTemplate.html"

        };

        function NetworkRentalInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.invoiceType = UtilsService.getArrayEnum(retail_SMS_InvoiceType);
                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    promises.push(getBusinessEntityDefinitionSelectorLoadPromise());
                    if (payload != undefined && payload.extendedSettingsEntity != undefined) { $scope.scopeModel.selectedInvoiceType = UtilsService.getItemByVal($scope.scopeModel.invoiceType, payload.extendedSettingsEntity.Type, "value"); }

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

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    return {
                        $type: "Retail.Demo.Business.SMSInvoiceSettings, Retail.Demo.Business",
                        AccountBEDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
                        Type: $scope.scopeModel.selectedInvoiceType != undefined ? $scope.scopeModel.selectedInvoiceType.value : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);