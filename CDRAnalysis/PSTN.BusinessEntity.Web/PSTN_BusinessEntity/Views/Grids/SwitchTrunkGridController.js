SwitchTrunkGridController.$inject = ["$scope", "SwitchTrunkAPIService", "SwitchService", "SwitchTrunkTypeEnum", "SwitchTrunkDirectionEnum", "UtilsService", "VRNotificationService", "VRModalService"];

function SwitchTrunkGridController($scope, SwitchTrunkAPIService, SwitchService, SwitchTrunkTypeEnum, SwitchTrunkDirectionEnum, UtilsService, VRNotificationService, VRModalService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {

        // vars
        $scope.trunks = [];
        $scope.gridMenuActions = [];

        // functions
        if ($scope.dataItem != undefined) {
            $scope.viewScope.switchTrunkGridConnector.onTrunkAdded = addTrunk;
        }

        if ($scope.dataItem == undefined) {

            $scope.switchTrunkGridConnector.loadTemplateData = function () {
                return loadGrid(); // the search button uses the promise object to display its loader
            }

            $scope.switchTrunkGridConnector.onTrunkAdded = addTrunk;
        }

        $scope.gridReady = function (api) {
            gridAPI = api;
            return loadGrid();
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
    }

    function loadGrid() {

        if ($scope.dataItem != undefined
            || ($scope.switchTrunkGridConnector.data != undefined && gridAPI != undefined))
            return retrieveData();
    }

    function retrieveData() {
        var query = ($scope.dataItem != undefined) ?
            {
                Name: null,
                Symbol: null,
                SelectedSwitchIDs: [$scope.dataItem.ID],
                SelectedTypes: null,
                SelectedDirections: null,
                IsLinkedToTrunk: null
            }
            : $scope.switchTrunkGridConnector.data;

        return gridAPI.retrieveData(query);
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

    function addTrunk(trunkObject) {
        setTrunkDescriptions(trunkObject);
        gridAPI.itemAdded(trunkObject);
    }

    function editTrunk(gridObject) {

        var eventHandler = function (firstTrunkObject, linkedToFirstTrunkID, secondTrunkID) {

            setTrunkDescriptions(firstTrunkObject);
            gridAPI.itemUpdated(firstTrunkObject);

            if (linkedToFirstTrunkID != null) {
                var linkedToFirstTrunkObject = UtilsService.getItemByVal($scope.trunks, linkedToFirstTrunkID, "ID");

                if (linkedToFirstTrunkObject != null) {
                    linkedToFirstTrunkObject = UtilsService.cloneObject(linkedToFirstTrunkObject, true);

                    linkedToFirstTrunkObject.LinkedToTrunkID = null;
                    linkedToFirstTrunkObject.LinkedToTrunkName = null;

                    gridAPI.itemUpdated(linkedToFirstTrunkObject);
                }
            }

            var secondTrunkObject = UtilsService.getItemByVal($scope.trunks, secondTrunkID, "ID");
            var linkedToSecondTrunkID = secondTrunkObject.LinkedToTrunkID;

            if (secondTrunkObject != null) {
                var clonedSecondTrunkObject = UtilsService.cloneObject(secondTrunkObject, true);

                clonedSecondTrunkObject.LinkedToTrunkID = firstTrunkObject.ID;
                clonedSecondTrunkObject.LinkedToTrunkName = firstTrunkObject.Name;

                gridAPI.itemUpdated(clonedSecondTrunkObject);
            }

            if (linkedToSecondTrunkID != null) {
                var linkedToSecondTrunkObject = UtilsService.getItemByVal($scope.trunks, linkedToSecondTrunkID, "ID");

                if (linkedToSecondTrunkObject != null) {
                    linkedToSecondTrunkObject = UtilsService.cloneObject(linkedToSecondTrunkObject, true);

                    linkedToSecondTrunkObject.LinkedToTrunkID = null;
                    linkedToSecondTrunkObject.LinkedToTrunkName = null;

                    gridAPI.itemUpdated(linkedToSecondTrunkObject);
                }
            }
        }

        SwitchService.editSwitchTrunk(gridObject, eventHandler);
    }

    function deleteTrunk(gridObject) {

        var eventHandler = function (deletedTrunkObject) {
            gridAPI.itemDeleted(deletedTrunkObject);
        }

        SwitchService.deleteSwitchTrunk(gridObject, eventHandler);
    }

    function setTrunkDescriptions(trunkObject) {

        var type = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", trunkObject.Type);
        trunkObject.TypeDescription = type.description;

        var direction = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", trunkObject.Direction);
        trunkObject.DirectionDescription = direction.description;
    }
}

appControllers.controller("PSTN_BE_SwitchTrunkGridController", SwitchTrunkGridController);
