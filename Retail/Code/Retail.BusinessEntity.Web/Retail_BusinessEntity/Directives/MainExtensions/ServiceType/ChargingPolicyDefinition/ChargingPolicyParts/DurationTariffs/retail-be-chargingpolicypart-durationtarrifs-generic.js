(function (app) {

    'use strict';

    ChargingpolicypartDurationtarrifsGenericDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function ChargingpolicypartDurationtarrifsGenericDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var chargingpolicypartDurationtarrifsGeneric = new ChargingpolicypartDurationtarrifsGeneric($scope, ctrl, $attrs);
                chargingpolicypartDurationtarrifsGeneric.initializeController();
            },
            controllerAs: "generictarrifCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ServiceType/ChargingPolicyDefinition/ChargingPolicyParts/DurationTariffs/Templates/GenericDurationTariffsTemplate.html"

        };

        function ChargingpolicypartDurationtarrifsGeneric($scope, ctrl, $attrs) {
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
                        $type: "Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.DurationTariffs.GenericRuleDurationTariffDefinition,Retail.BusinessEntity.MainExtensions",
                        RuleCriteriaDefinition: directiveAPI.getData()
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeChargingpolicypartDurationtarrifsGeneric', ChargingpolicypartDurationtarrifsGenericDirective);

})(app);