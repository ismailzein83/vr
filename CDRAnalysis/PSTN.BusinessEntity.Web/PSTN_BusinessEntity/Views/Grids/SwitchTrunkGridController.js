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
        $scope.switchTrunkGridConnector.loadTemplateData = function () {
            return loadGrid(); // the search button uses the promise object to display its loader
        }

        $scope.switchTrunkGridConnector.onTrunkAdded = function (trunkObject) {
            setTrunkDescriptions(trunkObject);
            gridAPI.itemAdded(trunkObject);
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

        if ($scope.switchTrunkGridConnector.data != undefined && gridAPI != undefined)
            return retrieveData();
    }

    function retrieveData() {
        var query = $scope.switchTrunkGridConnector.data;
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

    function editTrunk(gridObject) {

        var eventHandler = function (firstTrunkObject, linkedToFirstTrunkID, secondTrunkID) {

            console.log(firstTrunkObject);

            setTrunkDescriptions(firstTrunkObject);
            gridAPI.itemUpdated(firstTrunkObject);

            if (linkedToFirstTrunkID != null) {
                var linkedToFirstTrunkObject = UtilsService.getItemByVal($scope.trunks, linkedToFirstTrunkID, "ID");

                if (linkedToFirstTrunkObject != null) {
                    linkedToFirstTrunkObject.LinkedToTrunkID = null;
                    linkedToFirstTrunkObject.LinkedToTrunkName = null;

                    gridAPI.itemUpdated(linkedToFirstTrunkObject);
                }
            }

            var secondTrunkObject = UtilsService.getItemByVal($scope.trunks, secondTrunkID, "ID");
            var linkedToSecondTrunkID = secondTrunkObject.LinkedToTrunkID;

            console.log(secondTrunkObject);

            if (secondTrunkObject != null) {
                secondTrunkObject.LinkedToTrunkID = firstTrunkObject.ID;
                secondTrunkObject.LinkedToTrunkName = firstTrunkObject.Name;

                gridAPI.itemUpdated(secondTrunkObject);
            }

            if (linkedToSecondTrunkID != null) {
                var linkedToSecondTrunkObject = UtilsService.getItemByVal($scope.trunks, linkedToSecondTrunkID, "ID");

                if (linkedToSecondTrunkObject != null) {
                    linkedToSecondTrunkObject.LinkedToTrunkID = null;
                    linkedToSecondTrunkObject.LinkedToTrunkName = null;

                    gridAPI.itemUpdated(linkedToSecondTrunkObject);
                }
            }
        }

        /*
        var eventHandler = function (trunkObject, linkedToTrunkID) {

            setTrunkDescriptions(trunkObject);
            gridAPI.itemUpdated(trunkObject);

            var linkedToTrunkObject = UtilsService.getItemByVal($scope.trunks, linkedToTrunkID, "ID");
            console.log(trunkObject);

            if (linkedToTrunkObject != null) {
                console.log("in");
                linkedToTrunkObject.LinkedToTrunkID = trunkObject.ID;
                linkedToTrunkObject.LinkedToTrunkName = trunkObject.Name;

                console.log(linkedToTrunkObject);

                gridAPI.itemUpdated(linkedToTrunkObject);
            }
        };
        */

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
