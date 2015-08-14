(function (appControllers) {

    "use strict";

    CarrierMaskManagementController.$inject = ['$scope', 'CarrierMaskAPIService', 'VRModalService'];
    function CarrierMaskManagementController($scope, CarrierMaskAPIService, VRModalService) {

        var mainGridApi;

        function retrieveData() {
                return mainGridApi.retrieveData({
                    Name: $scope.name
                });
        }

        function defineGrid() {

            $scope.datasource = [];

            $scope.onGridReady = function (api) {
                mainGridApi = api;
                return retrieveData();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CarrierMaskAPIService.GetFilteredCarrierMasks(dataRetrievalInput)
                .then(function (response) {

                    onResponseReady(response);
                });
            };
        }

        function defineScope() {

            defineMenuActions();

            $scope.CarrierAccountsDataSource = [];

            $scope.AddNewCarrierMask = addCarrierMask;

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editCarrierMask
            }];
        }

        function editCarrierMask(carrierMaskObj) {
            var modalSettings = {
            };
            var parameters = {
                ID: carrierMaskObj.ID
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Carrier Mask Info (" + carrierMaskObj.Name + ")";
                modalScope.onCarrierMaskUpdated = function (CarrierMaskUpdated) {
                    mainGridApi.itemUpdated(CarrierMaskUpdated);

                };
            };
            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierMaskEditor.html', parameters, modalSettings);
        }

        function load() {

            $scope.isGettingData = true;
        }



        function addCarrierMask() {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Add Carrier Mask";
                modalScope.onCarrierMaskAdded = function (CarrierMaskAdded) {
                    gridApi.itemAdded(CarrierMaskAdded);
                };
            };
            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierMaskEditor.html', null, modalSettings);
        }

        defineScope();
        load();
        defineGrid();
    }
    appControllers.controller('Carrier_CarrierMaskManagementController', CarrierMaskManagementController);

})(appControllers);