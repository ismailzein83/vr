(function (app) {

    'use strict';

    VoiceServiceTypeDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'Retail_BE_PricingPackageSettingsService'];

    function VoiceServiceTypeDirective(UtilsService, VRUIUtilsService, VRNotificationService, Retail_BE_PricingPackageSettingsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var voiceServiceTypeCtor = new VoiceServiceTypeCtor($scope, ctrl);
                voiceServiceTypeCtor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_Voice/Directives/VoiceServiceType/Templates/VoiceServiceTypeTemplate.html'
        };

        function VoiceServiceTypeCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var voiceChargingPolicyExaluatorDirectiveAPI;
            var voiceChargingPolicyExaluatorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onVoiceChargingPolicyExaluatorReady = function (api) {
                    voiceChargingPolicyExaluatorDirectiveAPI = api;
                    voiceChargingPolicyExaluatorDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var voiceServiceType;
                    if (payload != undefined) {
                        voiceServiceType = payload.extendedSettings;
                    }

                    var voiceChargingPolicyEvaluatorLoadPromise = getVoiceChargingPolicyEvaluatorLoadPromise();
                    promises.push(voiceChargingPolicyEvaluatorLoadPromise);


                    function getVoiceChargingPolicyEvaluatorLoadPromise() {

                        var voiceChargingPolicyExaluatorDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        voiceChargingPolicyExaluatorDirectiveReadyDeferred.promise.then(function () {

                            var voiceChargingPolicyEvaluatorPayload;
                            if (voiceServiceType != undefined) {
                                voiceChargingPolicyEvaluatorPayload = { voiceChargingPolicyEvaluator: voiceServiceType.VoiceChargingPolicyEvaluator };
                            }
                            VRUIUtilsService.callDirectiveLoad(voiceChargingPolicyExaluatorDirectiveAPI, voiceChargingPolicyEvaluatorPayload, voiceChargingPolicyExaluatorDirectiveLoadDeferred);
                        });

                        return voiceChargingPolicyExaluatorDirectiveLoadDeferred.promise
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "Retail.Voice.Entities.VoiceServiceType, Retail.Voice.Entities",
                        VoiceChargingPolicyEvaluator: voiceChargingPolicyExaluatorDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailVoiceVoiceservicetype', VoiceServiceTypeDirective);

})(app);