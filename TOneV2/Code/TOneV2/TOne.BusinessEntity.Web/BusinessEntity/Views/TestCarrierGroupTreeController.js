TestCarrierGroupTreeController.$inject = ['$scope', 'CarrierGroupAPIService', 'CarrierAccountAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'CarrierTypeEnum', 'UtilsService'];

function TestCarrierGroupTreeController($scope, CarrierGroupAPIService, CarrierAccountAPIService, VRModalService, VRNotificationService, VRNavigationService, CarrierTypeEnum, UtilsService) {

    var treeAPI;
    var mainGridAPI;

    defineScope();
    load();

    function defineScope() {

        $scope.beList = [];

        $scope.gridMenuActions = [];

        $scope.carrierAccounts = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.loadMoreData = function () {
            return getCarrierAccounts();
        }


        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = function () {
            if (angular.isObject($scope.currentNode)) {               
                refreshGrid();
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

        $scope.CarrierAccountsDataSource = [];

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

    function getCarrierAccounts() {
        return CarrierGroupAPIService.GetCarrierAccountsByGroup($scope.currentNode.EntityId).then(function (response) {
            angular.forEach(response, function (item) {
                $scope.carrierAccounts.push(item);
            });
        });
    }

    function refreshGrid() {
        mainGridAPI.clearDataAndContinuePaging();
        getCarrierAccounts();
    }
}

appControllers.controller('BusinessEntity_TestCarrierGroupTreeController', TestCarrierGroupTreeController);
