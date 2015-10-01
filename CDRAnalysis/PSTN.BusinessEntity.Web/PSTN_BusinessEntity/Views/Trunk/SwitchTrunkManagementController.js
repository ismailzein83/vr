SwitchTrunkManagementController.$inject = ["$scope", "SwitchAPIService", "PSTN_BE_Service", "SwitchTrunkTypeEnum", "SwitchTrunkDirectionEnum", "UtilsService", "VRNotificationService", "VRModalService"];

function SwitchTrunkManagementController($scope, SwitchAPIService, PSTN_BE_Service, SwitchTrunkTypeEnum, SwitchTrunkDirectionEnum, UtilsService, VRNotificationService, VRModalService) {
    
    defineScope();
    load();

    function defineScope() {

        $scope.switchTrunkGridConnector = {};

        // filter vars
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
            return loadGrid();
        }

        $scope.addTrunk = function () {

            var onTrunkAdded = function (trunkObject) {
                if ($scope.switchTrunkGridConnector.onTrunkAdded != undefined)
                    $scope.switchTrunkGridConnector.onTrunkAdded(trunkObject);
            }

            PSTN_BE_Service.addSwitchTrunk(null, onTrunkAdded);
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
        $scope.switchTrunkGridConnector.query = getFilterObject();

        if ($scope.switchTrunkGridConnector.loadTemplateData != undefined) {
            return $scope.switchTrunkGridConnector.loadTemplateData();
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
