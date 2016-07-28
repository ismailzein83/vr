(function (appControllers) {

    'use strict';

    DealProgressManagementController.$inject = ['$scope', 'WhS_BE_DealService', 'WhS_BE_DealProgressAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'PeriodEnum'];

    function DealProgressManagementController($scope, WhS_BE_DealService, WhS_BE_DealProgressAPIService, UtilsService, VRUIUtilsService, VRNotificationService, PeriodEnum) {
        
        var sellingGridAPI;
        var buyingGridAPI;

        var timeRangeDirectiveAPI;
        var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.dataSourceSelling = [];
            $scope.dataSourceBuying = [];
            $scope.onTimeRangeDirectiveReady = function (api) {
                timeRangeDirectiveAPI = api;
                timeRangeReadyPromiseDeferred.resolve();
            }


            $scope.onSellingGridReady = function (api) {
                sellingGridAPI = api;
            };
            $scope.onBuyingGridReady = function (api) {
                buyingGridAPI = api;
            };
            $scope.search = function () {
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
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([ loadTimeRangeSelector])
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

                }
                VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, payload, timeRangeLoadPromiseDeferred);
            });
            return timeRangeLoadPromiseDeferred.promise;
        }
       
    }

    appControllers.controller('WhS_BE_DealProgressManagementController', DealProgressManagementController);

})(appControllers);