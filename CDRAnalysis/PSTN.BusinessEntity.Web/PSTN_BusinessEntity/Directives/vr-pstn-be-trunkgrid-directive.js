﻿"use strict";

app.directive("vrPstnBeTrunkgrid", ["PSTN_BE_Service", "SwitchTrunkAPIService", "SwitchTrunkTypeEnum", "SwitchTrunkDirectionEnum", "UtilsService", "VRNotificationService",
    function (PSTN_BE_Service, SwitchTrunkAPIService, SwitchTrunkTypeEnum, SwitchTrunkDirectionEnum, UtilsService, VRNotificationService) {
    
    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            switchid: "@",
            pagingtype: "@"
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
        template: function (element, attrs) {
            
            return getTemplate(attrs);
        }

    };

    function TrunkGrid($scope, ctrl) {

        var gridAPI;
        this.defineScope = defineScope;

        function defineScope()
        {
            $scope.trunks = [];

            $scope.onGridReady = function (api) {

                gridAPI = api;

                var directiveAPI = getDirectiveAPI();

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(directiveAPI);

                if (ctrl.switchid != undefined)
                    return directiveAPI.retrieveData({});

                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.retrieveData = function (filterObject) {
                        var query = (ctrl.switchid != undefined) ? { SelectedSwitchIDs: [ctrl.switchid] } : filterObject;
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onTrunkAdded = function (trunkObject) {
                        setTrunkDescriptions(trunkObject);
                        gridAPI.itemAdded(trunkObject);
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
                updateDataItem(secondTrunkID, firstTrunkObject.LinkedToTrunkID, firstTrunkObject.LinkedToTrunkName);

                var secondTrunkObject = UtilsService.getItemByVal(ctrl.datasource, secondTrunkID, "ID");
                var linkedToSecondTrunkID = (secondTrunkObject != null) ? secondTrunkObject.LinkedToTrunkID : null;
                updateDataItem(linkedToSecondTrunkID, null, null);
            }

            PSTN_BE_Service.editSwitchTrunk(gridObject, onTrunkUpdated);

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
        }               

        function deleteTrunk(gridObject) {

            var onTrunkDeleted = function (deletedTrunkObject) {
                gridAPI.itemDeleted(deletedTrunkObject);
            }

            PSTN_BE_Service.deleteSwitchTrunk(gridObject, onTrunkDeleted);
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

    function getTemplate(attrs) {

        var pagingType = (attrs.pagingtype != undefined) ? attrs.pagingtype : 'PagingOnScroll';

        return '<vr-row>'
            + '<vr-columns width="fullrow">'
                + '<vr-datagrid datasource="trunks" '
                    + ' on-ready="onGridReady" '
                    + ' dataretrievalfunction="dataRetrievalFunction" '
                    + ' pagingtype="\'' + pagingType + '\'" '
                    + ' menuactions="gridMenuActions" '
                    + ' idfield="ID" >'

                        + '<vr-datagridcolumn headertext="\'Name\'" field="\'Name\'" type="\'Text\'"></vr-datagridcolumn>'
                        + '<vr-datagridcolumn headertext="\'Symbol\'" field="\'Symbol\'" type="\'Text\'"></vr-datagridcolumn>'
                        + '<vr-datagridcolumn headertext="\'Switch\'" field="\'SwitchName\'" type="\'Text\'"></vr-datagridcolumn>'
                        + '<vr-datagridcolumn headertext="\'Type\'" field="\'TypeDescription\'" type="\'Text\'"></vr-datagridcolumn>'
                        + '<vr-datagridcolumn headertext="\'Direction\'" field="\'DirectionDescription\'" type="\'Text\'"></vr-datagridcolumn>'
                        + '<vr-datagridcolumn headertext="\'Linked-To Trunk\'" field="\'LinkedToTrunkName\'" type="\'Text\'"></vr-datagridcolumn>'
                + '</vr-datagrid>'
            + '</vr-columns>'
        + '</vr-row>';
    }

    return directiveDefinitionObject;

}]);
