(function (app) {

    'use strict';

    SwitchIntegrationRadius.$inject = ["UtilsService", 'VRUIUtilsService'];

    function SwitchIntegrationRadius(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var radiusIntegration = new RadiusIntegration($scope, ctrl, $attrs);
                radiusIntegration.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/SwitchIntegrations/Templates/RetailBESwitchIntegrationRadiusTemplate.html"

        };
        function RadiusIntegration($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var mainPayload;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.switchSettings != undefined) {
                        $scope.scopeModel.connectionString = payload.switchSettings.ConnectionString;
                        $scope.scopeModel.tableName = payload.switchSettings.TableName;
                        $scope.scopeModel.mappingLogic = payload.switchSettings.MappingLogic;
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.SwitchIntegrations.Radius,Retail.BusinessEntity.MainExtensions",
                        ConnectionString: $scope.scopeModel.connectionString,
                        TableName: $scope.scopeModel.tableName,
                        MappingLogic: $scope.scopeModel.mappingLogic,
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailBeSwitchintegrationsRadius', SwitchIntegrationRadius);

})(app);