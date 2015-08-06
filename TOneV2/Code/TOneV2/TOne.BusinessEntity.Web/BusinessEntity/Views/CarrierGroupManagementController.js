CarrierGroupManagementController.$inject = ['$scope', 'CarrierGroupAPIService', 'VRModalService'];
function CarrierGroupManagementController($scope, CarrierGroupAPIService, VRModalService) {

    var treeAPI;
    var mainGridAPI;

    defineScope();
    load();

    function defineScope() {

        $scope.beList = [];

        $scope.gridMenuActions = [];

        $scope.carrierAccounts = [];

        defineMenuActions();


        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            //getData();
        };

        $scope.loadMoreData = function () {
            return getData();
        }


        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = function () {
            if (angular.isObject($scope.currentNode)) {
                refreshGrid();
            }
        }
           

        $scope.CarrierAccountsDataSource = [];

        $scope.addNewGroup = addGroup;
        $scope.editGroup = editGroup;
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

    function getData() {
        return CarrierGroupAPIService.GetCarriersByGroup($scope.currentNode.EntityId).then(function (response) {
            angular.forEach(response, function (item) {
                $scope.carrierAccounts.push(item);
            });
        });
    }

    function refreshGrid() {
        mainGridAPI.clearDataAndContinuePaging();
        getData();
    }

    function defineMenuActions() {

    }

    function addGroup() {
        var settings = {
            useModalTemplate: true,
        };
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Carrier Group";
            modalScope.onTreeAdded = function () {
                load();
            };
        };

        VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierGroupEditor.html', null, settings);
    }

    function editGroup() {
        var settings = {
            useModalTemplate: true,
        };

        var parameters = {
            carrierGroupId: $scope.currentNode.EntityId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Carrier Group";
            modalScope.onTreeUpdated = function () {
                load();

            };
        };

        VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierGroupEditor.html', parameters, settings);
    }
}

appControllers.controller('Carrier_CarrierGroupManagementController', CarrierGroupManagementController);