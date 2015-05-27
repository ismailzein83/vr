SwitchManagmentController.$inject = ['$scope', '$q', 'SwitchManagmentAPIService', '$location', '$timeout', '$modal', 'VRModalService', 'VRNotificationService', 'UtilsService'];
function SwitchManagmentController($scope, $q, SwitchManagmentAPIService, $location, $timeout, $modal, VRModalService, VRNotificationService, UtilsService) {

    var gridApi;

    defineScope();
    load();

    function load() {
    }

    function defineScope() {       

        $scope.switchName = '';
       
        $scope.switchsDataSource = [];

        defineMenuActions();

        $scope.addSwitch = addSwitch;

        $scope.searchClicked = function () {
            gridApi.clearDataAndContinuePaging();
            return getData();
        }

        $scope.loadMoreData = function () {
            return getData();
        }

        $scope.gridReady = function (api) {
            gridApi = api;
            getData();
        };
    }

    function getData() {
        var pageInfo = gridApi.getPageInfo();
        return SwitchManagmentAPIService.GetFilteredSwitches($scope.switchName, pageInfo.fromRow, pageInfo.toRow).then(function (response) {

            angular.forEach(response, function (itm) {
                $scope.switchsDataSource.push(itm);
            });
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editSwitch
        }];
    }

    function addSwitch() {
        var modalSettings = {
        };
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "New Switch Info";
            modalScope.onSwitchAdded = function (switchObj) {
                gridApi.itemAdded(switchObj);
            };
        };
        VRModalService.showModal('/Client/Modules/BusinessEntity/Views/SwitchEditor.html', null, modalSettings);
    }

    function editSwitch(switchObj) {
        var modalSettings = {
        };
        var parameters = {
            switchId: switchObj.SwitchId
        };
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Switch Info(" + switchObj.Symbol + ")";
            modalScope.onSwitchUpdated = function (switchUpdated) {
                gridApi.itemUpdated(switchUpdated);

            };
        };
        VRModalService.showModal('/Client/Modules/BusinessEntity/Views/SwitchEditor.html', parameters, modalSettings);
    }
}
appControllers.controller('SwitchManagmentController', SwitchManagmentController);