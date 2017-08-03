"use strict";

app.directive("cdranalysisPstnSwitchGrid", ["CDRAnalysis_PSTN_SwitchService", "CDRAnalysis_PSTN_SwitchAPIService", "UtilsService", "VRNotificationService","VRUIUtilsService",
    function (CDRAnalysis_PSTN_SwitchService, CDRAnalysis_PSTN_SwitchAPIService, UtilsService, VRNotificationService , VRUIUtilsService) {
    
    var directiveDefinitionObj = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var switchGrid = new SwitchGrid($scope, ctrl);
            switchGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
           
        },
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/Switch/Templates/SwitchGridTemplate.html"

    };

    function SwitchGrid($scope, ctrl) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController()
        {
            $scope.switches = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = CDRAnalysis_PSTN_SwitchService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.retrieveData = function (query) {
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onSwitchAdded = function (switchObj) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(switchObj);
                        gridAPI.itemAdded(switchObj);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CDRAnalysis_PSTN_SwitchAPIService.GetFilteredSwitches(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
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
            defineMenuActions();
            
        }
        function defineMenuActions() {
            $scope.gridMenuActions = [
               {
                   name: "Edit",
                   clicked: editSwitch,
                   haspermission: hasUpdateSwitchPermission
               },
               {
                   name: "Delete",
                   clicked: deleteSwitch,
                   haspermission: hasDeleteSwitchPermission
               }
            ];

            function hasUpdateSwitchPermission() {
                return CDRAnalysis_PSTN_SwitchAPIService.HasUpdateSwitchPermission();
            }
            function hasDeleteSwitchPermission() {
                return CDRAnalysis_PSTN_SwitchAPIService.HasDeleteSwitchPermission();
            }

        }


        function editSwitch(switchObj) {
            var onSwitchUpdated = function (switchObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(switchObj);
                gridAPI.itemUpdated(switchObj);
            };
            CDRAnalysis_PSTN_SwitchService.editSwitch(switchObj.Entity.SwitchId, onSwitchUpdated);
        }

        function deleteSwitch(switchObj) {
            var onSwitchDeleted = function (deletedSwitchObj) {
                gridAPI.itemDeleted(deletedSwitchObj);
            };
            CDRAnalysis_PSTN_SwitchService.deleteSwitch(switchObj, onSwitchDeleted);
        }
    }

    return directiveDefinitionObj;

}]);
