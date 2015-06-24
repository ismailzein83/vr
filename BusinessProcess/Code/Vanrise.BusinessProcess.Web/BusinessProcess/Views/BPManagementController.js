BPManagementController.$inject = ['$scope','UtilsService', 'BusinessProcessAPIService', 'VRModalService'];

function BPManagementController($scope,UtilsService, BusinessProcessAPIService, VRModalService) {

    "use strict";

    var mainGridAPI;

    function showBPTrackingModal(bpInstanceObj) {

        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
            BPInstanceID: bpInstanceObj.ProcessInstanceID
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
        $scope.loadMoreData = function () {
            return getData();
        };
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
        };
    }

    function getCurrentDate(days) {
        var d = new Date();
        var curr_date = d.getDate() + days;
        var curr_month = d.getMonth();
        var curr_year = d.getFullYear();
        return new Date(curr_year, curr_month, curr_date);
    }

    function defineScope() {
        $scope.toDate = getCurrentDate(0);
        $scope.fromDate = getCurrentDate(-1);
        $scope.definitions = [];
        $scope.selectedDefinition = [];
        $scope.instanceStatus = [];
        $scope.selectedInstanceStatus = [];
        $scope.searchClicked = function () {
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
        };
        $scope.onTitleClicked = function (dataItem) {
            showBPTrackingModal(dataItem);
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