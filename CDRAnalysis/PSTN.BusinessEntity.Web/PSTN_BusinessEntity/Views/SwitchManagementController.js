SwitchManagementController.$inject = ["$scope", "SwitchAPIService", "UtilsService", "VRNotificationService", "VRModalService"];

function SwitchManagementController($scope, SwitchAPIService, UtilsService, VRNotificationService, VRModalService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {

        // filter vars
        $scope.name = undefined;
        $scope.types = [];
        $scope.selectedTypes = [];
        $scope.areaCode = undefined;

        // grid vars
        $scope.switches = [];
        $scope.gridMenuActions = [];

        // filter functions
        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.addSwitch = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add a Switch";

                modalScope.onSwitchAdded = function (switchObject, addedSwitchTypes) {

                    addNewSwitchTypes(addedSwitchTypes);

                    switchObject.TypeDescription = (switchObject.TypeID != null) ?
                        UtilsService.getItemByVal($scope.types, switchObject.TypeID, "ID").Name : null;

                    gridAPI.itemAdded(switchObject);
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchEditor.html", null, settings);
        }

        // grid functions
        $scope.gridReady = function (api) {
            gridAPI = api;

            if ($scope.types.length > 0) // types are loaded
                return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SwitchAPIService.GetFilteredSwitches(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (item) {
                        item.TypeDescription = UtilsService.getItemByVal($scope.types, item.TypeID, "ID").Name;
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

        return SwitchAPIService.GetSwitchTypes()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.types.push(item);
                });

                if (gridAPI != undefined)
                    return retrieveData();
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
            SelectedTypeIDs: UtilsService.getPropValuesFromArray($scope.selectedTypes, "ID"),
            AreaCode: $scope.areaCode,
        };

        gridAPI.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [
            {
                name: "Edit",
                clicked: editSwitch
            },
            {
                name: "Delete",
                clicked: deleteSwitch
            }
        ];
    }
    
    function editSwitch(gridObject) {
        var modalSettings = {};

        var parameters = {
            SwitchID: gridObject.ID
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Switch: " + gridObject.Name;

            modalScope.onSwitchUpdated = function (switchObject, addedSwitchTypes) {
                
                addNewSwitchTypes(addedSwitchTypes);

                switchObject.TypeDescription = (switchObject.TypeID != null) ?
                    UtilsService.getItemByVal($scope.types, switchObject.TypeID, "ID").Name : null;

                gridAPI.itemUpdated(switchObject);
            };
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchEditor.html", parameters, modalSettings);
    }

    function deleteSwitch(gridObject) { // ?

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {
                    return SwitchAPIService.DeleteSwitch(gridObject.ID)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Switch", deletionResponse);
                            return retrieveData(); // ?
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

    function addNewSwitchTypes(addedSwitchTypes) {
        angular.forEach(addedSwitchTypes, function (item) {
            $scope.types.push(item);
        });
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchManagementController", SwitchManagementController);
