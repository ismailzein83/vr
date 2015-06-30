﻿CarrierAccountManagementController.$inject = ['$scope', 'CarrierAPIService', 'VRModalService'];
function CarrierAccountManagementController($scope, CarrierAPIService, VRModalService) {
    var gridApi;
    defineScope();
    load();

    function load() {
    }

    function defineScope() {
        $scope.name = '';
        $scope.companyName = '';
        $scope.CarrierAccountsDataSource = [];
        defineMenuActions();
        $scope.gridReady = function (api) {
            gridApi = api;
            $scope.isLoading = true;
            getData().finally(function () {
                $scope.isLoading = false;
            });
        };
        $scope.loadMoreData = function () {
            return getData();
        }
        $scope.searchClicked = function () {
            gridApi.clearDataAndContinuePaging();
            return getData();
        }
    }
    function getData() {
        var pageInfo = gridApi.getPageInfo();
        return CarrierAPIService.GetCarrierAccounts($scope.name, $scope.companyName, pageInfo.fromRow, pageInfo.toRow).then(function (response) {
            gridApi.addItemsToSource(response);
            //angular.forEach(response, function (itm) {
            //    $scope.CarrierAccountsDataSource.push(itm);
            //});
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editCarrierAccount
        }];
    }

    function editCarrierAccount(carrierAccountObj) {
        var modalSettings = {
        };
        var parameters = {
            carrierAccountId: carrierAccountObj.CarrierAccountId
        };
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "CarrierAccount Info(" + carrierAccountObj.ProfileName + ")";
            modalScope.onCarrierAccountUpdated = function (CarrierAccountUpdated) {
                gridApi.itemUpdated(CarrierAccountUpdated);

            };
        };
        VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierAccountEditor.html', parameters, modalSettings);
    }
}

appControllers.controller('Carrier_CarrierAccountManagementController', CarrierAccountManagementController);