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

            function initializeController()
            {
                $scope.scopeModel = {};
                $scope.scopeModel.requestTypes = UtilsService.getArrayEnum(Retail_BE_WebServiceRequestTypeEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload)
                {
                    var switchIntegration;

                    if (payload != undefined) {
                        switchIntegration = payload.switchIntegration;
                    }

                    if (switchIntegration != undefined) {
                        $scope.scopeModel.url = switchIntegration.URL;
                        $scope.scopeModel.credentialLogic = switchIntegration.CredentialLogic;
                        $scope.scopeModel.mappingLogic = switchIntegration.MappingLogic;
                        $scope.scopeModel.selectedRequestType = UtilsService.getItemByVal($scope.scopeModel.requestTypes, switchIntegration.RequestType, 'value');
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
                        MappingLogic: $scope.scopeModel.mappingLogic
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeSwitchintegrationsWebservice', SwitchIntegrationsWebService);

})(app);