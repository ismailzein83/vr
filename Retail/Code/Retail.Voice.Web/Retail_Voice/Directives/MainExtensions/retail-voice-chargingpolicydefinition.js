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
            templateUrl: "/Client/Modules/Retail_Voice/Directives/MainExtensions/Templates/VoiceChargingPolicyDefinitionTemplate.html"

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
                        var payloadDirective =chargingPolicy !=undefined? { parts: chargingPolicy.PartsByTypeId }:undefined;
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
                    var directiveData = directiveAPI.getData();
                    var data = {
                        $type: "Retail.Voice.Entities.VoiceChargingPolicyDefinitionSettings, Retail.Voice.Entities",
                        PartsByTypeId: directiveData
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailVoiceChargingpolicydefinition', VoiceChargingpolicydefinitionDirective);

})(app);