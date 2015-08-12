TestCarrierGroupTreeController.$inject = ['$scope', 'CarrierGroupAPIService'];

function TestCarrierGroupTreeController($scope, CarrierGroupAPIService) {

    var treeAPI;
    var mainGridAPI;

    function retrieveData() {

        return mainGridApi.retrieveData({
            GroupId: $scope.currentNode.EntityId,
            WithDescendants: true
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

    defineScope();
    load();
    defineGrid();

    //function getCarrierAccounts() {
    //    return CarrierGroupAPIService.GetCarrierAccountsByGroup($scope.currentNode.EntityId, true).then(function (response) {
    //        angular.forEach(response, function (item) {
    //            $scope.carrierAccounts.push(item);
    //        });
    //    });
    //}

    //function refreshGrid() {
    //    mainGridAPI.clearDataAndContinuePaging();
    //    getCarrierAccounts();
    //}
}

appControllers.controller('BusinessEntity_TestCarrierGroupTreeController', TestCarrierGroupTreeController);
