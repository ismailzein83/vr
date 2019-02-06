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

            var rdbTableSelectorAPI;
            var rdbTableReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var rdbTableSelectorSelectionChangedDeferred;

            function initializeController() {
                $scope.scopeModel = {};

                //$scope.scopeModel.onRDBTableSelectorReady = function (api) {
                //    rdbTableSelectorAPI = api;
                //    rdbTableReadyPromiseDeferred.resolve();
                //};

                //$scope.scopeModel.onRDBTableSelectionChanged = function (dataItem) {
                //    if (dataItem != undefined) {
                //        if (rdbTableSelectorSelectionChangedDeferred != undefined) {
                //            rdbTableSelectorSelectionChangedDeferred.resolve();
                //        }
                //    }
                //    else {
                //        ///////
                //    }
                //};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    //    var isEditMode;
                    //    var rdbAnalyticDataProviderTable
                    //    if (payload != undefined && payload.analyticDataProviderSettings != undefined) {
                    //        $scope.scopeModel.moduleName = payload.analyticDataProviderSettings.ModuleName;
                    //        rdbAnalyticDataProviderTable = payload.analyticDataProviderSettings.RDBAnalyticDataProviderTable;

                    //        isEditMode = true;
                    //        rdbTableSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                    //    }

                    //    //Loading RDB Table selector
                    //    var rdbTableSelectorLoadPromise = getRdbTableSelectorLoadPromise();
                    //    promises.push(rdbTableSelectorLoadPromise);

                    //    if (isEditMode) {
                    //       // var analyticTableSelectorLoadPromise = getAnalyticTableSelectorLoadPromise();
                    //        //promises.push(analyticTableSelectorLoadPromise);
                    //    }

                    //    function getRdbTableSelectorLoadPromise() {
                    //        var rdbTableSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    //        rdbTableReadyPromiseDeferred.promise.then(function () {

                    //            var rdbTableSelectorPayload = {
                    //                filter: {
                    //                    Filters: [{
                    //                        $type: "Vanrise.GenericData.RDBDataStorage.RDBDataRecordStorageFilter, Vanrise.GenericData.RDBDataStorage"
                    //                    }]
                    //                }
                    //            };
                    //            VRUIUtilsService.callDirectiveLoad(rdbTableSelectorAPI, rdbTableSelectorPayload, rdbTableSelectorLoadDeferred);
                    //        });

                    //        return rdbTableSelectorLoadDeferred.promise;
                    //    }
                    //};

                    //api.getData = function () {
                    //    var data = {
                    //        $type: "Vanrise.Analytic.Data.RDB.RDBAnalyticDataProvider, Vanrise.Analytic.Data.RDB",
                    //        ModuleName: $scope.scopeModel.moduleName,
                    //        //Table: 
                    //    };
                    //    return data;
                    //};

                    //if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    //    ctrl.onReady(api);
                    //}
                }
            }
        }
    }
    app.directive('vrAnalyticRdbDataprovider', RDBDataProviderDirective);

})(app);