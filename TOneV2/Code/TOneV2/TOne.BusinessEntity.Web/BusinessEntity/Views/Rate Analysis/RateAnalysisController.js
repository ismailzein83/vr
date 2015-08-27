RateAnalysisController.$inject = ['$scope', 'UtilsService', '$q', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'RateAnalysisAPIService','ChangeEnum','EffectiveEnum','VRNavigationService','ZoneAPIService','VRNotificationService'];

function RateAnalysisController($scope, UtilsService, $q, CarrierAccountAPIService, CarrierTypeEnum, RateAnalysisAPIService, ChangeEnum, EffectiveEnum, VRNavigationService, ZoneAPIService, VRNotificationService) {
    var mainGridAPI;
    var chartRateAnalysisAPI;
    var responseData;
    var filter;
    var isExpendable = true;
    loadParameters();
    defineScope();
    load();
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);    
        if (parameters != undefined) {
            isExpendable = false;
            filter={
                Rate : parameters.Rate,
                ZoneId: parameters.ZoneId,
                CustomerId: parameters.CustomerId!=undefined?parameters.CustomerId:null,
                SupplierId: parameters.SupplierId != undefined ? parameters.SupplierId : null,
                EffectiveDate: parameters.EffectiveDate
            }

        }
       
    }
    function defineScope() {
        $scope.data = [];
        $scope.showResult = false;
        $scope.selectedSupplier;
        $scope.selectedZone;
        $scope.selectedCustomer;
        $scope.suppliers = [];
        $scope.customers = [];
        $scope.effectiveDate = new Date();
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            if (!isExpendable)
                retrieveData();
        }
        $scope.checkExpandablerow = function () {
            return isExpendable;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return RateAnalysisAPIService.GetRateAnalysis(dataRetrievalInput).then(function (response) {
                fillEffectiveValue(response);
                responseData = response;
                if (chartRateAnalysisAPI!=undefined)
                    showRateAnalysisChart(response);
                onResponseReady(response);
                $scope.showResult = true;
            })
        };
        $scope.getData = function () {
            return retrieveData();
        };
        $scope.getChangeIcon = function (dataItem) {
            switch (dataItem.Change) {
                case ChangeEnum.Increase.value: return ChangeEnum.Increase.icon;
                case ChangeEnum.Decrease.value: return ChangeEnum.Decrease.icon;
                case ChangeEnum.New.value: return ChangeEnum.New.icon;
            }
        }
        $scope.chartRateAnalysisReady = function (api) {
            chartRateAnalysisAPI = api;
            if(responseData!=undefined)
                showRateAnalysisChart(responseData);
            
        };
    }
    function fillEffectiveValue(response) {
        for(var i=0;i<response.Data.length;i++){
            switch (response.Data[i].Effective) {
                case EffectiveEnum.Y.value: response.Data[i].IsEffective = EffectiveEnum.Y.description; break;
                case EffectiveEnum.N.value: response.Data[i].IsEffective = EffectiveEnum.N.description; break;
            }
        }
     
    }
    function showRateAnalysisChart(response) {
        $scope.isGettingEntityStatistics = true;


        var chartData = [];
        for (var i = 0; i < response.Data.length; i++) {
            var values = {
                Rate: response.Data[i].Rate,
            }
            chartData.push(values);
        }

        var title = "Rate";
        var seriesDefinitions = [{
            title: "Rate Values",
            valuePath: "Rate",

        }];
        var xAxisDefinition = {
            titlePath: "Rate",
            isDateTime: true
        };
        var chartDefinition = {
            type: "spline",
            title: title,
            yAxisTitle: "Value"
        };
        chartRateAnalysisAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
        $scope.isGettingEntityStatistics = false;

    }
    function retrieveData() {
        if (mainGridAPI == undefined)
            return;
        var query = {
            ZoneId: $scope.selectedZone.ZoneId,
            EffectedDate:$scope.effectiveDate,
            CustomerId: $scope.selectedCustomer != undefined ? $scope.selectedCustomer.CarrierAccountID : null,
            SupplierId: $scope.selectedSupplier!=undefined?$scope.selectedSupplier.CarrierAccountID:null
        }

        return mainGridAPI.retrieveData(query);
    }
    function load() {
        UtilsService.waitMultipleAsyncOperations([loadSuppliers, loadCustomers])
           .then(function () {
               if (!isExpendable)
                   loadRateResult();
           }).catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           });

       
    }
    function loadRateResult() {
        if (filter.SupplierId != undefined) {
            $scope.supplierSelected = true;
            $scope.customerSelected = false;
            $scope.selectedSupplier = UtilsService.getItemByVal($scope.suppliers, filter.SupplierId, 'CarrierAccountID');
            
        }
           
        if (filter.CustomerId!=undefined)
            $scope.selectedCustomer = UtilsService.getItemByVal($scope.customers, filter.CustomerId, 'CarrierAccountID');
        $scope.effectiveDate = filter.EffectiveDate;
        ZoneAPIService.GetZoneById(filter.ZoneId).then(function (response) {
             $scope.selectedZone = response;
            if (mainGridAPI != undefined)
                retrieveData();
        });
        
    }
    function loadSuppliers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value, false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
          //  $scope.selectedSupplier = $scope.suppliers[0];
        });

    }
    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value, false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
         //   $scope.selectedSupplier = $scope.suppliers[0];
        });

    }
};
appControllers.controller('BE_RateAnalysisController', RateAnalysisController);