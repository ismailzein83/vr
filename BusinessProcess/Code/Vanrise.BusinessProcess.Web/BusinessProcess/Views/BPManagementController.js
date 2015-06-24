BPManagementController.$inject = ['$scope','UtilsService', 'BusinessProcessAPIService', 'VRModalService'];

function BPManagementController($scope,UtilsService, BusinessProcessAPIService, VRModalService) {

    "use strict";

    var mainGridAPI;

    function showBPTrackingModal(BPInstanceObj) {

        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
            BPInstanceID: BPInstanceObj.ProcessInstanceID
        }, {
            onScopeReady: function (modalScope) {
                modalScope.title = "Tracking";
            }
        });
    }

    function getData() {

        var pageInfo = mainGridAPI.getPageInfo();

        return BusinessProcessAPIService.GetFilteredBProcess(UtilsService.getPropValuesFromArray($scope.selectedDefinition, "BPDefinitionID"),
            UtilsService.getPropValuesFromArray($scope.selectedInstanceStatus, "Value"),
            pageInfo.fromRow,
            pageInfo.toRow,
            $scope.fromDate,
            $scope.toDate).then(function (response) {
                mainGridAPI.addItemsToSource(response);
            });
    }

    function defineGrid() {
        $scope.datasource = [];
        $scope.gridMenuActions = [];
        $scope.loadMoreData = function () {
            return getData();
        };
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
        };
        $scope.gridMenuActions = [{
            name: "Tracking",
            clicked: showBPTrackingModal
        }];
    }

    function defineScope() {

        $scope.definitions = [];
        $scope.selectedDefinition = [];
        $scope.instanceStatus = [];
        $scope.selectedInstanceStatus = [];
        $scope.searchClicked = function () {
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
        };
    }

    function loadFilters() {

        BusinessProcessAPIService.GetDefinitions().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.definitions.push(item);
            });
        });

        BusinessProcessAPIService.GetStatusList().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.instanceStatus.push(item);
            });
        });
    }

    function load() {
        loadFilters();
    }

    defineScope();
    load();
    defineGrid();
}
appControllers.controller('BusinessProcess_BPManagementController', BPManagementController);