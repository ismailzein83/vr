(function (app) {

    'use strict';

    DataChargingpolicydefinitionDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function DataChargingpolicydefinitionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataChargingpolicydefinition = new DataChargingpolicydefinition($scope, ctrl, $attrs);
                dataChargingpolicydefinition.initializeController();
            },
            controllerAs: "datachargingpolicyCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Data/Directives/MainExtensions/ChargingPolicy/Templates/DataChargingPolicyDefinitionTemplate.html"
        };

        function DataChargingpolicydefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            var ruleDefinitionEditorAPI;
            var ruleDefinitionEditorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRuleDefinitionEditorReady = function (api) {
                    ruleDefinitionEditorAPI = api;
                    ruleDefinitionEditorReadyDeferred.resolve();
                };

                $scope.scopeModel.onPartsDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var chargingPolicy;

                    if (payload != undefined) {
                        chargingPolicy = payload.chargingPolicy;
                    }

                    var ruleDefinitionLoadDeferred = UtilsService.createPromiseDeferred();
                    ruleDefinitionEditorReadyDeferred.promise.then(function () {
                        var ruleDefinitionPayload;

                        if (chargingPolicy != undefined) {
                            ruleDefinitionPayload = { ruleDefinitions: chargingPolicy.RuleDefinitions };
                        }

                        VRUIUtilsService.callDirectiveLoad(ruleDefinitionEditorAPI, ruleDefinitionPayload, ruleDefinitionLoadDeferred);
                    });
                    promises.push(ruleDefinitionLoadDeferred.promise);

                    var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    directiveReadyDeferred.promise.then(function () {
                        var payloadDirective = chargingPolicy != undefined ? { parts: chargingPolicy.PartDefinitions } : undefined;
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
                        $type: "Retail.Data.Entities.DataChargingPolicyDefinitionSettings, Retail.Data.Entities",
                        RuleDefinitions: ruleDefinitionEditorAPI.getData(),
                        PartDefinitions: directiveAPI.getData()
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailDataChargingpolicydefinition', DataChargingpolicydefinitionDirective);
})(app);