(function (app) {

    'use strict';

    VoiceServiceDirective.$inject = ["UtilsService", 'VRUIUtilsService','Retail_BE_ChargeTypeEnum'];

    function VoiceServiceDirective(UtilsService, VRUIUtilsService, Retail_BE_ChargeTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var voiceService = new VoiceService($scope, ctrl, $attrs);
                voiceService.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Package/Service/Templates/VoiceServiceTemplate.html"

        };
        function VoiceService($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var voiceTypeDirectiveAPI;
            var voiceTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.chargeTypes = UtilsService.getArrayEnum(Retail_BE_ChargeTypeEnum);
                $scope.scopeModel.selectedChargeType = Retail_BE_ChargeTypeEnum.PerMinute;
                $scope.scopeModel.onVoiceTypeDirectiveReady = function (api) {
                    voiceTypeDirectiveAPI = api;
                    voiceTypeReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;
                    if (payload != undefined && payload.serviceSettings != undefined && payload.serviceSettings.VoiceType!=undefined)
                        $scope.scopeModel.selectedChargeType = UtilsService.getItemByVal($scope.scopeModel.chargeTypes, payload.serviceSettings.ChargeType, "value");

                    var voiceTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    voiceTypeReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = {
                                voiceType: payload != undefined ? payload.serviceSettings.VoiceType : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(voiceTypeDirectiveAPI, directivePayload, voiceTypeLoadPromiseDeferred);
                        });
                    return voiceTypeLoadPromiseDeferred.promise;

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.Package.VoiceService,Retail.BusinessEntity.MainExtensions",
                        VoiceType: voiceTypeDirectiveAPI.getData(),
                        ChargeType: $scope.scopeModel.selectedChargeType.value
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBePackageServiceVoice', VoiceServiceDirective);

})(app);