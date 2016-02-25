"use strict";

app.directive("vrInterconnectBeOperatorprofileGrid", ["UtilsService", "VRNotificationService", "InterConnect_BE_OperatorProfileAPIService", "InterConnect_BE_OperatorProfileService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, InterConnect_BE_OperatorProfileAPIService, InterConnect_BE_OperatorProfileService, VRUIUtilsService) {

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
        templateUrl: "/Client/Modules/InterConnect_BusinessEntity/Directives/OperatorProfile/Templates/OperatorProfileGridTemplate.html"

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

                drillDownDefinition.title = "Operator Accounts";
                drillDownDefinition.directive = "vr-interconnect-be-operatoraccount-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, operatorProfileItem) {
                    operatorProfileItem.operatorAccountGridAPI = directiveAPI;
                    var operatorProfileIds = [];
                    operatorProfileIds.push(operatorProfileItem.Entity.OperatorProfileId);
                    var payload = {
                        OperatorProfileIds: operatorProfileIds,
                        isDrillDownOperatorProfile: true
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
                return InterConnect_BE_OperatorProfileAPIService.GetFilteredOperatorProfiles(dataRetrievalInput)
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
            }];
        }

        function editOperatorProfile(operatorProfileObj) {
            var onOperatorProfileUpdated = function (operatorProfile) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(operatorProfile);
                gridAPI.itemUpdated(operatorProfile);
            }
            InterConnect_BE_OperatorProfileService.editOperatorProfile(operatorProfileObj, onOperatorProfileUpdated);
        }
    }

    return directiveDefinitionObject;

}]);