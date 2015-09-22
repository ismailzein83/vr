SwitchTrunkManagementController.$inject = ["$scope", "SwitchTrunkAPIService", "SwitchAPIService", "SwitchTrunkTypeEnum", "SwitchTrunkDirectionEnum", "UtilsService", "VRNotificationService"];

function SwitchTrunkManagementController($scope, SwitchTrunkAPIService, SwitchAPIService, SwitchTrunkTypeEnum, SwitchTrunkDirectionEnum, UtilsService, VRNotificationService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {

        // filter vars
        $scope.name = undefined;
        $scope.symbol = undefined;
        $scope.switches = [];
        $scope.selectedSwitches = [];
        $scope.types = UtilsService.getArrayEnum(SwitchTrunkTypeEnum);
        $scope.selectedTypes = [];
        $scope.directions = UtilsService.getArrayEnum(SwitchTrunkDirectionEnum);
        $scope.selectedDirections = [];

        // grid vars
        $scope.trunks = [];
        $scope.gridMenuActions = [];

        // filter functions
        $scope.searchClicked = function () {
            return retrieveData();
        }

        // grid functions
        $scope.gridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return SwitchTrunkAPIService.GetFilteredSwitchTrunks(dataRetrievalInput)
                .then(function (response) {
                    console.log(response.Data);

                    angular.forEach(response.Data, function (item) {
                        var type = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", item.Type);
                        item.TypeDescription = type.description;

                        var direction = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", item.Direction);
                        item.DirectionDescription = direction.description;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        defineMenuActions();
    }

    function load() {
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

    function retrieveData() {
        var query = {
            Name: $scope.name,
            Symbol: $scope.symbol,
            SelectedSwitchIDs: UtilsService.getPropValuesFromArray($scope.selectedSwitches, "ID"),
            SelectedTypes: UtilsService.getPropValuesFromArray($scope.selectedTypes, "value"),
            SelectedDirections: UtilsService.getPropValuesFromArray($scope.selectedDirections, "value")
        };

        gridAPI.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editTrunk
        }];
    }

    function editTrunk(gridObject) {
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchTrunkManagementController", SwitchTrunkManagementController);
