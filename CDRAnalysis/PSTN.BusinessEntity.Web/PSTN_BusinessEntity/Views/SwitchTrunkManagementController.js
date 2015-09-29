SwitchTrunkManagementController.$inject = ["$scope", "SwitchAPIService", "SwitchTrunkTypeEnum", "SwitchTrunkDirectionEnum", "UtilsService", "VRNotificationService", "VRModalService"];

function SwitchTrunkManagementController($scope, SwitchAPIService, SwitchTrunkTypeEnum, SwitchTrunkDirectionEnum, UtilsService, VRNotificationService, VRModalService) {
    
    defineScope();
    load();

    function defineScope() {

        // filter vars
        $scope.switchTrunkGridConnector = {};

        $scope.name = undefined;
        $scope.symbol = undefined;
        $scope.switches = [];
        $scope.selectedSwitches = [];
        $scope.types = UtilsService.getArrayEnum(SwitchTrunkTypeEnum);
        $scope.selectedTypes = [];
        $scope.directions = UtilsService.getArrayEnum(SwitchTrunkDirectionEnum);
        $scope.selectedDirections = [];
        $scope.linkedToTrunkObjects = [
            { value: true, description: "Linked" },
            { value: false, description: "Unlinked" }
        ];
        $scope.selectedLinkedToTrunkObjects = [];

        // filter functions
        $scope.searchClicked = function () {
            loadGrid();
        }

        $scope.addTrunk = function () {
            if ($scope.switchTrunkGridConnector.addTrunk != undefined)
                $scope.switchTrunkGridConnector.addTrunk();
        }
    }

    function load() {

        loadFilters()
            .then(function () {
                setFiltersToDefaultValues();
                loadGrid();
            });
    }

    function loadFilters() {
        $scope.isLoadingFilters = true;

        return SwitchAPIService.GetSwitches()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.switches.push(item);
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isLoadingFilters = false;
            });
    }

    function setFiltersToDefaultValues() {
    }

    function loadGrid() {
        $scope.switchTrunkGridConnector.data = getFilterObject();

        if ($scope.switchTrunkGridConnector.loadTemplateData != undefined) {
            $scope.switchTrunkGridConnector.loadTemplateData();
        }
    }

    function getFilterObject() {
        return {
            Name: $scope.name,
            Symbol: $scope.symbol,
            SelectedSwitchIDs: UtilsService.getPropValuesFromArray($scope.selectedSwitches, "ID"),
            SelectedTypes: UtilsService.getPropValuesFromArray($scope.selectedTypes, "value"),
            SelectedDirections: UtilsService.getPropValuesFromArray($scope.selectedDirections, "value"),
            IsLinkedToTrunk: ($scope.selectedLinkedToTrunkObjects.length == 1) ? $scope.selectedLinkedToTrunkObjects[0].value : null
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchTrunkManagementController", SwitchTrunkManagementController);
