(function (app) {

    'use strict';

    AnalyticitemactionOpenrecordsearchDirective.$inject = ["UtilsService", 'VRUIUtilsService','VR_Analytic_AnalyticReportAPIService'];

    function AnalyticitemactionOpenrecordsearchDirective(UtilsService, VRUIUtilsService,VR_Analytic_AnalyticReportAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var analyticitemactionOpenrecordsearch = new AnalyticitemactionOpenrecordsearch($scope, ctrl, $attrs);
                analyticitemactionOpenrecordsearch.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticItemAction/Templates/OpenRecordSearchItemActionTemplate.html"

        };
        function AnalyticitemactionOpenrecordsearch($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            var selectedReportPromiseDeferred;
            var directiveAPI;
            var selectorAPI;
            var mainPayload;
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.sources = [];
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };
                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                };
                $scope.scopeModel.onReportSelectionChanged = function () {
                    var selectedReportId = directiveAPI.getSelectedIds();
                    if (selectedReportId != undefined) {
                        if (selectedReportPromiseDeferred != undefined) {
                            selectedReportPromiseDeferred.resolve();
                        } else {
                            $scope.scopeModel.isLoadingSources = true;
                            getReportDefinition(selectedReportId).then(function (reportEntity) {
                                $scope.scopeModel.isLoadingSources = false;
                            });
                        }
                    }
                };
                defineAPI();
            }
            function getReportDefinition(reportId)
            {
                return VR_Analytic_AnalyticReportAPIService.GetAnalyticReportById(reportId).then(function (reportEntity) {
                    if (selectorAPI != undefined) {
                        selectorAPI.clearDataSource();
                    }
                    if (reportEntity && reportEntity.Settings)
                    {
                                
                        for(var i=0;i<reportEntity.Settings.Sources.length;i++)
                        {
                            var source = reportEntity.Settings.Sources[i];
                            $scope.scopeModel.sources.push({ Name: source.Name, Title: source.Title });
                        }
                        
                    }
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                        mainPayload = payload;
                        var promises = [];
                    
                        if (payload != undefined && payload.ReportId != undefined) {
                            selectedReportPromiseDeferred = UtilsService.createPromiseDeferred();
                        }


                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred.promise.then(function () {
                            var payloadDirective = { filter: { TypeName: "VR_Analytic_Report_RecordSearch" }, selectedIds: payload  !=undefined?payload.ReportId:undefined};
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, directiveLoadDeferred);
                           
                        });
                        promises.push(directiveLoadDeferred.promise);

                        if (payload != undefined && payload.ReportId != undefined)
                        {
                           
                           var  loadReportPromiseDeferred = UtilsService.createPromiseDeferred();
                           selectedReportPromiseDeferred.promise.then(function () {

                               getReportDefinition(payload.ReportId).then(function () {
                                   if (mainPayload != undefined && mainPayload.SourceName != undefined) {
                                       $scope.scopeModel.selectedSource = UtilsService.getItemByVal($scope.scopeModel.sources, mainPayload.SourceName, "Name");
                                   }
                                   loadReportPromiseDeferred.resolve();
                                   selectedReportPromiseDeferred = undefined;
                               });
                           });
                           
                            promises.push(loadReportPromiseDeferred.promise);
                        }
                        return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.AnalyticItemAction.OpenRecordSearchItemAction, Vanrise.Analytic.MainExtensions ",
                        ReportId: directiveAPI.getSelectedIds(),
                        SourceName: $scope.scopeModel.selectedSource.Name
                    };
                    return data;
                }
            }
        }
    }

    app.directive('vrAnalyticAnalyticitemactionOpenrecordsearch', AnalyticitemactionOpenrecordsearchDirective);

})(app);