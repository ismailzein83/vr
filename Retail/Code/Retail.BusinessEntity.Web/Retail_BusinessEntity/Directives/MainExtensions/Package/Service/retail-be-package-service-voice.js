(function (app) {

    'use strict';

    VoiceServiceDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function VoiceServiceDirective(UtilsService, VRUIUtilsService) {
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
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                        if (payload.serviceEntity != undefined) {
                            $scope.scopeModel.roamingSupport = payload.serviceEntity.RoamingSupport;
                            $scope.scopeModel.fractionUnit = payload.serviceEntity.FractionUnit;
                            $scope.scopeModel.durationPerUnit = payload.serviceEntity.DurationPerUnit;
                        }

                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.Package.VoiceService,Retail.BusinessEntity.MainExtensions",
                        RoamingSupport: $scope.scopeModel.roamingSupport,
                        FractionUnit: $scope.scopeModel.fractionUnit,
                        DurationPerUnit: $scope.scopeModel.durationPerUnit,
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailBePackageServiceVoice', VoiceServiceDirective);

})(app);