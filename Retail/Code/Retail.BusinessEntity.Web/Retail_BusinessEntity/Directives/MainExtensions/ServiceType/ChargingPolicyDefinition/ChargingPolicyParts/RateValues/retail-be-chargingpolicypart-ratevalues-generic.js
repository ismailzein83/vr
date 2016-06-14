(function (app) {

    'use strict';

    ChargingpolicypartRatevaluesGenericDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function ChargingpolicypartRatevaluesGenericDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var chargingpolicypartRatevaluesGeneric = new ChargingpolicypartRatevaluesGeneric($scope, ctrl, $attrs);
                chargingpolicypartRatevaluesGeneric.initializeController();
            },
            controllerAs: "genericratevalueCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ServiceType/ChargingPolicyDefinition/ChargingPolicyParts/RateValues/Templates/GenericRateValuesTemplate.html"

        };

        function ChargingpolicypartRatevaluesGeneric($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var directiveAPI;
            var directiveReadyDeferred= UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.onRuleDefinitionCriteriaReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var ruleDefinition;
                    if (payload != undefined) {
                        ruleDefinition = payload.RuleCriteriaDefinition;
                    }
                    var promises = [];
                    var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    directiveReadyDeferred.promise.then(function () {
                        var payloadDirective = ruleDefinition != undefined ? { GenericRuleDefinitionCriteriaFields: ruleDefinition.Fields } : undefined;
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                    });
                    promises.push(loadDirectivePromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.RateValues.GenericRuleRateValueDefinition,Retail.BusinessEntity.MainExtensions",
                        RuleCriteriaDefinition: directiveAPI.getData()
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeChargingpolicypartRatevaluesGeneric', ChargingpolicypartRatevaluesGenericDirective);

})(app);