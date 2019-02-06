(function (app) {

    'use strict';

    RdbAnalyticDataproviderTableSelector.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RdbAnalyticDataproviderTableSelector(VR_Analytic_AnalyticConfigurationAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var analyticDataProviderSettings = new AnalyticDataProviderSettings($scope, ctrl, $attrs);
                analyticDataProviderSettings.initializeController();
            },
            controllerAs: "providerCtrl",
            bindToController: true
           // templateUrl: 
        };

        function AnalyticDataProviderSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }
    vrRdbAnalyticDataproviderTable

    app.directive('vrRdbAnalyticDataproviderTable', RdbAnalyticDataproviderTableSelector);

})(app);