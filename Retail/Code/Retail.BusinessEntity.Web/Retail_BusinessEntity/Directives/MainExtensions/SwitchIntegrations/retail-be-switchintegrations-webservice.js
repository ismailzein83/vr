(function (app) {

    'use strict';

    SwitchIntegrationsWebService.$inject = ["Retail_BE_WebServiceRequestTypeEnum", "UtilsService", 'VRUIUtilsService'];

    function SwitchIntegrationsWebService(Retail_BE_WebServiceRequestTypeEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var webServiceIntegration = new WebServiceIntegration($scope, ctrl, $attrs);
                webServiceIntegration.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/SwitchIntegrations/Templates/RetailBESwitchIntegrationWebServiceTemplate.html"

        };
        function WebServiceIntegration($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var mainPayload;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onRequestTypesReady = function () {
                    $scope.scopeModel.requestTypes = UtilsService.getArrayEnum(Retail_BE_WebServiceRequestTypeEnum);
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.switchSettings != undefined) {
                        $scope.scopeModel.url = payload.switchSettings.URL;
                        $scope.scopeModel.credentialLogic = payload.switchSettings.CredentialLogic;
                        $scope.scopeModel.mappingLogic = payload.switchSettings.MappingLogic;
                        $scope.scopeModel.selectedRequestType = payload.switchSettings.RequestType
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.SwitchIntegrations.WebService,Retail.BusinessEntity.MainExtensions",
                        URL: $scope.scopeModel.url,
                        RequestType: $scope.scopeModel.selectedRequestType.value,
                        CredentialLogic: $scope.scopeModel.credentialLogic,
                        MappingLogic: $scope.scopeModel.mappingLogic,
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailBeSwitchintegrationsWebservice', SwitchIntegrationsWebService);

})(app);