CarrierGroupTreeController.$inject = ['$scope', 'CarrierGroupAPIService','VRNavigationService'];

function CarrierGroupTreeController($scope, CarrierGroupAPIService, VRNavigationService) {

    var treeAPI;
    var mainGridAPI;
    var assignedCarrier;
    var carrierType;
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        console.log(parameters);
        assignedCarrier = parameters.assignedCarrier;
        carrierType = parameters.carrierType;
    }
    function retrieveData() {

        return mainGridApi.retrieveData({
            GroupId: $scope.currentNode.EntityId,
            WithAssignedCarrier:assignedCarrier,
            WithDescendants: true,
            CarrierType:carrierType
        });
    }

    function defineGrid() {

        $scope.datasource = [];

        $scope.onGridReady = function (api) {
            mainGridApi = api;
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CarrierGroupAPIService.GetCarrierAccountsByGroup(dataRetrievalInput)
            .then(function (response) {

                onResponseReady(response);
            });
        };
    }

    function defineScope() {

        $scope.beList = [];

        $scope.onGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = function () {
            if (angular.isObject($scope.currentNode)) {               
                return retrieveData();
            }
        }

        $scope.saveGroup = function () {
            if (angular.isObject($scope.currentNode)) {
                $scope.onTreeSelected($scope.currentNode);
            }
            $scope.modalContext.closeModal()
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {

        $scope.isGettingData = true;

        loadTree().finally(function () {
            $scope.isGettingData = false;
            treeAPI.refreshTree($scope.beList);
        });
    }

    function loadTree() {

        return CarrierGroupAPIService.GetEntityNodes()
           .then(function (response) {
               $scope.beList = response;
           });
    }
    loadParameters();
    defineScope();
    load();
    defineGrid();
}

appControllers.controller('BusinessEntity_CarrierGroupTreeController', CarrierGroupTreeController);
