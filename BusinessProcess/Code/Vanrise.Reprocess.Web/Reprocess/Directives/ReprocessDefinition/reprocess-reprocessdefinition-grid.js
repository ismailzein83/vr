'use strict';

app.directive('reprocessReprocessdefinitionGrid', ['Reprocess_ReprocessDefinitionAPIService', 'Reprocess_ReprocessDefinitionService', 'VRNotificationService', 'VRUIUtilsService',
    function (Reprocess_ReprocessDefinitionAPIService, Reprocess_ReprocessDefinitionService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var reprocessDefinitionGrid = new ReprocessDefinitionGrid($scope, ctrl, $attrs);
                reprocessDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Reprocess/Directives/ReprocessDefinition/Templates/ReprocessDefinitionGridTemplate.html'
        };

        function ReprocessDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.reprocessDefinition = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = Reprocess_ReprocessDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Reprocess_ReprocessDefinitionAPIService.GetFilteredReprocessDefinitions(dataRetrievalInput).then(function (response) {
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

                api.onReprocessDefinitionAdded = function (addedReprocessDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedReprocessDefinition);
                    gridAPI.itemAdded(addedReprocessDefinition);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editReprocessDefinition,
                    haspermission: hasEditReprocessDefinitionPermission
                });
            }
            function hasEditReprocessDefinitionPermission() {
                  return Reprocess_ReprocessDefinitionAPIService.HasUpdateReprocessDefinitionPermission();
            }

            function editReprocessDefinition(reprocessDefinitionItem) {
                var onReprocessDefinitionUpdated = function (updatedReprocessDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedReprocessDefinition);
                    gridAPI.itemUpdated(updatedReprocessDefinition);
                };

                Reprocess_ReprocessDefinitionService.editReprocessDefinition(reprocessDefinitionItem.Entity.ReprocessDefinitionId, onReprocessDefinitionUpdated);
            }
        }
    }]);
