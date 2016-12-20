(function (app) {

    'use strict';

    SwitchIntegrationTelesAPI.$inject = ["UtilsService", 'VRUIUtilsService'];

    function SwitchIntegrationTelesAPI(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var telesAPIIntegration = new TelesAPIIntegration($scope, ctrl, $attrs);
                telesAPIIntegration.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/SwitchIntegrations/Templates/SwitchIntegrationTelesAPITemplate.html"

        };
        function TelesAPIIntegration($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var mainPayload;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
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
                        $scope.scopeModel.authorization = switchIntegration.Authorization;
                        $scope.scopeModel.token = switchIntegration.Token;
                        $scope.scopeModel.url = switchIntegration.URL;
                    }
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.TelesAPISwitchIntegration,Retail.BusinessEntity.MainExtensions",
                        Authorization: $scope.scopeModel.authorization,
                        Token: $scope.scopeModel.token,
                        URL:$scope.scopeModel.url,
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeSwitchintegrationsTelesapi', SwitchIntegrationTelesAPI);

})(app);