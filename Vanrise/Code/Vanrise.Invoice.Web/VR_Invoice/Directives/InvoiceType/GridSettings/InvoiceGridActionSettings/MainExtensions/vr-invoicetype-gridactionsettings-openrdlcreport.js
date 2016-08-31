"use strict";

app.directive("vrInvoicetypeGridactionsettingsOpenrdlcreport", ["UtilsService", "VRNotificationService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new OpenRDLCReportAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/GridSettings/InvoiceGridActionSettings/MainExtensions/Templates/OpenRDLCReportActionTemplate.html"

        };

        function OpenRDLCReportAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainReportDataSourcesDirectiveAPI;
            var mainReportDataSourcesDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var mainReportParametersDirectiveAPI;
            var mainReportParametersDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var subReportsDirectiveAPI;
            var subReportsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onMainReportDataSourcesDirectiveReady = function (api)
                {
                    mainReportDataSourcesDirectiveAPI = api;
                    mainReportDataSourcesDirectiveReadyPromiseDeferred.resolve();
                }
                $scope.scopeModel.onMainReportParametersDirectiveReady = function (api) {
                    mainReportParametersDirectiveAPI = api;
                    mainReportParametersDirectiveReadyPromiseDeferred.resolve();
                }
                $scope.scopeModel.onSubReportsDirectiveReady = function (api) {
                    subReportsDirectiveAPI = api;
                    subReportsDirectiveReadyPromiseDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined)
                    {
                        $scope.scopeModel.reportURL = payload.ReportURL
                    }
                    var promises = [];
                    var mainReportDataSourcesLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    mainReportDataSourcesDirectiveReadyPromiseDeferred.promise.then(function () {
                        var mainReportDataSourcesDirectivePayload = payload != undefined ? { dataSources: payload.MainReportDataSources } : undefined;
                        VRUIUtilsService.callDirectiveLoad(mainReportDataSourcesDirectiveAPI, mainReportDataSourcesDirectivePayload, mainReportDataSourcesLoadPromiseDeferred);
                    });
                    promises.push(mainReportDataSourcesLoadPromiseDeferred.promise);


                    var mainReportParametersLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    mainReportParametersDirectiveReadyPromiseDeferred.promise.then(function () {
                        var mainReportParametersDirectivePayload = payload != undefined ? { parameters: payload.MainReportParameters } : undefined;
                        VRUIUtilsService.callDirectiveLoad(mainReportParametersDirectiveAPI, mainReportParametersDirectivePayload, mainReportParametersLoadPromiseDeferred);
                    });
                    promises.push(mainReportParametersLoadPromiseDeferred.promise);

                    var subReportsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    subReportsDirectiveReadyPromiseDeferred.promise.then(function () {
                        var subReportsDirectivePayload = payload != undefined ? { subReports: payload.SubReports } : undefined;
                        VRUIUtilsService.callDirectiveLoad(subReportsDirectiveAPI, subReportsDirectivePayload, subReportsLoadPromiseDeferred);
                    });
                    promises.push(subReportsLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.OpenRDLCReportAction ,Vanrise.Invoice.MainExtensions",
                        ReportURL: $scope.scopeModel.reportURL,
                        MainReportDataSources: mainReportDataSourcesDirectiveAPI.getData(),
                        MainReportParameters: mainReportParametersDirectiveAPI.getData(),
                        SubReports: subReportsDirectiveAPI.getData()
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);