(function (app) {

    'use strict';

    VoiceChargingPolicyEvaluatorDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_BE_VoiceChargingPolicyAPIService'];

    function VoiceChargingPolicyEvaluatorDirective(UtilsService, VRUIUtilsService, Retail_BE_VoiceChargingPolicyAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var voiceChargingPolicyEvaluator = new VoiceChargingPolicyEvaluator($scope, ctrl, $attrs);
                voiceChargingPolicyEvaluator.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Voice/Directives/VoiceChargingPolicyEvaluator/Templates/VoiceChargingPolicyEvaluatorTemplate.html'
        };

        function VoiceChargingPolicyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var voiceChargingPolicyEvaluator;

                    if (payload != undefined) {
                        voiceChargingPolicyEvaluator = payload.voiceChargingPolicyEvaluator;
                    }

                    if (voiceChargingPolicyEvaluator != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getVoiceChargingPolicyEvaluatorTemplateConfigsPromise = getVoiceChargingPolicyEvaluatorTemplateConfigs();
                    promises.push(getVoiceChargingPolicyEvaluatorTemplateConfigsPromise);

                    function getVoiceChargingPolicyEvaluatorTemplateConfigs() {
                        return Retail_BE_VoiceChargingPolicyAPIService.GetVoiceChargingPolicyEvaluatorTemplateConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (voiceChargingPolicyEvaluator != undefined && voiceChargingPolicyEvaluator.ConfigId != null) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, voiceChargingPolicyEvaluator.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { voiceChargingPolicyEvaluator: voiceChargingPolicyEvaluator };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailVoiceVoicechargingpolicyevaluator', VoiceChargingPolicyEvaluatorDirective);

})(app);