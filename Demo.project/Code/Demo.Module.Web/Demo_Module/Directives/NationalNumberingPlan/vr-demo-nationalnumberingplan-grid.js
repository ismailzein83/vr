"use strict";

app.directive("vrDemoOperatorprofileGrid", ["UtilsService", "VRNotificationService", "Demo_OperatorProfileAPIService", "Demo_OperatorAccountService", "Demo_OperatorProfileService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_OperatorProfileAPIService, Demo_OperatorAccountService, Demo_OperatorProfileService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var operatorProfileGrid = new OperatorProfileGrid($scope, ctrl, $attrs);
            operatorProfileGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/OperatorAccount/Templates/OperatorProfileGridTemplate.html"

    };

    function OperatorProfileGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.operatorProfiles = [];
            defineMenuActions();
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Operator Account";
                drillDownDefinition.directive = "vr-demo-operatoraccount-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, operatorProfileItem) {
                    operatorProfileItem.operatorAccountGridAPI = directiveAPI;
                    var payload = {
                        query: {
                            OperatorProfilesIds: [operatorProfileItem.Entity.OperatorProfileId]
                        },
                        hideProfileColumn: true
                    };
                    return operatorProfileItem.operatorAccountGridAPI.loadGrid(payload);
                };
                drillDownDefinitions.push(drillDownDefinition);
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onOperatorProfileAdded = function (operatorProfileObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(operatorProfileObject);
                        gridAPI.itemAdded(operatorProfileObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_OperatorProfileAPIService.GetFilteredOperatorProfiles(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editOperatorProfile,
            },
                {
                    name: "New Operator Account",
                    clicked: addOperatorAccount
                }
            ];
        }

        function editOperatorProfile(operatorProfileObj) {
            var onOperatorProfileUpdated = function (operatorProfile) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(operatorProfile);
                gridAPI.itemUpdated(operatorProfile);

            }
            Demo_OperatorProfileService.editOperatorProfile(operatorProfileObj, onOperatorProfileUpdated);
        }

        function addOperatorAccount(dataItem) {
            gridAPI.expandRow(dataItem);
            var query = {
                OperatorProfilesIds: [dataItem.OperatorProfileId],
            }

            var onOperatorAccountAdded = function (operatorAccountObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(operatorAccountObj);
                if (dataItem.operatorAccountGridAPI != undefined)
                    dataItem.operatorAccountGridAPI.onOperatorAccountAdded(operatorAccountObj);
            };
            Demo_OperatorAccountService.addOperatorAccount(onOperatorAccountAdded, dataItem.Entity);
        }

        function deleteOperatorProfile(operatorProfileObj) {
            var onOperatorProfileDeleted = function () {
                //TODO: This is to refresh the Grid after delete, should be removed when centralized
                retrieveData();
            };

            // Demo_MainService.deleteOperatorAccount(operatorProfileObj, onOperatorProfileDeleted); to be added in OperatorAccountService
        }
    }

    return directiveDefinitionObject;

}]);