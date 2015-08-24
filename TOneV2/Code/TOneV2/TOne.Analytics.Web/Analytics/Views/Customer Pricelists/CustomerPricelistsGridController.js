'use strict'
CustomerPricelistsGridController.$inject = ['$scope', 'VRModalService', 'UtilsService','PriceListMeasureEnum','CustomerPricelistsAPIService'];
function CustomerPricelistsGridController($scope, VRModalService, UtilsService, PriceListMeasureEnum, CustomerPricelistsAPIService) {
    var mainGridAPI;
    defineScope();
    load();
    function defineScope() {
        $scope.measures = [];
        $scope.data = [];
        $scope.menuActions = [{
            name: "Send Email",
            clicked: function (dataItem) {
                var modalSettings = {
                    useModalTemplate: true,
                    width: "80%",
                    maxHeight: "800px"
                };
                VRModalService.showModal('/Client/Modules/Analytics/Views/Customer Pricelists/CustomerPricelistsGrid.html', null, modalSettings);
            }
        },
        {
            name: "Export",
            clicked: function (dataItem) {
                var modalSettings = {
                    useModalTemplate: true,
                    width: "80%",
                    maxHeight: "800px"
                };
                VRModalService.showModal('/Client/Modules/Analytics/Views/Customer Pricelists/CustomerPricelistsGrid.html', null, modalSettings);
            }
        },
        {
            name: "Get Change Log",
            clicked: function (dataItem) {
                var modalSettings = {
                    useModalTemplate: true,
                    width: "80%",
                    maxHeight: "800px"
                };
                VRModalService.showModal('/Client/Modules/Analytics/Views/Customer Pricelists/CustomerPricelistsGrid.html', null, modalSettings);
            }
        }];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            retrieveData();
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CustomerPricelistsAPIService.GetCustomerPriceListDetails(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
            })
        };
      
    }
    function load() {
        loadMeasures();
        retrieveData();
        
    }
    function retrieveData() {
        if (mainGridAPI == undefined)
            return;
        var query = $scope.dataItem.PriceListID;
        return mainGridAPI.retrieveData(query);
    }
    function loadMeasures() {
        for (var prop in PriceListMeasureEnum) {
            $scope.measures.push(PriceListMeasureEnum[prop]);
        }
    }
};

appControllers.controller('CustomerPricelistsGridController', CustomerPricelistsGridController);