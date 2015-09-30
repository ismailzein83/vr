﻿SwitchManagementController.$inject = ["$scope", "SwitchAPIService", "SwitchTypeAPIService", "SwitchService", "UtilsService", "VRNotificationService", "VRModalService"];

function SwitchManagementController($scope, SwitchAPIService, SwitchTypeAPIService, SwitchService, UtilsService, VRNotificationService, VRModalService) {

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

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchEditor.html", null, settings);
        }

        // grid functions
        $scope.gridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SwitchAPIService.GetFilteredSwitches(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        $scope.validateAreaCode = function (value) {
            if (value == undefined || value == "") return null;

            if (value.length > 7 || isNaN(parseInt(value))) return "Max of 7 digits";

            return null;
        }

        defineMenuActions();
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

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchEditor.html", parameters, modalSettings);
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

    function addTrunk(gridObject) {

        var eventHandler = function (trunkObject) {
            $scope.switchTrunkGridConnector.onTrunkAdded(trunkObject);
        }

        SwitchService.addSwitchTrunk(eventHandler);
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchManagementController", SwitchManagementController);
