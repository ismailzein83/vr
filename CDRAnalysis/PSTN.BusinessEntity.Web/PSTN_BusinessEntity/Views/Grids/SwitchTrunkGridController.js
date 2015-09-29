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
            },
            {
                name: "Link to Trunk",
                clicked: linkToTrunk
            }
        ];
    }

    function editTrunk(gridObject) {

        var eventHandler = function (trunkObject) {
            setTrunkDescriptions(trunkObject);
            gridAPI.itemUpdated(trunkObject);
        };

        SwitchService.editSwitchTrunk(gridObject, eventHandler);
    }

    function deleteTrunk(gridObject) {

        var eventHandler = function (deletedTrunkObject) {
            gridAPI.itemDeleted(deletedTrunkObject);
        }

        SwitchService.deleteSwitchTrunk(gridObject, eventHandler);
    }

    function linkToTrunk(gridObject) {
        var modalSettings = {};

        var parameters = {
            TrunkID: gridObject.ID,
            SwitchID: gridObject.SwitchID
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Link " + gridObject.Name + " to a Trunk";

            modalScope.onSwitchTrunkUpdated = function (trunkObject, linkedToTrunkID) {

                var type = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", trunkObject.Type);
                trunkObject.TypeDescription = type.description;

                var direction = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", trunkObject.Direction);
                trunkObject.DirectionDescription = direction.description;

                gridAPI.itemUpdated(trunkObject);

                var linkedToTrunkObject = UtilsService.getItemByVal($scope.trunks, linkedToTrunkID, "ID");

                if (linkedToTrunkObject != null) {
                    linkedToTrunkObject.LinkedToTrunkID = trunkObject.ID;
                    linkedToTrunkObject.LinkedToTrunkName = trunkObject.Name;
                    gridAPI.itemUpdated(linkedToTrunkObject);
                }
            };
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/LinkToTrunk.html", parameters, modalSettings);
    }

    function setTrunkDescriptions(trunkObject) {

        var type = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", trunkObject.Type);
        trunkObject.TypeDescription = type.description;

        var direction = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", trunkObject.Direction);
        trunkObject.DirectionDescription = direction.description;
    }
}

appControllers.controller("PSTN_BE_SwitchTrunkGridController", SwitchTrunkGridController);
