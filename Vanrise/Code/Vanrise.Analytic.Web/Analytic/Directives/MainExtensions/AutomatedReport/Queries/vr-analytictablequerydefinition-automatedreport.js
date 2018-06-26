"use strict";
app.directive("vrAnalytictablequerydefinitionAutomatedreport", ["UtilsService", "VR_GenericData_DataRecordStorageAPIService", "VRUIUtilsService",
function (UtilsService, VR_GenericData_DataRecordStorageAPIService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticTableQuery = new AnalyticTableQuery($scope, ctrl, $attrs);
            analyticTableQuery.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/Queries/Templates/AnalyticTableQueryDefinition.html"
    };


    function AnalyticTableQuery($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var analyticTableSelectorAPI;
        var analyticTableSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onAnalyticTableSelectorReady = function (api) {
                analyticTableSelectorAPI = api;
                analyticTableSelectorReadyDeferred.resolve();
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(getDirectiveAPI());
            }
        }

        function getDirectiveAPI() {

            var api = {};
            var analyticTableId;

            api.load = function (payload) {
                if (payload != undefined && payload.extendedSettings != undefined) {
                    analyticTableId = payload.extendedSettings.AnalyticTableId;
                }
                var promises = [];

                var loadAnalyticTableSelectorPromise = loadAnalyticTableSelector();
                promises.push(loadAnalyticTableSelectorPromise);

                function loadAnalyticTableSelector() {
                    var analyticTableSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    analyticTableSelectorReadyDeferred.promise.then(function () {
                        var payload = {
                            selectedIds: analyticTableId
                        };
                        VRUIUtilsService.callDirectiveLoad(analyticTableSelectorAPI, payload, analyticTableSelectorLoadDeferred);
                    });
                    return analyticTableSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.Queries.AnalyticTableQueryDefinitionSettings,Vanrise.Analytic.MainExtensions",
                    AnalyticTableId: analyticTableSelectorAPI.getSelectedIds(),
                };

            };

            return api;
        }
    }

    return directiveDefinitionObject;
}
]);