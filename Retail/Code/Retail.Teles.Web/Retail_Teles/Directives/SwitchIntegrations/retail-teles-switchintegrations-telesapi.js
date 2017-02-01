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
            templateUrl: "/Client/Modules/Retail_Teles/Directives/SwitchIntegrations/Templates/SwitchIntegrationTelesAPITemplate.html"

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
                        $type: "Retail.Teles.Business.SwitchIntegrations.TelesAPISwitchIntegration,Retail.Teles.Business",
                        Authorization: $scope.scopeModel.authorization,
                        Token: $scope.scopeModel.token,
                        URL:$scope.scopeModel.url,
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesSwitchintegrationsTelesapi', SwitchIntegrationTelesAPI);

})(app);