"use strict";

app.directive("vrWhsBeSwitchGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SwitchAPIService", "WhS_BE_SwitchService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_BE_SwitchAPIService, WhS_BE_SwitchService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
            //= (means function), @ (means attribute)
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var switchGrid = new SwitchGrid($scope, ctrl, $attrs);
            switchGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Switch/Templates/SwitchGridTemplate.html"

    };

    function SwitchGrid($scope, ctrl, $attrs) {
        var gridAPI;
        var gridDrillDownTabsObj;
        function initializeController() {
            $scope.switches = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = WhS_BE_SwitchService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.menuActions, true);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onSwitchDeleted = function (switchObject) {
                        gridAPI.itemDeleted(switchObject);
                    };

                    directiveAPI.onSwitchUpdated = function (switchObject) {
                        gridAPI.itemUpdated(switchObject);
                    };

                    directiveAPI.onSwitchAdded = function (switchObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(switchObject);
                        gridAPI.itemAdded(switchObject);
                    };

                    return directiveAPI;
                }

            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SwitchAPIService.GetFilteredSwitches(dataRetrievalInput)
                   .then(function (response) {
                       if (response && response.Data) {
                           for (var i = 0; i < response.Data.length; i++) {
                               gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);

                           }
                       }
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editSwitch,
                haspermission: hasUpdateSwitchPermission
            }
            ];
        }

        function hasUpdateSwitchPermission() {
            return WhS_BE_SwitchAPIService.HasUpdateSwitchPermission();
        }

        function hasDeleteSwitchPermission() {
            return WhS_BE_SwitchAPIService.HasDeleteSwitchPermission();
        }

        function editSwitch(whsSwitch) {
            var onSwitchUpdated = function (updatedItem) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(updatedItem);
                gridAPI.itemUpdated(updatedItem);
            };
            WhS_BE_SwitchService.editSwitch(whsSwitch.Entity.SwitchId, onSwitchUpdated);
        }

        function deleteSwitch(whsSwitch) {
            var onSwitchDeleted = function () {
                gridAPI.itemDeleted(whsSwitch);
            };

            WhS_BE_SwitchService.deleteSwitch($scope, whsSwitch.Entity.SwitchId, onSwitchDeleted);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
