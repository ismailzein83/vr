(function (app) {

    'use strict';

    BePackageServicesettingsVoiceNationalDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'Retail_BE_NetworkTypeEnum'];

    function BePackageServicesettingsVoiceNationalDirective(UtilsService, VRUIUtilsService, Retail_BE_NetworkTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var packageServicesettingsVoiceNational = new PackageServicesettingsVoiceNational($scope, ctrl, $attrs);
                packageServicesettingsVoiceNational.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Package/Service/VoiceType/Templates/NationalVoiceTypeTemplate.html"

        };
        function PackageServicesettingsVoiceNational($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.networkTypes = UtilsService.getArrayEnum(Retail_BE_NetworkTypeEnum);
                $scope.scopeModel.selectedNetworkType = Retail_BE_NetworkTypeEnum.OnNet;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                        if (payload.voiceType != undefined) {
                            $scope.scopeModel.selectedNetworkType = UtilsService.getItemByVal($scope.scopeModel.networkTypes, payload.voiceType.NetworkType, "value");
                        }

                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.Package.NationalVoiceType,Retail.BusinessEntity.MainExtensions",
                        NetworkType: $scope.scopeModel.selectedNetworkType.value
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBePackageServicesettingsVoiceNational', BePackageServicesettingsVoiceNationalDirective);

})(app);