'use strict'
CustomerPricelistsGridController.$inject = ['$scope', 'VRModalService', 'UtilsService','CustomerPricelistsAPIService','ChangeEnum','CodeAPIService'];
function CustomerPricelistsGridController($scope, VRModalService, UtilsService, CustomerPricelistsAPIService, ChangeEnum, CodeAPIService) {
    var mainGridAPI;
    defineScope();
    load();
    function defineScope() {
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
        $scope.onRateClicked = function (dataItem) {
            var modalSettings = {
                useModalTemplate: true,
                width: "80%",
                maxHeight: "800px"
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Rate Analysis";
            };
            var parameters = {
                Rate: dataItem.Rate,
                ZoneId: dataItem.ZoneID,
                CustomerId: $scope.gridParentScope.selectedCustomer.CarrierAccountID,
                EffectiveDate: $scope.dataItem.BeginEffectiveDate
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/Rate Analysis/RateAnalysis.html', parameters, modalSettings);
        }
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            retrieveData();
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CustomerPricelistsAPIService.GetCustomerPriceListDetails(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
            })
        };
        $scope.getChangeIcon = function (dataItem) {
            switch(dataItem.Change){
                case ChangeEnum.Increase.value:return ChangeEnum.Increase.icon;
                case ChangeEnum.Decrease.value: return ChangeEnum.Decrease.icon;
                case ChangeEnum.New.value: return ChangeEnum.New.icon;
            }
        }
        $scope.getCodes = function (dataItem) {
            if (dataItem.isCodeLoaded == undefined) {
                dataItem.isCodeLoaded = true;
                CodeAPIService.GetCodes(dataItem.ZoneID, $scope.dataItem.BeginEffectiveDate).then(function (response) {
                    dataItem.codes = response;
                });

            }


        }
      
    }
    function load() {
        retrieveData();
        
    }
    function retrieveData() {
        if (mainGridAPI == undefined)
            return;
        var query = $scope.dataItem.PriceListID;
        return mainGridAPI.retrieveData(query);
    }

};

appControllers.controller('CustomerPricelistsGridController', CustomerPricelistsGridController);