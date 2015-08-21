CustomerTODManagementController.$inject = ['$scope', 'CarrierAccountAPIService', 'ZonesService', 'BusinessEntityAPIService_temp', 'VRModalService', 'CarrierTypeEnum'];
function CustomerTODManagementController($scope, CarrierAccountAPIService,ZonesService, BusinessEntityAPIService ,VRModalService, CarrierTypeEnum) {
    var gridApi;
    defineScope();
    load();

    function load() {
        loadCustomers()
    }

    function defineScope() {
        $scope.customers = [];
        $scope.searchZones = [];
        $scope.selectedZones = [];
        //$scope.CarrierAccountsDataSource = [];
        //defineMenuActions();
        //$scope.gridReady = function (api) {
        //    gridApi = api;
        //    return retrieveData();
        //};
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
        //$scope.searchClicked = function () {
        //    return retrieveData();
        //};

        //$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
        //    return CarrierAccountAPIService.GetFilteredCarrierAccounts(dataRetrievalInput)
        //    .then(function (response) {
        //        onResponseReady(response);
        //    });
        //};
    }

    function retrieveData() {
        //var query = {
        //    Name: $scope.name,
        //    CompanyName: $scope.companyName
        //};
        //return gridApi.retrieveData(query);
    }

    function defineMenuActions() {
        //$scope.gridMenuActions = [{
        //    name: "Edit",
        //    clicked: editCarrierAccount
        //}];
    }

    function editCarrierAccount(carrierAccountObj) {
        //var modalSettings = {
        //};
        //var parameters = {
        //    carrierAccountId: carrierAccountObj.CarrierAccountId
        //};
        //modalSettings.onScopeReady = function (modalScope) {
        //    modalScope.title = "CarrierAccount Info(" + carrierAccountObj.ProfileName + ")";
        //    modalScope.onCarrierAccountUpdated = function (CarrierAccountUpdated) {
        //        gridApi.itemUpdated(CarrierAccountUpdated);

        //    };
        //};
        //VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierAccountEditor.html', parameters, modalSettings);
    }

    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value ,false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        });
    }
}

appControllers.controller('Customer_CustomerTODManagementController', CustomerTODManagementController);