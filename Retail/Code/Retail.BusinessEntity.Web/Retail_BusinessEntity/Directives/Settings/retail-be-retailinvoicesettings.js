(function (app) {

    'use strict';

    RetailInvoiceSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function RetailInvoiceSettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Settings/Templates/RetailInvoiceSettingsTemplate.html"
        };


        function RetailInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var taxDefinitionDirectiveAPI;
            var taxDefinitionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTaxDefinitionDirectiveReady = function (api) {
                    taxDefinitionDirectiveAPI = api;
                    taxDefinitionDirectiveReadyDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    
                    var promises = [];

                    promises.push(loadTaxDefinitionDirective());

                    function loadTaxDefinitionDirective() {
                        var taxDefinitionDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        taxDefinitionDirectiveReadyDeferred.promise.then(function () {
                            var taxDefinitionDirectivePayload;
                            if (payload != undefined && payload.data != undefined)
                                taxDefinitionDirectivePayload = {
                                    TaxesDefinition: payload.data.VRTaxesDefinition
                                };

                            VRUIUtilsService.callDirectiveLoad(taxDefinitionDirectiveAPI, taxDefinitionDirectivePayload, taxDefinitionDirectiveLoadDeferred);
                        });
                        return taxDefinitionDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Entities.RetailInvoiceSettings,Retail.BusinessEntity.Entities",
                        VRTaxesDefinition: taxDefinitionDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeRetailinvoicesettings', RetailInvoiceSettingsDirective);
})(app);