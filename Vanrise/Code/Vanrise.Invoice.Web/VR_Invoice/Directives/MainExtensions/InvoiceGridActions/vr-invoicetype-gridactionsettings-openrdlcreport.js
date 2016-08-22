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
            templateUrl: "/Client/Modules/VR_Invoice/Directives/MainExtensions/InvoiceGridActions/Templates/OpenRDLCReportActionTemplate.html"

        };

        function OpenRDLCReportAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var dataSourceDirectiveAPI;
            var dataSourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var parameterDirectiveAPI;
            var parameterDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onDataSourcesDirectiveReady = function(api)
                {
                    dataSourceDirectiveAPI = api;
                    dataSourceDirectiveReadyPromiseDeferred.resolve();
                }
                $scope.scopeModel.onParametersDirectiveReady = function (api) {
                    parameterDirectiveAPI = api;
                    parameterDirectiveReadyPromiseDeferred.resolve();
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
                    var dataSourceLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    dataSourceDirectiveReadyPromiseDeferred.promise.then(function () {
                        var dataSourceDirectivePayload = payload != undefined ? { dataSources: payload.DataSources } : undefined;
                        VRUIUtilsService.callDirectiveLoad(dataSourceDirectiveAPI, dataSourceDirectivePayload, dataSourceLoadPromiseDeferred);
                    });
                    promises.push(dataSourceLoadPromiseDeferred.promise);

                    var parameterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    parameterDirectiveReadyPromiseDeferred.promise.then(function () {
                        var parameterDirectivePayload = payload != undefined ? { parameters: payload.Parameters } : undefined;
                        VRUIUtilsService.callDirectiveLoad(parameterDirectiveAPI, parameterDirectivePayload, parameterLoadPromiseDeferred);
                    });
                    promises.push(parameterLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.OpenRDLCReportAction ,Vanrise.Invoice.MainExtensions",
                        ReportURL: $scope.scopeModel.reportURL,
                        DataSources: dataSourceDirectiveAPI.getData(),
                        Parameters:parameterDirectiveAPI.getData()
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);