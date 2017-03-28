'use strict';

app.directive('vrCommonStatusdefinitionGrid', ['VR_Common_StatusDefinitionAPIService', 'VR_Common_StatusDefinitionService', 'VRNotificationService', 'VRUIUtilsService',
    function (VR_Common_StatusDefinitionAPIService, VR_Common_StatusDefinitionService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var statusDefinitionGrid = new RetailBeStatusDefinitionGrid($scope, ctrl, $attrs);
                statusDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/StatusDefinition/Templates/StatusDefinitionGridTemplate.html'
        };

        function RetailBeStatusDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.statusDefinition = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_Common_StatusDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Common_StatusDefinitionAPIService.GetFilteredStatusDefinitions(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onStatusDefinitionAdded = function (addedStatusDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedStatusDefinition);
                    gridAPI.itemAdded(addedStatusDefinition);
                };

                api.onStatusDefinitionUpdated = function (updatedStatusDefinition) {
                    gridAPI.itemUpdated(updatedStatusDefinition);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editStatusDefinition,
                    haspermission: hasEditServiceTypePermission
                });
            }

            function hasEditServiceTypePermission() {
                return VR_Common_StatusDefinitionAPIService.HasUpdateStatusDefinitionPermission();
            }
            function editStatusDefinition(statusDefinitionItem) {
                var onStatusDefinitionUpdated = function (updatedStatusDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedStatusDefinition);
                    gridAPI.itemUpdated(updatedStatusDefinition);
                };

                VR_Common_StatusDefinitionService.editStatusDefinition(statusDefinitionItem.Entity.StatusDefinitionId, onStatusDefinitionUpdated);
            }
        }
    }]);
