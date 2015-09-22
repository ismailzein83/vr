SwitchTrunkManagementController.$inject = ["$scope", "SwitchTrunkAPIService", "SwitchAPIService", "SwitchTrunkTypeEnum", "SwitchTrunkDirectionEnum", "UtilsService", "VRNotificationService", "VRModalService"];

function SwitchTrunkManagementController($scope, SwitchTrunkAPIService, SwitchAPIService, SwitchTrunkTypeEnum, SwitchTrunkDirectionEnum, UtilsService, VRNotificationService, VRModalService) {

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

        $scope.addTrunk = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add a Switch Trunk";

                modalScope.onTrunkAdded = function (trunkObject) {

                    var type = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", trunkObject.Type);
                    trunkObject.TypeDescription = type.description;

                    var direction = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", trunkObject.Direction);
                    trunkObject.DirectionDescription = direction.description;

                    gridAPI.itemAdded(trunkObject);
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchTrunkEditor.html", null, settings);
        }

        // grid functions
        $scope.gridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return SwitchTrunkAPIService.GetFilteredSwitchTrunks(dataRetrievalInput)
                .then(function (response) {

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
        $scope.gridMenuActions = [
            {
                name: "Edit",
                clicked: editTrunk
            },
            {
                name: "Delete",
                clicked: deleteTrunk
            }
        ];
    }

    function editTrunk(gridObject) {
        var modalSettings = {};

        var parameters = {
            TrunkID: gridObject.ID
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Switch Trunk: " + gridObject.Name;

            modalScope.onTrunkUpdated = function (trunkObject) {

                var type = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", trunkObject.Type);
                trunkObject.TypeDescription = type.description;

                var direction = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", trunkObject.Direction);
                trunkObject.DirectionDescription = direction.description;

                gridAPI.itemUpdated(trunkObject);
            };
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchTrunkEditor.html", parameters, modalSettings);
    }

    function deleteTrunk(gridObject) { // ?
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {
                    return SwitchTrunkAPIService.DeleteSwitchTrunk(gridObject.ID)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Switch Trunk", deletionResponse);
                            return retrieveData(); // ?
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchTrunkManagementController", SwitchTrunkManagementController);
