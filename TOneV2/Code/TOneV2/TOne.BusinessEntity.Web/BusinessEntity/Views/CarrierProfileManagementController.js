CarrierProfileManagementController.$inject = ['$scope', 'CarrierProfileAPIService', 'VRModalService'];
function CarrierProfileManagementController($scope, CarrierProfileAPIService, VRModalService) {
    var gridApi;
    var resultKey = "";
    defineScope();
    load();

    function load() {
    }

    function defineScope() {
        $scope.CarrierProfileDataSource = [];
        defineMenuActions();
        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        };
        $scope.searchClicked = function () {
            return retrieveData();
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CarrierProfileAPIService.GetFilteredCarrierProfiles(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }
        $scope.AddNewCarrierProfile = addCarrierProfile;
    }
    function retrieveData() {
        var query = {
            Name: $scope.name,
            CompanyName: $scope.companyName,
            BillingEmail: $scope.billingEmail
        };
        var result = gridApi.retrieveData(query);
        return result;
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
            modalScope.title = "Carrier Profile Info(" + carrierProfileObj.Name + ")";
            modalScope.onCarrierProfileUpdated = function (CarrierProfileUpdated) {
                gridApi.itemUpdated(CarrierProfileUpdated);

            };
        };
        VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierProfileEditor.html', parameters, modalSettings);
    }

    function addCarrierProfile() {
        var modalSettings = {};

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Carrier Profile";
            modalScope.onCarrierProfileAdded = function (CarrierProfileUpdated) {
                gridApi.itemAdded(CarrierProfileUpdated);
            };
        };
        VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierProfileEditor.html', null, modalSettings);
    }
}
appControllers.controller('Carrier_CarrierProfileManagementController', CarrierProfileManagementController);