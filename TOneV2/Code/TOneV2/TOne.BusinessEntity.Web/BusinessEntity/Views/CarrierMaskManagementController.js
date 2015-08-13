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
                return CarrierMaskAPIService.GetCarrierMasks(dataRetrievalInput)
                .then(function (response) {

                    onResponseReady(response);
                });
            };
        }

        function defineScope() {

            $scope.CarrierAccountsDataSource = [];

            $scope.addNewGroup = addGroup;
        }

        function load() {

            $scope.isGettingData = true;
        }

        function addGroup() {
            var settings = {
                useModalTemplate: true,
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add Carrier Mask";
                modalScope.onTreeAdded = function () {
                    load();
                    $scope.currentNode = undefined;
                };
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierMaskEditor.html', null, settings);
        }

        defineScope();
        load();
        defineGrid();
    }
    appControllers.controller('Carrier_CarrierMaskManagementController', CarrierMaskManagementController);

})(appControllers);