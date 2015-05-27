SwitchManagmentController.$inject = ['$scope', '$q', 'SwitchManagmentAPIService', '$location', '$timeout', '$modal', 'VRModalService', 'VRNotificationService', 'UtilsService'];
function SwitchManagmentController($scope, $q, SwitchManagmentAPIService, $location, $timeout, $modal, VRModalService, VRNotificationService, UtilsService) {

    var stopPaging;
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
            return getData(true);
        }

        $scope.loadMoreData = function () {
            if (stopPaging)
                return;
            return getData(false);
        }

        $scope.gridReady = function (api) {
            gridApi = api;
            getData();
        };
    }

    function getData(startFromFirstRow) {
        if (startFromFirstRow) {
            stopPaging = false;
            $scope.switchsDataSource.length = 0;
        }
        var fromRow = $scope.switchsDataSource.length + 1;
        var toRow = fromRow + 20 - 1;
        return SwitchManagmentAPIService.getFilteredSwitches($scope.switchName, fromRow, toRow).then(function (response) {

            angular.forEach(response, function (itm) {
                $scope.switchsDataSource.push(itm);
            });
            if (response.length < 20)
                stopPaging = true;
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit/Delete Switch",
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