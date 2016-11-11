(function (appControllers) {

    'use strict';

    DealProgressManagementController.$inject = ['$scope', 'WhS_BE_DealService', 'WhS_BE_DealProgressAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'PeriodEnum'];

    function DealProgressManagementController($scope, WhS_BE_DealService, WhS_BE_DealProgressAPIService, UtilsService, VRUIUtilsService, VRNotificationService, PeriodEnum) {
        
        var sellingGridAPI;
        var buyingGridAPI;

        var sellingChartAPI;
        var buyingChartAPI;


        var timeRangeDirectiveAPI;
        var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var dealSelectorAPI;
        var dealSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        load();

        function defineScope() {
            $scope.showData = false;
            $scope.dataSourceSelling = [];
            $scope.dataSourceBuying = [];

            $scope.onTimeRangeDirectiveReady = function (api) {
                timeRangeDirectiveAPI = api;
                timeRangeReadyPromiseDeferred.resolve();
            };

            $scope.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };
            $scope.onDealSelectorReady = function (api) {
                dealSelectorAPI = api;
                dealSelectorReadyDeferred.resolve();
            };
            
            $scope.onSellingGridReady = function (api) {
                sellingGridAPI = api;
            };
            $scope.onBuyingGridReady = function (api) {
                buyingGridAPI = api;
            };
            $scope.onSellingChartReady = function (api) {
                sellingChartAPI = api;
            };
            $scope.onBuyingChartReady = function (api) {
                buyingChartAPI = api;
            };
            $scope.search = function () {
                $scope.showData = true;
                var promises = [];
                var loadSellingGrid = sellingGridAPI.retrieveData({
                    FromDate:$scope.fromDate,
                    ToDate: $scope.toDate,
                    IsSelling : true
                });
                promises.push(loadSellingGrid);
                var loadBuyingGrid = buyingGridAPI.retrieveData({
                    FromDate: $scope.fromDate,
                    ToDate: $scope.toDate,
                    IsSelling: false
                });
                promises.push(loadBuyingGrid);
               
                return UtilsService.waitMultiplePromises(promises);
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_DealProgressAPIService.GetFilteredDealsProgress(dataRetrievalInput).then(function (response) {
                   
                    
                    onResponseReady(response);
                    var api = (dataRetrievalInput.Query.IsSelling== true) ? sellingChartAPI : buyingChartAPI;
                    loadChart(api , response.Data);

                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
        }
        function loadChart(chartAPI,data) {
            var reportData = data;

            var chartConfig =
            {
                type: 'column',
                yAxisTitle: 'Duration'
            };

            var seriesConfig = { isDate : true, titlePath: 'Entity.ProgressDate' };

            var seriesList = [];

            var seriesDefinition = [];
            seriesDefinition.push
             ({
                 title: 'Estimated Duration',
                 valuePath: 'Entity.EstimatedDuration',
                 type: 'column'
             });
            seriesDefinition.push
            ({
                title: 'Reached Duration',
                valuePath: 'Entity.ReachedDuration',
                type: 'column'
            });
         

            chartAPI.renderChart(reportData, chartConfig, seriesDefinition, seriesConfig);
        }


        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadTimeRangeSelector, loadCarrierAccountSelector, loadDealSelector])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
        }
        function loadTimeRangeSelector() {
            var timeRangeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            timeRangeReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    period: PeriodEnum.CurrentYear.value

                };
                VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, payload, timeRangeLoadPromiseDeferred);
            });
            return timeRangeLoadPromiseDeferred.promise;
        }
        function loadCarrierAccountSelector() {
            var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            carrierAccountSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, undefined, carrierAccountSelectorLoadDeferred);
            });

            return carrierAccountSelectorLoadDeferred.promise;
        }
        function loadDealSelector() {
            var dealSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            dealSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dealSelectorAPI, undefined, dealSelectorLoadDeferred);
            });
            return dealSelectorLoadDeferred.promise;
        }
    }

    appControllers.controller('WhS_BE_DealProgressManagementController', DealProgressManagementController);

})(appControllers);