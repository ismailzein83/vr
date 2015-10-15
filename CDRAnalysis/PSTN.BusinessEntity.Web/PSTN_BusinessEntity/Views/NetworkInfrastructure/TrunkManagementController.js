TrunkManagementController.$inject = ["$scope", "PSTN_BE_Service", "SwitchAPIService", "TrunkTypeEnum", "TrunkDirectionEnum", "UtilsService", "VRNotificationService", "VRModalService"];

function TrunkManagementController($scope, PSTN_BE_Service, SwitchAPIService, TrunkTypeEnum, TrunkDirectionEnum, UtilsService, VRNotificationService, VRModalService) {
    
    var trunkGridAPI;

    defineScope();
    load();

    function defineScope() {

        // filter vars
        $scope.name = undefined;
        $scope.symbol = undefined;
        $scope.switches = [];
        $scope.selectedSwitches = [];
        $scope.types = UtilsService.getArrayEnum(TrunkTypeEnum);
        $scope.selectedTypes = [];
        $scope.directions = UtilsService.getArrayEnum(TrunkDirectionEnum);
        $scope.selectedDirections = [];
        $scope.linkedToTrunkObjs = [
            { value: true, description: "Linked" },
            { value: false, description: "Unlinked" }
        ];
        $scope.selectedLinkedToTrunkObjs = [];

        // filter functions
        $scope.searchClicked = function () {
            if (trunkGridAPI != undefined) {
                var query = getFilterObj();
                trunkGridAPI.retrieveData(query);
            }
        }

        $scope.addTrunk = function () {

            var onTrunkAdded = function (trunkObj) {
                if (trunkGridAPI != undefined)
                    trunkGridAPI.onTrunkAdded(trunkObj);
            }

            PSTN_BE_Service.addTrunk(null, onTrunkAdded);
        }

        // directive functions
        $scope.onTrunkGridReady = function (api) {
            trunkGridAPI = api;
            trunkGridAPI.retrieveData({});
        }
    }

    function load() {

        loadFilters().then(function () {
            setFiltersToDefaultValues();

            if (trunkGridAPI != undefined)
                trunkGridAPI.retrieveData(getFilterObj());
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

    function getFilterObj() {
        return {
            Name: $scope.name,
            Symbol: $scope.symbol,
            SelectedSwitchIds: UtilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId"),
            SelectedTypes: UtilsService.getPropValuesFromArray($scope.selectedTypes, "value"),
            SelectedDirections: UtilsService.getPropValuesFromArray($scope.selectedDirections, "value"),
            IsLinkedToTrunk: ($scope.selectedLinkedToTrunkObjs.length == 1) ? $scope.selectedLinkedToTrunkObjs[0].value : null
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_TrunkManagementController", TrunkManagementController);
