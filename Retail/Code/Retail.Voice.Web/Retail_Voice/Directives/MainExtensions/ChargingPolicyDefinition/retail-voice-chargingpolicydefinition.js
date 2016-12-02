(function (app) {

    'use strict';

    VoiceChargingpolicydefinitionDirective.$inject = [ 'UtilsService', 'VRUIUtilsService'];

    function VoiceChargingpolicydefinitionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var voiceChargingpolicydefinition = new VoiceChargingpolicydefinition($scope, ctrl, $attrs);
                voiceChargingpolicydefinition.initializeController();
            },
            controllerAs: "voicechargingpolicyCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Voice/Directives/MainExtensions/ChargingPolicyDefinition/Templates/VoiceChargingPolicyDefinitionTemplate.html"

        };
        function VoiceChargingpolicydefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            var ruleDefinitionEditorAPI;
            var ruleDefinitionEditorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onPartsDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onRuleDefinitionEditorReady = function (api) {
                    ruleDefinitionEditorAPI = api;
                    ruleDefinitionEditorReadyDeferred.resolve();
                }
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
                    var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    directiveReadyDeferred.promise.then(function () {
                        var payloadDirective = chargingPolicy != undefined ? { parts: chargingPolicy.PartDefinitions } : undefined;
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                    });
                    promises.push(loadDirectivePromiseDeferred.promise);
               

                    var ruleDefinitionLoadDeferred = UtilsService.createPromiseDeferred();
                    ruleDefinitionEditorReadyDeferred.promise.then(function () {
                        var ruleDefinitionPayload;

                        if (chargingPolicy != undefined) {
                            ruleDefinitionPayload = { ruleDefinitions: chargingPolicy.RuleDefinitions };
                        }

                        VRUIUtilsService.callDirectiveLoad(ruleDefinitionEditorAPI, ruleDefinitionPayload, ruleDefinitionLoadDeferred);
                    });
                    promises.push(ruleDefinitionLoadDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var directiveData = directiveAPI.getData();
                    var data = {
                        $type: "Retail.Voice.Entities.VoiceChargingPolicyDefinitionSettings, Retail.Voice.Entities",
                        PartDefinitions: directiveData,
                        RuleDefinitions: ruleDefinitionEditorAPI.getData()
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailVoiceChargingpolicydefinition', VoiceChargingpolicydefinitionDirective);

})(app);