(function (app) {

    'use strict';

    RDBDataProviderDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function RDBDataProviderDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var rdbDataProvider = new RDBDataProvider($scope, ctrl, $attrs);
                rdbDataProvider.initializeController();
            },
            controllerAs: "rdbCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AnalyticDataProvider/Templates/RDBAnalyticDataProviderTemplate.html"

        };
        function RDBDataProvider($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var rdbAnalyticDataProviderTable;

            var dataProviderTableSelectorAPI;
            var dataProviderTableReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataProviderTableSelectorReady = function (api) {
                    dataProviderTableSelectorAPI = api;
                    dataProviderTableReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};
               

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.analyticDataProviderSettings != undefined) {
                        $scope.scopeModel.moduleName = payload.analyticDataProviderSettings.ModuleName;
                        rdbAnalyticDataProviderTable = payload.analyticDataProviderSettings.Table;
                    }

                    var dataProviderTableSelectorLoadPromise = getDataProviderTableSelectorLoadPromise();
                    promises.push(dataProviderTableSelectorLoadPromise);

                    function getDataProviderTableSelectorLoadPromise() {
                        var dataProviderTableSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataProviderTableReadyPromiseDeferred.promise.then(function () {

                            var dataProviderTableSelectorPayload = {
                                analyticDataProviderTable: rdbAnalyticDataProviderTable
                            };
                            VRUIUtilsService.callDirectiveLoad(dataProviderTableSelectorAPI, dataProviderTableSelectorPayload, dataProviderTableSelectorLoadDeferred);
                        });

                        return dataProviderTableSelectorLoadDeferred.promise;
                    }
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Analytic.Data.RDB.RDBAnalyticDataProvider, Vanrise.Analytic.Data.RDB",
                        ModuleName: $scope.scopeModel.moduleName,
                        Table: dataProviderTableSelectorAPI.getData()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticRdbDataprovider', RDBDataProviderDirective);

})(app);