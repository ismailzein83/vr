(function (appControllers) {

    "use strict";

    vrAnalyticAnalyticReportController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService'];
    function vrAnalyticAnalyticReportController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService) {

        var chartApi;
        var gridApi;
        var filterDirectiveAPI;
        var filterReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var measures = [];
        var periods = [];
        var dimensions = [];

        defineScope();
        load();


        function defineScope() {


            $scope.onGridReady = function (api) {
                gridApi = api;
            };

            $scope.onFilterDirectivectiveReady = function (api) {
                filterDirectiveAPI = api;
                filterReadyPromiseDeferred.resolve();
            };

            $scope.searchClicked = function () {
                return UtilsService.waitMultipleAsyncOperations([loadGrid]).finally(function () {

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };
        }

        function loadGrid() {
            return gridApi.loadGrid(getQuery());
        }

        function getQuery() {
            var selectedObject = filterDirectiveAPI.getData();
            var query = {
                Filters: selectedObject.selectedfilters,
                DimensionFields: selectedObject.selecteddimensions,
                FixedDimensionFields: selectedObject.selectedperiod,
                MeasureFields: measures,
                Dimensions: dimensions,
                FromTime: selectedObject.fromdate,
                ToTime: selectedObject.todate,
                Currency: selectedObject.currency,
            };
            return query;
        }


        function load() {
            $scope.isLoadingFilter = true;
            return UtilsService.waitMultipleAsyncOperations([loadFilterSection, loadDimensions, loadMeasures]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
                $scope.isLoadingFilter = false;
            }).finally(function () {
                $scope.isLoadingFilter = false;
            });
        }

        function loadFilterSection() {
            var loadFilterPromiseDeferred = UtilsService.createPromiseDeferred();
            filterReadyPromiseDeferred.promise.then(function () {
                var payload = loadPayload();
                VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, payload, loadFilterPromiseDeferred);
            });
            return loadFilterPromiseDeferred.promise;
        }

        function loadDimensions() {

            return VR_Analytic_AnalyticConfigurationAPIService.GetDimensionsInfo().then(function (response) {
                dimensions.length = 0;
                angular.forEach(response, function (itm) {
                    dimensions.push(itm);
                });
            });
        }

        function loadPayload() {
            var payload = {};
            payload.dimensions = loadDimensions();
            payload.filters = [];
            payload.measureThresholds = [];

            return payload;
        }

        function loadMeasures() {
            return VR_Analytic_AnalyticConfigurationAPIService.GetMeasuresInfo().then(function (response) {
                measures.length = 0;
                angular.forEach(response, function (itm) {
                    measures.push(itm);
                });
            });
        }

    }
    appControllers.controller('VR_Analytic_AnalyticReportController', vrAnalyticAnalyticReportController);

})(appControllers);