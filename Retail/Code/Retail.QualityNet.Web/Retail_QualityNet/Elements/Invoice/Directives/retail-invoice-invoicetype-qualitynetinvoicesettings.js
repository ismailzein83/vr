"use strict";

app.directive("retailInvoiceInvoicetypeQualitynetinvoicesettings", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new QualityNetInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_QualityNet/Elements/Invoice/Directives/Templates/QualityNetInvoiceSettingsTemplate.html"
        };

        function QualityNetInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    promises.push(getBusinessEntityDefinitionSelectorLoadPromise());

                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorReadyPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined && payload.extendedSettingsEntity != undefined)
                                selectorPayload.selectedIds = payload.extendedSettingsEntity.AccountBEDefinitionId;

                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });
                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    return {
                        $type: "Retail.QualityNet.Business.QualityNetInvoiceTypeSettings,Retail.QualityNet.Business",
                        AccountBEDefinitionId: beDefinitionSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);