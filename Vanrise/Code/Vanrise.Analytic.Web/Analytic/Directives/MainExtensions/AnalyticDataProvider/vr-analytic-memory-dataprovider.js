(function (app) {

    'use strict';

    MemoryDataProviderDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function MemoryDataProviderDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var memoryDataProvider = new MemoryDataProvider($scope, ctrl, $attrs);
                memoryDataProvider.initializeController();
            },
            controllerAs: "memCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AnalyticDataProvider/Templates/MemoryAnalyticDataProviderTemplate.html"

        };
        function MemoryDataProvider($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.analyticDataProviderSettings != undefined)
                            $scope.scopeModel.dataManager = payload.analyticDataProviderSettings.DataManager != undefined ? UtilsService.serializetoJson(payload.analyticDataProviderSettings.DataManager) : undefined;
                    }
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Business.MemoryAnalyticDataProvider, Vanrise.Analytic.Business",
                        DataManager: $scope.scopeModel.dataManager != undefined ? UtilsService.parseStringToJson($scope.scopeModel.dataManager) : undefined
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticMemoryDataprovider', MemoryDataProviderDirective);

})(app);