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
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function VoiceChargingpolicydefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onPartsDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var voiceChargingPolicy;
                    if (payload != undefined) {
                        voiceChargingPolicy = payload.voiceChargingPolicy;
                    }
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();
                    var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    directiveReadyDeferred.promise.then(function () {
                        var payloadDirective = voiceChargingPolicy;
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
                    var data;
                    return data;
                }
            }
        }
    }

    app.directive('retailVoiceChargingpolicydefinition', VoiceChargingpolicydefinitionDirective);

})(app);