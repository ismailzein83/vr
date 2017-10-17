"use strict";

app.directive("whsBeSwitchreleasecauseGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SwitchReleaseCauseAPIService", "WhS_BE_SwitchReleaseCauseService", "VRUIUtilsService","VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, WhS_BE_SwitchReleaseCauseAPIService, WhS_BE_SwitchReleaseCauseService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var switchReleaseCauseGrid = new SwitchReleaseCauseGrid($scope, ctrl, $attrs);
            switchReleaseCauseGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SwitchReleaseCause/Templates/SwitchReleaseCauseGridTemplate.html"
    };

    function SwitchReleaseCauseGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;
        var filterobj = {};
        var gridAPI;
        var gridDrillDownTabsObj;
        function initializeController() {
            $scope.switcheReleaseCauses = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = getDrillDownDefinitions();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                
          
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onSwitchReleaseCauseAdded = function (switchReleaseCauseObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(switchReleaseCauseObject);
                        gridAPI.itemAdded(switchReleaseCauseObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SwitchReleaseCauseAPIService.GetFilteredSwitchReleaseCauses(dataRetrievalInput)
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
                clicked: editSwitchReleaseCause,
            }];
        }

        function editSwitchReleaseCause(switchReleaseCauseObject) {
            var onSwitchReleaseCauseUpdated = function (updatedItem) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(updatedItem);
                gridAPI.itemUpdated(updatedItem);
            };
            var switchReleaseCauseId = switchReleaseCauseObject.SwitchReleaseCauseId;

            WhS_BE_SwitchReleaseCauseService.editSwitchReleaseCause(switchReleaseCauseId, onSwitchReleaseCauseUpdated);
        }
        function getDrillDownDefinitions() {
            var drillDownDefinitions = [];

            AddObjectTrackingDrillDownDefinition();

            function AddObjectTrackingDrillDownDefinition() {
                var objectTrackingDrillDownDefinition = {
                    title: VRCommon_ObjectTrackingService.getObjectTrackingGridTitle(),
                    directive: 'vr-common-objecttracking-grid',
                    loadDirective: function (directiveAPI, switchReleaseCauseObject) {
                        switchReleaseCauseObject.objectTrackingGridAPI = directiveAPI;
                        var query = {
                            ObjectId: switchReleaseCauseObject.SwitchReleaseCauseId,
                            EntityUniqueName: WhS_BE_SwitchReleaseCauseService.getEntityUniqueName()
                        };
                        return switchReleaseCauseObject.objectTrackingGridAPI.load(query);
                    }
                };
                drillDownDefinitions.push(objectTrackingDrillDownDefinition);
            }

            return drillDownDefinitions;
        }
    }
    return directiveDefinitionObject;
}]);