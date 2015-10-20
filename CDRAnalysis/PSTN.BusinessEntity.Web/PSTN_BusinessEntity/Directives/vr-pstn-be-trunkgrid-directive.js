"use strict";

app.directive("vrPstnBeTrunkgrid", ["PSTN_BE_Service", "TrunkAPIService", "TrunkTypeEnum", "TrunkDirectionEnum", "UtilsService", "VRNotificationService",
    function (PSTN_BE_Service, TrunkAPIService, TrunkTypeEnum, TrunkDirectionEnum, UtilsService, VRNotificationService) {
    
    var directiveDefinitionObj = {

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
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/TrunkGridTemplate.html"

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

                    directiveAPI.onTrunkAdded = function (trunkObj) {
                        setTrunkDescriptions(trunkObj);
                        gridAPI.itemAdded(trunkObj);

                        var linkedToTrunkId = trunkObj.Entity.LinkedToTrunkId;
                        updateDataItem(linkedToTrunkId, trunkObj.Entity.TrunkId, trunkObj.Entity.Name);

                        if (linkedToTrunkId != null) {
                            var linkedToTheLinkedToTrunkObj = UtilsService.getItemByVal($scope.trunks, linkedToTrunkId, "LinkedToTrunkId");

                            if (linkedToTheLinkedToTrunkObj != null)
                                updateDataItem(linkedToTheLinkedToTrunkObj.TrunkId, null, null);
                        }
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return TrunkAPIService.GetFilteredTrunks(dataRetrievalInput)
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

        function editTrunk(gridObj) {

            var onTrunkUpdated = function (firstTrunkObj, linkedToFirstTrunkId, secondTrunkId) {

                setTrunkDescriptions(firstTrunkObj);
                gridAPI.itemUpdated(firstTrunkObj);

                updateDataItem(linkedToFirstTrunkId, null, null);

                var entities = UtilsService.getPropValuesFromArray($scope.trunks, "Entity");
                var dataItemIndex = UtilsService.getItemIndexByVal(entities, secondTrunkId, "TrunkId");

                var secondTrunkObj = $scope.trunks[dataItemIndex];
                var linkedToSecondTrunkId = (secondTrunkObj != null) ? secondTrunkObj.Entity.LinkedToTrunkId : null;



                updateDataItem(linkedToSecondTrunkId, null, null);
                updateDataItem(secondTrunkId, firstTrunkObj.Entity.TrunkId, firstTrunkObj.Entity.Name);
            }

            PSTN_BE_Service.editTrunk(gridObj, onTrunkUpdated);
        }               

        function deleteTrunk(gridObj) {

            var onTrunkDeleted = function (deletedTrunkObj, linkedToTrunkId) {
                gridAPI.itemDeleted(deletedTrunkObj);
                updateDataItem(linkedToTrunkId, null, null);
            }

            PSTN_BE_Service.deleteTrunk(gridObj, onTrunkDeleted);
        }
        
        function updateDataItem(dataItemId, linkedToTrunkId, linkedToTrunkName) {

            if (dataItemId != null) {
                var entities = UtilsService.getPropValuesFromArray($scope.trunks, "Entity");
                var dataItemIndex = UtilsService.getItemIndexByVal(entities, dataItemId, "TrunkId");

                var dataItem = $scope.trunks[dataItemIndex];

                if (dataItem != null) {
                    dataItem = UtilsService.cloneObject(dataItem, true);

                    dataItem.Entity.LinkedToTrunkId = linkedToTrunkId;
                    dataItem.LinkedToTrunkName = linkedToTrunkName;

                    gridAPI.itemUpdated(dataItem);
                }
            }
        }

        function setTrunkDescriptions(trunkObj) {

            var type = UtilsService.getEnum(TrunkTypeEnum, "value", trunkObj.Entity.Type);
            trunkObj.TypeDescription = type.description;

            var direction = UtilsService.getEnum(TrunkDirectionEnum, "value", trunkObj.Entity.Direction);
            trunkObj.DirectionDescription = direction.description;
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

    return directiveDefinitionObj;

}]);
