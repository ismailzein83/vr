(function (app) {

    'use strict';

    RecordSearchAnalyticReportDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function RecordSearchAnalyticReportDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var recordSearchAnalyticReport = new RecordSearchAnalyticReport($scope, ctrl, $attrs);
                recordSearchAnalyticReport.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/RecordSearch/Templates/RecordSearchAnalyticReportTemplates.html"

        };
        function RecordSearchAnalyticReport($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var sourceAPI;
            var sourceReadyDeferred = UtilsService.createPromiseDeferred();



            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.numberOfRecords = 100;
                $scope.scopeModel.onDataRecordSourceReady = function (api) {
                    sourceAPI = api;
                    sourceReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var reportSettings;
                    var promises = [];
                    if (payload != undefined && payload.reportSettings != undefined) {
                        reportSettings = payload.reportSettings;
                        if (reportSettings.NumberOfRecords != undefined)
                            $scope.scopeModel.numberOfRecords = reportSettings.NumberOfRecords;

                       $scope.scopeModel.maxNumberOfRecords = reportSettings.MaxNumberOfRecords;
                        var loadSourcePromiseDeferred = UtilsService.createPromiseDeferred();
                        sourceReadyDeferred.promise.then(function () {

                            var payLoad;
                            if (reportSettings != undefined) {
                                payLoad = {
                                    sources: reportSettings.Sources
                                }
                            }

                            VRUIUtilsService.callDirectiveLoad(sourceAPI, payLoad, loadSourcePromiseDeferred);
                        });
                        promises.push(loadSourcePromiseDeferred.promise);
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                api.getData = function () {

                    var viewSettings = {
                        $type: "Vanrise.Analytic.Entities.DataRecordSearchPageSettings, Vanrise.Analytic.Entities",
                        Sources: sourceAPI != undefined ? sourceAPI.getData() : undefined,
                        MaxNumberOfRecords: $scope.scopeModel.maxNumberOfRecords,
                        NumberOfRecords: $scope.scopeModel.numberOfRecords,
                    };

                    return viewSettings;
                };
            }
        }
    }

    app.directive('vrAnalyticAnalyticreportRecordsearchDefinition', RecordSearchAnalyticReportDirective);
})(app);