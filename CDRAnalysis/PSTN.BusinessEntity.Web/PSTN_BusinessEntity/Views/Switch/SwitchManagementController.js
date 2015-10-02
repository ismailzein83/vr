SwitchManagementController.$inject = ["$scope", "SwitchAPIService", "SwitchTypeAPIService", "PSTN_BE_Service", "UtilsService", "VRNotificationService", "VRModalService"];

function SwitchManagementController($scope, SwitchAPIService, SwitchTypeAPIService, PSTN_BE_Service, UtilsService, VRNotificationService, VRModalService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {

        $scope.switchTrunkGridConnector = {};

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

                modalScope.onSwitchAdded = function (switchObject) {
                    gridAPI.itemAdded(switchObject);
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Switch/SwitchEditor.html", null, settings);
        }

        // grid functions
        $scope.gridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SwitchAPIService.GetFilteredSwitches(dataRetrievalInput)
                .then(function (response) {
                    if (response.Data != undefined) {
                        for (var i = 0; i < response.Data.length; i++) {
                            setDataItemExtension(response.Data[i]);
                        }
                        console.log(i);
                    }
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        defineMenuActions();
    }

    function setDataItemExtension(dataItem) {
        var extensionObject = {};
        extensionObject.onTrunkGridReady = function (api) {
            extensionObject.trunkGridAPI = api;
            extensionObject.onTrunkGridReady = undefined;
        };
        dataItem.extensionObject = extensionObject;
    }

    function load() {
        $scope.isLoadingFilters = true;

        return SwitchTypeAPIService.GetSwitchTypes()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.types.push(item);
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
            },
            {
                name: "Add a Trunk",
                clicked: addTrunk
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

            modalScope.onSwitchUpdated = function (switchObject) {
                gridAPI.itemUpdated(switchObject);
            };
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Switch/SwitchEditor.html", parameters, modalSettings);
    }

    function deleteSwitch(gridObject) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return SwitchAPIService.DeleteSwitch(gridObject.ID)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Switch", deletionResponse))
                                gridAPI.itemDeleted(gridObject);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

    function addTrunk(dataItem) {

        gridAPI.expandRow(dataItem);

        var onTrunkAdded = function (trunkObject) {
            console.log(dataItem);
            if (dataItem.extensionObject.trunkGridAPI.onTrunkAdded != undefined)
                dataItem.extensionObject.trunkGridAPI.onTrunkAdded(trunkObject);
        }

        PSTN_BE_Service.addSwitchTrunk(dataItem.ID, onTrunkAdded);
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchManagementController", SwitchManagementController);
