"use strict";

app.directive("vrPstnBeTrunkgrid", ["PSTN_BE_Service", "SwitchTrunkAPIService", "SwitchTrunkTypeEnum", "SwitchTrunkDirectionEnum", "UtilsService", "VRNotificationService",
    function (PSTN_BE_Service, SwitchTrunkAPIService, SwitchTrunkTypeEnum, SwitchTrunkDirectionEnum, UtilsService, VRNotificationService) {
    
    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var trunkGrid = new TrunkGrid($scope, ctrl);
            trunkGrid.defineScope();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
           
        },
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/SwitchTrunkGridTemplate.html"

    };

    function TrunkGrid($scope, ctrl) {

        var gridAPI;
        this.defineScope = defineScope;

        function defineScope()
        {
            $scope.trunks = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.retrieveData = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onTrunkAdded = function (trunkObject) {
                        setTrunkDescriptions(trunkObject);
                        gridAPI.itemAdded(trunkObject);

                        var linkedToTrunkID = trunkObject.LinkedToTrunkID;
                        updateDataItem(linkedToTrunkID, trunkObject.ID, trunkObject.Name);

                        var linkedToTheLinkedToTrunkObject = UtilsService.getItemByVal($scope.trunks, linkedToTrunkID, "LinkedToTrunkID");
                        if (linkedToTheLinkedToTrunkObject != null)
                            updateDataItem(linkedToTheLinkedToTrunkObject.ID, null, null);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return SwitchTrunkAPIService.GetFilteredSwitchTrunks(dataRetrievalInput)
                    .then(function (response) {

                        angular.forEach(response.Data, function (item) {
                            setTrunkDescriptions(item);
                        });

                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function editTrunk(gridObject) {

            var onTrunkUpdated = function (firstTrunkObject, linkedToFirstTrunkID, secondTrunkID) {

                setTrunkDescriptions(firstTrunkObject);
                gridAPI.itemUpdated(firstTrunkObject);

                updateDataItem(linkedToFirstTrunkID, null, null);

                var secondTrunkObject = UtilsService.getItemByVal($scope.trunks, secondTrunkID, "ID");
                var linkedToSecondTrunkID = (secondTrunkObject != null) ? secondTrunkObject.LinkedToTrunkID : null;

                updateDataItem(linkedToSecondTrunkID, null, null);
                updateDataItem(secondTrunkID, firstTrunkObject.ID, firstTrunkObject.Name);
            }

            PSTN_BE_Service.editSwitchTrunk(gridObject, onTrunkUpdated);
        }               

        function deleteTrunk(gridObject) {

            var onTrunkDeleted = function (deletedTrunkObject, linkedToTrunkID) {
                gridAPI.itemDeleted(deletedTrunkObject);
                updateDataItem(linkedToTrunkID, null, null);
            }

            PSTN_BE_Service.deleteSwitchTrunk(gridObject, onTrunkDeleted);
        }
        
        function updateDataItem(dataItemID, linkedToTrunkID, linkedToTrunkName) {

            if (dataItemID != null) {
                var dataItem = UtilsService.getItemByVal($scope.trunks, dataItemID, "ID");

                if (dataItem != null) {
                    dataItem = UtilsService.cloneObject(dataItem, true);

                    dataItem.LinkedToTrunkID = linkedToTrunkID;
                    dataItem.LinkedToTrunkName = linkedToTrunkName;

                    gridAPI.itemUpdated(dataItem);
                }
            }
        }

        function setTrunkDescriptions(trunkObject) {

            var type = UtilsService.getEnum(SwitchTrunkTypeEnum, "value", trunkObject.Type);
            trunkObject.TypeDescription = type.description;

            var direction = UtilsService.getEnum(SwitchTrunkDirectionEnum, "value", trunkObject.Direction);
            trunkObject.DirectionDescription = direction.description;
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
    }

    return directiveDefinitionObject;

}]);
