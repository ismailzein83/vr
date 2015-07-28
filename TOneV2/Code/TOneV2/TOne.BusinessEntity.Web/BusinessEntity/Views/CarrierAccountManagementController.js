CarrierAccountManagementController.$inject = ['$scope', 'CarrierAccountAPIService', 'VRModalService'];
function CarrierAccountManagementController($scope, CarrierAccountAPIService, VRModalService) {
    var gridApi;
    defineScope();
    load();

    function load() {
    }

    function defineScope() {

        $scope.CarrierAccountsDataSource = [];
        defineMenuActions();
        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CarrierAccountAPIService.GetFilteredCarrierAccounts(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        };
    }

    function retrieveData() {
        var query = {
            Name: $scope.name,
            CompanyName: $scope.companyName
        };
        return gridApi.retrieveData(query);
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