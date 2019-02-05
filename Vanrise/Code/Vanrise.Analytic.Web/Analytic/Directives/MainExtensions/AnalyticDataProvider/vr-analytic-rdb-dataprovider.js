(function (app) {

    'use strict';

    RDBDataProviderDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function RDBDataProviderDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var sqlDataProvider = new SqlDataProvider($scope, ctrl, $attrs);
                sqlDataProvider.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AnalyticDataProvider/Templates/RDBAnalyticDataProviderTemplate.html"

        };
        function SqlDataProvider($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Data.SQL.SQLAnalyticDataProvider, Vanrise.Analytic.Data.SQL"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticRDBDataprovider', RDBDataProviderDirective);

})(app);