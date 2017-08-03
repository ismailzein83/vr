"use strict";

app.directive("cdranalysisPstnTrunkGrid", ["CDRAnalysis_PSTN_TrunkService", "CDRAnalysis_PSTN_TrunkAPIService", "TrunkTypeEnum", "TrunkDirectionEnum", "UtilsService", "VRNotificationService",
    function (CDRAnalysis_PSTN_TrunkService, CDRAnalysis_PSTN_TrunkAPIService, TrunkTypeEnum, TrunkDirectionEnum, UtilsService, VRNotificationService) {
    
    var directiveDefinitionObj = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var trunkGrid = new TrunkGrid($scope, ctrl);
            trunkGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
           
        },
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/Trunk/Templates/TrunkGridTemplate.html"

    };

    function TrunkGrid($scope, ctrl) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController()
        {
            $scope.trunks = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
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
                    };

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return CDRAnalysis_PSTN_TrunkAPIService.GetFilteredTrunks(dataRetrievalInput)
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
        function defineMenuActions() {
            $scope.gridMenuActions = [
               {
                   name: "Edit",
                   clicked: editTrunk,
                   haspermission: hasUpdateTrunkPermission
               },
               {
                   name: "Delete",
                   clicked: deleteTrunk,
                   haspermission: hasDeleteTrunkPermission
               }
            ];

            function hasUpdateTrunkPermission() {
                return CDRAnalysis_PSTN_TrunkAPIService.HasUpdateTrunkPermission();
            }

            function hasDeleteTrunkPermission() {
                return CDRAnalysis_PSTN_TrunkAPIService.HasDeleteTrunkPermission();
            }
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
            };

            CDRAnalysis_PSTN_TrunkService.editTrunk(gridObj.Entity.TrunkId, onTrunkUpdated);
        }               

        function deleteTrunk(gridObj) {
            var onTrunkDeleted = function (deletedTrunkObj, linkedToTrunkId) {
                gridAPI.itemDeleted(deletedTrunkObj);
                updateDataItem(linkedToTrunkId, null, null);
            };
            CDRAnalysis_PSTN_TrunkService.deleteTrunk(gridObj, onTrunkDeleted);
        }
        
        function updateDataItem(dataItemId, linkedToTrunkId, linkedToTrunkName) {

            if (dataItemId != null) {
                if ($scope.trunks != undefined && $scope.trunks != null && $scope.trunks.length > 0) {
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
        }

        function setTrunkDescriptions(trunkObj) {

            var type = UtilsService.getEnum(TrunkTypeEnum, "value", trunkObj.Entity.Type);
            trunkObj.TypeDescription = type.description;

            var direction = UtilsService.getEnum(TrunkDirectionEnum, "value", trunkObj.Entity.Direction);
            trunkObj.DirectionDescription = direction.description;
        }
    }
    return directiveDefinitionObj;

}]);
