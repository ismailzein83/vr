dailyReportController.$inject = ['$scope', 'WhS_Analytics_GenericAnalyticDimensionEnum', 'WhS_Analytics_GenericAnalyticMeasureEnum', 'VRNotificationService', 'UtilsService','VRUIUtilsService'];

function dailyReportController($scope, WhS_Analytics_GenericAnalyticDimensionEnum, WhS_Analytics_GenericAnalyticMeasureEnum, VRNotificationService, UtilsService, VRUIUtilsService) {

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
        $scope.onReadyGenericGrid = function (api) {
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
            WithSummary: false,
        };
        return query;
    }


    function load() {
        $scope.isLoadingFilter = true;
        return UtilsService.waitMultipleAsyncOperations([loadFilterSection, loadMeasures]).catch(function (error) {
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
        dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value);
        dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Customer.value);
        dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Supplier.value);
        return dimensions;
    }
    function loadPayload() {
        var payload = {};
        payload.dimensions = loadDimensions();

        payload.filters = [];
        payload.filters.push(WhS_Analytics_GenericAnalyticDimensionEnum.Customer.value);
        payload.filters.push(WhS_Analytics_GenericAnalyticDimensionEnum.Supplier.value);
        payload.filters.push(WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value);
        return payload;
    }

    function loadMeasures() {
        measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.Attempts.value);
        measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.SuccessfulAttempts.value);
        measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.DurationsInSeconds.value);
        measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.ASR.value);
        measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.ACD.value);
        measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.PDDInSeconds.value);
        measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.CostRate.value);
        measures.push(WhS_Analytics_GenericAnalyticMeasureEnum.SaleRate.value);
       
    }

}

appControllers.controller('WhS_Analytics_DailyReportController', dailyReportController);