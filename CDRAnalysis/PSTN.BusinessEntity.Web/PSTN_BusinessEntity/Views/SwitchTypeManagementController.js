SwitchTypeManagementController.$inject = ["$scope", "SwitchTypeAPIService", "VRNotificationService", "VRModalService"];

function SwitchTypeManagementController($scope, SwitchTypeAPIService, VRNotificationService, VRModalService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {

        // filter vars
        $scope.name = undefined;

        // grid vars
        $scope.switchTypes = [];
        $scope.gridMenuActions = [];

        // filter functions
        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.addSwitchType = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add a Switch Type";

                modalScope.onSwitchTypeAdded = function (switchTypeObject) {
                    gridAPI.itemAdded(switchTypeObject);
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchTypeEditor.html", null, settings);
        }

        // grid functions
        $scope.gridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SwitchTypeAPIService.GetFilteredSwitchTypes(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        defineMenuActions();
    }

    function load() {
        
    }

    function retrieveData() {
        var query = {
            Name: $scope.name
        };

        gridAPI.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [
            {
                name: "Edit",
                clicked: editSwitchType
            },
            {
                name: "Delete",
                clicked: deleteSwitchType
            }
        ];
    }

    function editSwitchType(gridObject) {
        var modalSettings = {};

        var parameters = {
            SwitchTypeID: gridObject.ID
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Switch Type: " + gridObject.Name;

            modalScope.onSwitchTypeUpdated = function (switchTypeObject) {
                gridAPI.itemUpdated(switchTypeObject);
            };
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchTypeEditor.html", parameters, modalSettings);
    }

    function deleteSwitchType(gridObject) { // ?

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {
                    return SwitchTypeAPIService.DeleteSwitchType(gridObject.ID)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Switch Type", deletionResponse);
                            return retrieveData(); // ?
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchTypeManagementController", SwitchTypeManagementController);
