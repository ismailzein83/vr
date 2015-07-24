CarrierProfileManagementController.$inject = ['$scope', 'CarrierAPIService', 'VRModalService'];
function CarrierProfileManagementController($scope, CarrierAPIService, VRModalService) {
    var gridApi;
    var resultKey = "";
    defineScope();
    load();

    function load() {
    }

    function defineScope() {
        $scope.name = '';
        $scope.companyName = '';
        $scope.billingEmail = '';
        $scope.CarrierProfileDataSource = [];
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
            resultKey = "";
            return getData();
        }
    }

    function getData() {
        var pageInfo = gridApi.getPageInfo();
        return CarrierAPIService.GetFilteredProfiles(resultKey, $scope.name, $scope.companyName, $scope.billingEmail, pageInfo.fromRow, pageInfo.toRow).then(function (response) {
            gridApi.addItemsToSource(response.Data);
            resultKey = response.ResultKey;
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editCarrierProfile
        }];
    }

    function editCarrierProfile(carrierProfileObj) {
        var modalSettings = {
        };
        var parameters = {
            profileID: carrierProfileObj.ProfileID
        };
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "CarrierProfile Info(" + carrierProfileObj.ProfileName + ")";
            modalScope.onCarrierProfileUpdated = function (CarrierProfileUpdated) {
                gridApi.itemUpdated(CarrierProfileUpdated);

            };
        };
        VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierProfileEditor.html', parameters, modalSettings);
    }
}
appControllers.controller('Carrier_CarrierProfileManagementController', CarrierProfileManagementController);