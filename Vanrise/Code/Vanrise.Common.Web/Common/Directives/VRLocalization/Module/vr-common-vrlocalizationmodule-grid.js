"use strict";

app.directive("vrCommonLocalizationmoduleGrid", [ "VRUIUtilsService","VRNotificationService", 'VRCommon_VRLocalizationModuleAPIService', 'VRCommon_VRLocalizationModuleService',
    function ( VRUIUtilsService, VRNotificationService, VRCommon_VRLocalizationModuleAPIService, VRCommon_VRLocalizationModuleService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrLocalizationModuleGrid = new VRLocalizationModuleGrid($scope, ctrl, $attrs);
                vrLocalizationModuleGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: '/Client/Modules/Common/Directives/VRLocalization/Module/Templates/VRLocalizationModuleGridTemplate.html'
        };
        function VRLocalizationModuleGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var gridDrillDownTabsObj;
            var drillDownDefinitionsArray = [];
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.vrLocalizationModules = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;

                    drillDownDefinitionsArray = VRCommon_VRLocalizationModuleService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitionsArray, gridAPI, $scope.scopeModel.menuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRLocalizationModuleAPIService.GetFilteredVRLocalizationModules(dataRetrievalInput)
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

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onVRLocalizationModuleAdded = function (addedVRLocalizationModule) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedVRLocalizationModule);
                    gridAPI.itemAdded(addedVRLocalizationModule);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: "Edit",
                    clicked: editVRLocalizationModule
                }];
            }

            function editVRLocalizationModule(vrLocalizationModuleItem) {
                var onVRLocalizationModuleUpdated = function (updatedvrLocalizationModule) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedvrLocalizationModule);
                    gridAPI.itemUpdated(updatedvrLocalizationModule);
                };
                VRCommon_VRLocalizationModuleService.editVRLocalizationModule(vrLocalizationModuleItem.VRLocalizationModuleId, onVRLocalizationModuleUpdated);
            }
        }

        return directiveDefinitionObject;
    }
]);